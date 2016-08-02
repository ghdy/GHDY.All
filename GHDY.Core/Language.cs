using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Core
{
    public class Language
    {
        public string Name { get; set; }
        public string Culture { get; set; }

        public Language(string name, string culture)
        {
            this.Name = name;
            this.Culture = culture;
        }

        public static Language Parse(string culture)
        {
            var all = System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.AllCultures);
            foreach (var item in all)
            {
                
            }
            foreach (var l in Languages.All)
            {
                if (l.Culture == culture)
                    return l;
            }

            return new Language(culture, culture);
        }
    }

    public static class Languages
    {
        public static Language ChineseCN { get { return new Language("Chinese (PRC)", "zh-CN"); } }

        public static Language ChineseTW { get { return new Language("Chinese (Taiwan)", "zh-TW"); } }

        public static Language AmericanEnglish { get { return new Language("English (US)", "en-US"); } }

        public static Language Japanese { get { return new Language("Japanese (Japan)", "ja-JP"); } }

        public static IEnumerable<Language> All
        {
            get
            {
                yield return Languages.ChineseCN;
                yield return Languages.ChineseTW;
                yield return Languages.AmericanEnglish;
                yield return Languages.Japanese;
            }
        }
    }
}
