using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DifferenceEngine;

namespace GHDY.Core
{
    public class DiffResult
    {
        public int Add { get; set; }

        public int Del { get; set; }

        public int Replace { get; set; }

        public int Same { get; set; }

        //public double Diff
        //{
        //    get
        //    {
        //        return ((double)(this.Del + this.Replace + this.Add)) / (double)Same;
        //    }
        //}

        public DiffResult(int addCount, int deleteCount, int replaceCount, int sameCount)
        {
            this.Add = addCount;
            this.Del = deleteCount;
            this.Replace = replaceCount;
            this.Same = sameCount;
        }
    }

    public class DiffHelper
    {
        //public static double StringSimilar(string sourceString, string checkString)
        public static DiffResult matchString(string sourceString, string checkString)
        {
            DiffEngine diffEngine = new DiffEngine();
            DiffList_CharData source = new DiffList_CharData(sourceString);
            DiffList_CharData dest = new DiffList_CharData(checkString);
            var time = diffEngine.ProcessDiff(source, dest);
            var report = diffEngine.DiffReport();

            string result = "";
            int addCount = 0;
            int deleteCount = 0;
            int replaceCount = 0;
            int sameCount = 0;

            foreach (DiffResultSpan diffResultSpan in report)
            {
                switch (diffResultSpan.Status)
                {
                    case DiffResultSpanStatus.AddDestination:
                        {
                            addCount += diffResultSpan.Length;
                            break;
                        }
                    case DiffResultSpanStatus.DeleteSource:
                        {

                            deleteCount += diffResultSpan.Length;
                            break;
                        }
                    case DiffResultSpanStatus.Replace:
                        {
                            for (int i = 0; i < diffResultSpan.Length; i++)
                            {
                                result += dest.GetByIndex(diffResultSpan.DestIndex + i);
                            }

                            replaceCount += diffResultSpan.Length;
                            break;
                        }
                    case DiffResultSpanStatus.NoChange:
                        {
                            for (int i = 0; i < diffResultSpan.Length; i++)
                            {
                                result += dest.GetByIndex(diffResultSpan.DestIndex + i);
                            }

                            sameCount += diffResultSpan.Length;
                            break;
                        }
                }
            }
            return new DiffResult(addCount, deleteCount, replaceCount, sameCount);//(deleteCount) + "|" + result;
        }


    }
}
