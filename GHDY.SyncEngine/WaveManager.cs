using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace GHDY.SyncEngine
{
    public static class WaveManager
    {
        [DllImport("winmm.dll")]
        private static extern uint mciSendString(
            string command,
            StringBuilder returnValue,
            int returnLength,
            IntPtr winHandle);

        public static TimeSpan GetAudioLengthOfTimeSpan(string file)
        {
            return TimeSpan.FromMilliseconds(GetAudioLengthOfSeconds(file));
        }

        // 毫秒
        public static double GetAudioLengthOfSeconds(string file)
        {
            string waveFile = null;

            if (Path.GetExtension(file) != ".wav")
            {
                waveFile = Path.ChangeExtension(file, "wav");

                if (File.Exists(waveFile) == false)
                    new WaveDecoder().Process(file, waveFile);
            }
            else
                waveFile = file;

            StringBuilder lengthBuf = new StringBuilder(32);

            mciSendString(string.Format("open \"{0}\" type Wave Audio alias wave", waveFile), null, 0, IntPtr.Zero);
            mciSendString("status wave length", lengthBuf, lengthBuf.Capacity, IntPtr.Zero);
            mciSendString("close wave", null, 0, IntPtr.Zero);

            double length = 0;
            double.TryParse(lengthBuf.ToString(), out length);

            return length/1000;
        }


        public static void Split(string waveSource, double startSeconds, double endSeconds, string SavePath)
        {
            var sourceBytes = File.ReadAllBytes(waveSource);

            var waveInfo = GetWavInfo(waveSource);

            var start = startSeconds * waveInfo.SampleRate * waveInfo.BitesPerSample / 8;
            var end = endSeconds * waveInfo.SampleRate * waveInfo.BitesPerSample / 8;
            var len = end - start;

            var bytePerSample = waveInfo.BitesPerSample / 8;

            start -= start % bytePerSample;

            var header = sourceBytes.Take(40 + (int)(waveInfo.Subchunk1Size - 16)).Concat(BitConverter.GetBytes((int)len)).ToArray();

            var data = sourceBytes.Skip(header.Length).Skip((int)start).Take((int)len);

            File.WriteAllBytes(SavePath, header.Concat(data).ToArray());
        }


        /// <summary>
        /// 制作静音的音频
        /// </summary>
        /// <param name="referWaveFile">制作静音音频参照的音频，如果参照音频的采样率是44100，那么静音音频的采样率也是44100</param>
        /// <param name="seconds">制作音频的长度（秒）</param>
        /// <returns></returns>
        public static string CreateSilentWave(string referWaveFile, double seconds)
        {
            var waveInfo = GetWavInfo(referWaveFile);
            var header = waveInfo.Header;

            int len = (int)(seconds * Convert.ToInt32(waveInfo.SampleRate) * Convert.ToInt32(waveInfo.BlockAlign));
            var data = new byte[len];
            for (int i = 0; i < len; i++)
                data[i] = 0;

            var bytes = header.Take(40).Concat(BitConverter.GetBytes((int)len)).Concat(data);

            var result = seconds + ".wav";
            File.WriteAllBytes(result, bytes.ToArray());

            return result;
        }

        public static string CreateSilentWave(string referWaveFile, TimeSpan length)
        {
            return CreateSilentWave(referWaveFile, length.TotalSeconds);
        }


        public struct WavInfo
        {
            // The canonical WAVE format starts with the RIFF header:
            public string ChunkID; // RIFF: Resource Interchange File Format

            public string Format; // WAVE

            public long ChunkSize; // 36 + SubChunk2Size, or more precisely:
            //4 + (8 + SubChunk1Size) + (8 + SubChunk2Size)
            //This is the size of the rest of the chunk 
            //following this number.  This is the size of the 
            //entire file in bytes minus 8 bytes for the
            //two fields not included in this count:
            //ChunkID and ChunkSize.


            // The "WAVE" format consists of two subchunks: "fmt " and "data":

            // The "fmt " subchunk describes the sound data's format:
            public string Subchunk1ID;

            public long Subchunk1Size; //16 for PCM.  This is the size of the
            //rest of the Subchunk which follows this number.

            public short AudioFormat; //记录着此声音的格式代号，例如WAVE_FORMAT_PCM，WAVE_F0RAM_ADPCM等等。 
            // PCM = 1 (i.e. Linear quantization)
            //Values other than 1 indicate some 
            //form of compression.

            public ushort NumChannels; // Mono = 1, Stereo = 2, etc.

            public ulong SampleRate;//记录每秒取样数。  8000, 44100, etc.

            public ulong ByteRate;//记录每秒的数据量。  == SampleRate * NumChannels * BitsPerSample/8

            public ushort BlockAlign;//记录区块的对齐单位。 == NumChannels * BitsPerSample/8
            //The number of bytes for one sample including
            //all channels. I wonder what happens when
            //this number isn't an integer?

            public ushort BitesPerSample;//记录每个取样所需的位元数。   bits = 8, 16 bits = 16, etc.

            // 2   ExtraParamSize   if PCM, then doesn't exist
            //X   ExtraParams      space for extra parameters


            //The "data" subchunk contains the size of the data and the actual sound:

            public string Subchunk2ID;
            // Contains the letters "data"
            //(0x64617461 big-endian form).

            public long Subchunk2Size; //   == NumSamples * NumChannels * BitsPerSample/8
            //This is the number of bytes in the data.
            //You can also think of this as the size
            //of the read of the subchunk following this 
            //number.

            public byte[] Data; // The actual sound data.
            public byte[] Header;

            public byte[] ToBytes()
            {
                throw new NotImplementedException();
            }
        }

        public static WavInfo GetWavInfo(string strpath)
        {
            WavInfo wavInfo = new WavInfo();

            FileInfo fi = new FileInfo(strpath);

            using (System.IO.FileStream fs = fi.OpenRead())
            {
                if (fs.Length >= 44)
                {
                    byte[] bInfo = new byte[44];

                    fs.Read(bInfo, 0, 44);

                    wavInfo.Header = bInfo;

                    System.Text.Encoding.Default.GetString(bInfo, 0, 4);

                    if (System.Text.Encoding.Default.GetString(bInfo, 0, 4) == "RIFF"
                        && System.Text.Encoding.Default.GetString(bInfo, 8, 4) == "WAVE"
                        && System.Text.Encoding.Default.GetString(bInfo, 12, 4) == "fmt ")
                    {
                        wavInfo.ChunkID = System.Text.Encoding.Default.GetString(bInfo, 0, 4);

                        wavInfo.ChunkSize = System.BitConverter.ToInt32(bInfo, 4);

                        //wavInfo.filesize = Convert.ToInt64(System.Text.Encoding.Default.GetString(bInfo,4,4)); 

                        wavInfo.Format = System.Text.Encoding.Default.GetString(bInfo, 8, 4);

                        wavInfo.Subchunk1ID = System.Text.Encoding.Default.GetString(bInfo, 12, 4);

                        wavInfo.Subchunk1Size = System.BitConverter.ToInt32(bInfo, 16);

                        wavInfo.AudioFormat = System.BitConverter.ToInt16(bInfo, 20);

                        wavInfo.NumChannels = System.BitConverter.ToUInt16(bInfo, 22);

                        wavInfo.SampleRate = System.BitConverter.ToUInt32(bInfo, 24);

                        wavInfo.ByteRate = System.BitConverter.ToUInt32(bInfo, 28);

                        wavInfo.BlockAlign = System.BitConverter.ToUInt16(bInfo, 32);

                        wavInfo.BitesPerSample = System.BitConverter.ToUInt16(bInfo, 34);

                        wavInfo.Subchunk2ID = System.Text.Encoding.Default.GetString(bInfo, 36, 4);

                        wavInfo.Subchunk2Size = System.BitConverter.ToInt32(bInfo, 40);

                        //System.Console.WriteLine("NumChannels:" + wavInfo.NumChannels);
                        //System.Console.WriteLine("SampleRate:" + wavInfo.SampleRate);
                        //System.Console.WriteLine("file real size:" + (wavInfo.Subchunk2Size + 44) + " bytes");

                        //System.Console.WriteLine("ChunkID:" + wavInfo.ChunkID);

                        //System.Console.WriteLine("ChunkSize:" + wavInfo.ChunkSize);

                        ////System.Console.WriteLine("file real size:" + (wavInfo.ChunkSize + 8)); // TODO error

                        //System.Console.WriteLine("Format:" + wavInfo.Format);

                        //System.Console.WriteLine("Subchunk1ID:" + wavInfo.Subchunk1ID);

                        //System.Console.WriteLine("Subchunk1Size:" + wavInfo.Subchunk1Size);

                        //System.Console.WriteLine("AudioFormat:" + wavInfo.AudioFormat);

                        //System.Console.WriteLine("ByteRate:" + wavInfo.ByteRate);

                        //System.Console.WriteLine("BlockAlign:" + wavInfo.BlockAlign);

                        //System.Console.WriteLine("BitesPerSample:" + wavInfo.BitesPerSample);

                        //System.Console.WriteLine("Subchunk2ID:" + wavInfo.Subchunk2ID);

                        //System.Console.WriteLine("Subchunk2Size:" + wavInfo.Subchunk2Size + " bytes");


                        return wavInfo;
                    }
                }
            }

            return new WavInfo();
        }
    }
}
