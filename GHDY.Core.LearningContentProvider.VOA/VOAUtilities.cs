using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GHDY.Core.LearningContentProvider.VOA;

namespace LanguageResourceProvider.VOA
{

    public static class VOAUtilities
    {
        public static Dictionary<string, int> AlbumWeekDateDict { get; set; }

        static VOAUtilities()
        {
            AlbumWeekDateDict = new Dictionary<string, int>();
            AlbumWeekDateDict.Add("TechnologyReport", 1);
            AlbumWeekDateDict.Add("DevelopmentReport", 1);
            AlbumWeekDateDict.Add("AgricultureReport", 2);
            AlbumWeekDateDict.Add("HealthReport", 3);
            AlbumWeekDateDict.Add("EducationReport", 4);
            AlbumWeekDateDict.Add("EconomicsReport", 5);
            AlbumWeekDateDict.Add("InTheNews", 6);
            AlbumWeekDateDict.Add("WordsAndTheirStories", 0);
        }

        public static bool ChackEpisodeDate(string album, int year, int month, int day, out DateTime realDate)
        {
            DateTime date = new DateTime(year, month, day);
            realDate = date;

            if (AlbumWeekDateDict.ContainsKey(album) == false)
                return true;

            int realDayInWeek = AlbumWeekDateDict[album];
            int currentDayInWeek = Convert.ToInt32(date.DayOfWeek);
            if (currentDayInWeek == realDayInWeek)
            {
                return true;
            }
            else
            {
                for (int i = -1; i > -7; i--)
                {
                    var temp = date.AddDays(i);
                    var dayInWeek = Convert.ToInt32(temp.DayOfWeek);
                    if (dayInWeek == realDayInWeek)
                    {
                        realDate = temp;
                        return false;
                    }
                }
                throw new Exception("Date has error!!!");
            }
        }

        public static int weekNumber(DateTime date)
        {
            DateTime firstDay = new DateTime(date.Year, 1, 1);
            int theday;

            if (firstDay.DayOfWeek == DayOfWeek.Sunday || firstDay.DayOfWeek == DayOfWeek.Monday)
            {
                theday = 0;
            }
            else if (firstDay.DayOfWeek == DayOfWeek.Tuesday)
            {
                theday = 1;
            }
            else if (firstDay.DayOfWeek == DayOfWeek.Wednesday)
            {
                theday = 2;
            }
            else if (firstDay.DayOfWeek == DayOfWeek.Thursday)
            {
                theday = 3;
            }
            else if (firstDay.DayOfWeek == DayOfWeek.Friday)
            {
                theday = 4;
            }
            else
            {
                theday = 5;
            }

            int weekNum = (date.DayOfYear + theday) / 7 + 1;
            return weekNum;
        }
    }
}
