using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OpenNLP.Tools.SentenceDetect;
using OpenNLP.Tools.NameFind;
using OpenNLP.Tools.Tokenize;
using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.Chunker;
using OpenNLP.Tools.Parser;

namespace GHDY.NLP
{
    public static class NlpUtilities
    {
        public static string MModelPath { get; private set; }

        public static string[] NameModels
        {
            get;
            private set;
        }

        static NlpUtilities()
        {
            MModelPath = Path.Combine(Environment.CurrentDirectory, "OpenNLP", "Models");
            NameModels = new string[] { "date", "location", "money", "organization", "percentage", "person", "time" };
        }

        static EnglishMaximumEntropySentenceDetector _sentenceDetector = null;
        public static EnglishMaximumEntropySentenceDetector SentenceDetector
        {
            get
            {
                var englishSD = Path.Combine(MModelPath, "EnglishSD.nbin");
                if (_sentenceDetector == null)
                {
                    if (File.Exists(englishSD) == false)
                    {
                        throw new Exception("EnglishSD.nbin isn't exists.");
                    }
                    else
                    {
                        try
                        {
                            _sentenceDetector = new EnglishMaximumEntropySentenceDetector(englishSD);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Error in Splitting Text to Sentences. Message:" + e.Message);

                        }
                    }
                }

                return _sentenceDetector;
            }
        }

        static EnglishNameFinder _englishNameFinder = null;
        public static EnglishNameFinder EnglishNameFinder
        {
            get
            {
                if (_englishNameFinder == null)
                {
                    var nlpModelPath = Path.Combine(Environment.CurrentDirectory, "OpenNLP", "NameFind");

                    _englishNameFinder = new EnglishNameFinder(nlpModelPath);
                }
                return _englishNameFinder;
            }
        }

        public static string FindNames(string transcript)
        {
            return EnglishNameFinder.GetNames(NameModels, transcript);
        }

        public static string[] DetectSentences(string transcript)
        {
            return SentenceDetector.SentenceDetect(transcript);
        }

        public static Dictionary<string, List<string>> DetectSentences(IEnumerable<string> paragraphs)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            foreach (var transcript in paragraphs)
            {
                result.Add(transcript, DetectSentences(transcript).ToList());
            }

            return result;
        }

        static EnglishMaximumEntropyTokenizer mTokenizer = null;
        public static string[] TokenizeSentence(string sentence)
        {
            if (mTokenizer == null)
            {
                mTokenizer = new OpenNLP.Tools.Tokenize.EnglishMaximumEntropyTokenizer(MModelPath + "EnglishTok.nbin");
            }

            return mTokenizer.Tokenize(sentence);
        }

        static EnglishMaximumEntropyPosTagger mPosTagger = null;
        public static string[] PosTagTokens(string[] tokens)
        {
            if (mPosTagger == null)
            {
                mPosTagger = new OpenNLP.Tools.PosTagger.EnglishMaximumEntropyPosTagger(MModelPath + "EnglishPOS.nbin", MModelPath + @"\Parser\tagdict");
            }

            return mPosTagger.Tag(tokens);
        }

        static EnglishTreebankChunker mChunker = null;
        public static string ChunkSentence(string[] tokens, string[] tags)
        {
            if (mChunker == null)
            {
                mChunker = new OpenNLP.Tools.Chunker.EnglishTreebankChunker(MModelPath + "EnglishChunk.nbin");
            }

            return mChunker.GetChunks(tokens, tags);
        }

        static EnglishTreebankParser mParser = null;
        public static OpenNLP.Tools.Parser.Parse ParseSentence(string sentence)
        {
            if (mParser == null)
            {
                mParser = new OpenNLP.Tools.Parser.EnglishTreebankParser(MModelPath, true, false);
            }

            return mParser.DoParse(sentence);
        }
    }
}
