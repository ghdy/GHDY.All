using GHDY.Core;
using GHDY.Core.DocumentModel;
using GHDY.SyncEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dictation
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentFilePath = "";
            args = new string[1] { Path.Combine(Environment.CurrentDirectory, "SGTV.wav") };
            if (args != null && args.Length > 0)
            {
                var str = args[0];
                Console.WriteLine("Args : " + str);

                var waveFile = str + ".wav";// str.ToLower().Replace(".mp3", ".wav");
                var lrcFile = str + ".lrc";
                currentFilePath = str + ".xml";
                if (File.Exists(currentFilePath) == true)
                {
                    Console.WriteLine("xml file is E");
                    DMDocument doc = DMDocument.Load(currentFilePath);
                    Lyrics lrc = new Lyrics();

                    foreach (var sentence in doc.Sentences)
                    {
                        LyricsPhrase phrase = new LyricsPhrase() {
                            BeginTime = sentence.BeginTime,
                            EndTime = sentence.EndTime,
                            Text = sentence.Text
                        };
                        lrc.Phrases.Add(phrase);
                    }
                    lrc.Save(lrcFile);
                }
                else
                {
                    WaveDecoder wd = new WaveDecoder();
                    wd.ProcessForRecognize(str, waveFile);

                    DictationSyncEngine engine = new DictationSyncEngine("en-US");

                    engine.SentenceRecognized += Engine_SentenceRecognized;
                    engine.RecognizeCompleted += Engine_RecognizeCompleted;
                    engine.Process(waveFile);
                }
            }
            else
                Console.WriteLine("Args is null!");
            Console.ReadLine();
        }

        static void Engine_RecognizeCompleted(object sender, System.Speech.Recognition.RecognizeCompletedEventArgs e)
        {
            DMDocument doc = new DMDocument();
            DMParagraph paragraph = new DMParagraph();


            if (sender is DictationSyncEngine engine)
            {
                for (int i = 0; i < engine.Result.Count; i++)
                {
                    var sentence = engine.Result[i];
                    DMSentence dmSentence = BuildSentence(i, sentence);
                    paragraph.Inlines.Add(dmSentence);
                }

                doc.Blocks.Add(paragraph);
            }

            doc.Save(Path.Combine(Environment.CurrentDirectory, "data.xml"));

            Console.WriteLine("Recognize Completed!");

            Console.WriteLine("Press any key to exists!");
        }

        private static DMSentence BuildSentence(int index, RecognizedSentence sentence)
        {
            var dmSentence = new DMSentence(index);

            for (int i = 0; i < sentence.Words.Count; i++)
            {
                var word = sentence.Words[i];
                SyncableWord syncWord = new SyncableWord(word.Text)
                {
                    BeginTime = word.Begin,
                    EndTime = word.End,
                    Confidence = word.Confidence,
                };
                dmSentence.Inlines.Add(syncWord);
            }

            return dmSentence;
        }

        static void Engine_SentenceRecognized(object sender, SentenceRecognizedEventArgs e)
        {
            Console.WriteLine(">>Recognized : " + e.Sentence.Text);
        }
    }
}
