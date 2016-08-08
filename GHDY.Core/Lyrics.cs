using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using GHDY.Core.DocumentModel;
using System.Web;

namespace GHDY.Core
{
    public class LyricsPhrase : ISyncable
    {
        public int Index { get; set; }

        public string Text { get; set; }

        public double Begin { get; set; }

        public double End { get; set; }

        public double Duration
        {
            get
            {
                return End - Begin;
            }
        }

        public TimeSpan BeginTime
        {
            get
            {
                return TimeSpan.FromSeconds(this.Begin);
            }
            set
            {
                this.Begin = value.TotalSeconds;
            }
        }

        public TimeSpan EndTime
        {
            get
            {
                return TimeSpan.FromSeconds(this.End);
            }
            set
            {
                this.End = value.TotalSeconds;
            }
        }

        public double Confidence
        {
            get
            {
                return 1;
            }
            set
            {

            }
        }

        public string ToSpeechText()
        {
            return this.Text;
        }
    }

    public class Lyrics
    {
        public string Name { get; set; }

        public Dictionary<string, string> InformationDict { get; set; }

        Encoding _encoding = Encoding.UTF8;
        public Encoding Encoding
        {
            get { return this._encoding; }
            set
            {
                this._encoding = value;
            }
        }

        string[] lines;

        public double offset { get; set; }

        public bool NeedSort { get; set; }

        public List<LyricsPhrase> Phrases { get; set; }

        public string Transcript
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (var phrase in this.Phrases)
                {
                    if (sb.Length > 0)
                        sb.Append(" ");
                    sb.Append(phrase.Text.Trim());
                }

                return sb.ToString().Trim();
            }
        }

        public Lyrics()
        {
            this.Phrases = new List<LyricsPhrase>();
            this.InformationDict = new Dictionary<string, string>();
            this.NeedSort = false;
        }

        public Lyrics(string lrcFilePath, bool sortByBeginTime)
        {
            this.NeedSort = sortByBeginTime;
            this.Phrases = new List<LyricsPhrase>();
            this.InformationDict = new Dictionary<string, string>();

            this.Name = Path.GetFileNameWithoutExtension(lrcFilePath);
            lines = File.ReadAllLines(lrcFilePath, this.Encoding);

            foreach (var line in lines)
            {
                Regex regex = new Regex("\\[[a-z]+?:.+?\\]");
                var formated = HttpUtility.HtmlEncode(line.Trim());
                Match match = regex.Match(formated);

                if (match.Success == true)
                {
                    var array = line.Trim().Split(new char[] { '[', ']', ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (array.Length == 2)
                    {
                        var key = array[0].Trim().ToLower();
                        if (key.Trim() == "offset")
                        {
                            if (array[1].Length > 0)
                            {
                                offset = double.Parse(array[1]) / 1000;
                            }

                        }
                        else if (key.Length == 2 && key.ToLower() != key.ToUpper())
                        {
                            if (InformationDict.ContainsKey(key) == false)
                                InformationDict.Add(key, array[1]);
                        }
                    }
                }
                else
                {
                    regex = new Regex("\\[\\d\\d:\\d\\d\\.\\d\\d\\]+", RegexOptions.Singleline);
                    var matches = regex.Matches(line);
                    var count = matches.Count;
                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            string time = matches[i].Value.Trim('[', ']').Trim();

                            var begin = stringToInterval(time) - this.offset;

                            if (begin <= 0)
                                continue;

                            string text = line.Substring(count * 10);

                            var phrase = new LyricsPhrase() { Text = text, Begin = begin };

                            this.Phrases.Add(phrase);
                        }
                    }
                }
            }

            if (this.NeedSort == true)
                this.Phrases.Sort(CompareListSmall);

            for (int i = 0; i < this.Phrases.Count; i++)
            {
                var phrase = this.Phrases[i];
                phrase.Index = i;
                if (i < this.Phrases.Count - 1)
                {
                    phrase.End = this.Phrases[i + 1].Begin;
                }
            }
            this.Phrases[this.Phrases.Count - 1].End = 60 * 60 * 4;

            File.WriteAllText(lrcFilePath, this.ToString(), this.Encoding);

        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var phrase in this.Phrases)
            {
                var line = "[" + intervalToString(phrase.Begin) + "]" + phrase.Text;
                sb.AppendLine(line);
            }

            return sb.ToString();
        }

        public Lyrics ToSentenceLyrics(string[] textArray)
        {
            Lyrics result = new Lyrics();
            int phraseIndex = 0;
            for (int i = 0; i < textArray.Length; i++)
            {
                var sentenceText = textArray[i].Trim();
                TimeSpan begin = TimeSpan.Zero;
                TimeSpan end = TimeSpan.Zero;

                for (int j = phraseIndex; j < this.Phrases.Count; j++)
                {
                    var phrase = this.Phrases[j];
                    var phraseText = phrase.Text.Trim();
                    if (begin <= TimeSpan.Zero)
                    {
                        begin = this.Phrases[j].BeginTime;
                    }
                    if (sentenceText.EndsWith(phraseText) == true)
                    {
                        end = phrase.EndTime;

                        result.Phrases.Add(new LyricsPhrase()
                        {
                            Text = sentenceText,
                            BeginTime = begin,
                            EndTime = end
                        });
                        phraseIndex = j + 1;
                        break;
                    }
                }
            }
            return result;
        }

        public bool FindTimeRange(string text, out TimeSpan begin, out TimeSpan end)
        {
            bool setFlag = false;
            begin = TimeSpan.Zero;
            end = TimeSpan.Zero;

            foreach (var phrase in this.Phrases)
            {
                var phraseText = phrase.Text;

                var temp = (double)phraseText.Length / (double)text.Length;

                if (temp < 1.1)
                {
                    var result = DiffHelper.matchString(text, phrase.Text);
                    temp = (double)(result.Same - result.Replace) / (double)phrase.Text.Length;

                    if (temp > 0.8)
                    {

                        if (setFlag == false)
                        {
                            begin = TimeSpan.FromSeconds(phrase.Begin);
                        }

                        try
                        {
                            end = TimeSpan.FromSeconds(phrase.End);
                        }
                        catch
                        {
                            end = TimeSpan.FromSeconds(phrase.Begin + 100);
                        }
                        setFlag = true;
                        continue;
                    }
                }

                if (setFlag == true)
                    break;
            }

            return setFlag;
        }

        public void Save(string filePath)
        {
            if (this.NeedSort == true)
                this.Phrases.Sort(CompareListSmall);

            StringBuilder sb = new StringBuilder();
            foreach (var phrase in this.Phrases)
            {
                sb.AppendLine(string.Format("[{0}]{1}", this.intervalToString(phrase.Begin), phrase.Text));
            }

            File.WriteAllText(filePath, sb.ToString(), this.Encoding);
        }

        private static int CompareListSmall(LyricsPhrase a, LyricsPhrase b)//由大到小
        {
            double _temp = a.Begin - b.Begin;
            if (_temp < 0) return -1;
            if (_temp > 0) return 1;
            return 0;
        }

        private string intervalToString(double interval)
        {
            int min;
            float sec;
            min = (int)interval / 60;
            sec = (float)(interval - (float)min * 60.0);
            String smin = String.Format("{0:d2}", min);
            String ssec = String.Format("{0:00.00}", sec);
            return smin + ":" + ssec;
        }

        private double stringToInterval(String str)
        {
            try
            {
                //str = str.Replace("-", "");
                double min = double.Parse(str.Split(':').GetValue(0).ToString());
                double sec = double.Parse(str.Split(':').GetValue(1).ToString());
                return min * 60.0 + sec;
            }
            catch
            {
                return TimeSpan.MaxValue.TotalSeconds;
            }
        }
    }

}