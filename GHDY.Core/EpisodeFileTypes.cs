using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GHDY.Core
{
    public enum EpisodeFileTypes
    {
        AudioFile = 0,
        ContentFile = 1,
        DictationFile = 2,
        RecognitionFile = 3,
        SubtitleFile = 4,
        TitleFile = 5,
        TranslationFile = 6,
        WaveFile = 7,
        HtmlFile = 8
    }

    public static class EpisodeFileExtsExtends
    {
        public static string ToExt(this EpisodeFileTypes fileType)
        {
            string result = "null";
            switch (fileType)
            {
                case EpisodeFileTypes.AudioFile:
                    result = ".mp3";
                    break;
                case EpisodeFileTypes.ContentFile:
                    result = ".content";
                    break;
                case EpisodeFileTypes.DictationFile:
                    result = ".dictation";
                    break;
                case EpisodeFileTypes.RecognitionFile:
                    result = ".recognition";
                    break;
                case EpisodeFileTypes.SubtitleFile:
                    result = ".lrc";
                    break;
                case EpisodeFileTypes.TitleFile:
                    result = ".title";
                    break;
                case EpisodeFileTypes.TranslationFile:
                    result = ".translation";
                    break;
                case EpisodeFileTypes.WaveFile:
                    result = ".wav";
                    break;
                case EpisodeFileTypes.HtmlFile:
                    result = ".html";
                    break;
            }

            return result;
        }

        public static string ToFileName(this EpisodeFileTypes fileType, string fileNameWithourExt)
        {
            return fileNameWithourExt.Trim() + ToExt(fileType);
        }
    }
}
