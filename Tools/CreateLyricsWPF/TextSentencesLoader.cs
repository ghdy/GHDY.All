using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateLyricsWPF
{
    public class TextSentencesLoader
    {
        public string FilePath { get; private set; }

        public List<KeyValuePair<string, string>> Sentences { get; set; }

        public TextSentencesLoader(string textFilePath)
        {
            this.FilePath = textFilePath;

            this.Sentences = new List<KeyValuePair<string, string>>();

            string text = File.ReadAllText(textFilePath, Encoding.UTF8);
            string[] array = text.Split(new string[] { "\r\n\r\n", "\t" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var str in array)
            {
                var lines = str.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length == 2)
                    this.Sentences.Add(new KeyValuePair<string, string>(lines[0], lines[1]));
                if (lines.Length == 1)
                    this.Sentences.Add(new KeyValuePair<string, string>(lines[0], ""));
            }
        }


    }
}
