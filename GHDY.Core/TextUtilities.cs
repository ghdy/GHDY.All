using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GHDY.Core
{
    public static class TextUtilities
    {
        public static bool IsMatch(string text, string pattern)
        {
            Regex regex = new Regex(pattern);

            return regex.IsMatch(text);
        }


        public static Match Match(string text, string pattern)
        {
            Regex regex = new Regex(pattern);

            return regex.Match(text);
        }

        public static IEnumerable<Match> Matches(string text, string pattern)
        {
            Regex regex = new Regex(pattern);

            return regex.Matches(text).OfType<Match>();
        }

        public static bool IsSpecialPronunciation(this string text, bool isFirstWordInSentence)
        {
            var array = text.ToArray();
            int number = 0, up = 0, low = 0, p = 0;

            foreach (var c in array)
            {
                switch (c.GetCharType())
                {
                    case CharTypes.number:
                        number += 1;
                        break;
                    case CharTypes.up:
                        up += 1;
                        break;
                    case CharTypes.low:
                        low += 1;
                        break;
                    case CharTypes.p:
                        p += 1;
                        break;
                }
            }

            if (up + low + number < 1)
                return false;

            int defaultUpCount = 1;
            if (isFirstWordInSentence == true)
                defaultUpCount = 2;

            if (number > 0 || p > 0 || up > defaultUpCount)
                return true;

            return false;
        }

        public enum CharTypes { space, number, up, low, p }

        public static CharTypes GetCharType(this char c)
        {
            var spaceArray = new List<char>() { ' ', '\'', '\"', ':' };

            if (c >= 'a' && c <= 'z')
                return CharTypes.low;
            else if (c >= 'A' && c <= 'Z')
                return CharTypes.up;
            else if (c >= '0' && c <= '9')
                return CharTypes.number;
            else if (spaceArray.Contains(c) == true)
                return CharTypes.space;
            else
                return CharTypes.p;
        }
    }
}
