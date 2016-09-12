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


    }
}
