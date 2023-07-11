using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Markup;
using System.Xml.Linq;
using GHDY.Core.DocumentModel;


namespace GHDY.Core.Episode
{
    public class LocalEpisode : IEpisode
    {
        #region ITaskEpisode

        public string ID
        {
            get;
            set;
        }

        [XmlIgnore]
        public string AudioFilePath
        {
            get { return GetFilePath(EpisodeFileTypes.AudioFile); }
        }

        [XmlIgnore]
        public string SubtitleFilePath
        {
            get { return this.GetFilePath(EpisodeFileTypes.SubtitleFile); }
        }

        [XmlIgnore]
        public string SyncDocumentFilePath
        {
            get { return this.GetFilePath(EpisodeFileTypes.RecognitionFile); }
        }

        [XmlIgnore]
        public string DictationDocumentFilePath
        {
            get { return this.GetFilePath(EpisodeFileTypes.DictationFile); }
        }

        [XmlIgnore]
        public string WaveFilePath
        {
            get { return GetFilePath(EpisodeFileTypes.WaveFile); }
        }

        [NonSerialized]
        DMDocument _syncDocument = null;

        [XmlIgnore]
        public DMDocument SyncDocument
        {
            get
            {
                if (this._syncDocument == null)
                {
                    var filePath = this.SyncDocumentFilePath;
                    if (File.Exists(filePath) == true)
                        return DMDocument.Load(filePath);
                    else
                        return null;
                }
                else
                    return this._syncDocument;
            }
            set
            {
                if (value == null)
                {
                    this._syncDocument = null;
                    File.Delete(this.SyncDocumentFilePath);
                }
                else if (value.Sentences.Count() > 0)
                {
                    this._syncDocument = value;
                    SaveDocument(value, this.SyncDocumentFilePath);
                }
            }
        }

        private void SaveDocument(DMDocument doc, string filepath)
        {
            if (doc != null)
            {
                doc.Save(filepath);
            }
        }

        [NonSerialized]
        DMDocument _dictationDocument = null;

        [XmlIgnore]
        public DMDocument DictationDocument
        {
            get
            {
                if (this._dictationDocument == null)
                {
                    var filePath = this.DictationDocumentFilePath;
                    if (File.Exists(filePath) == true)
                        return DMDocument.Load(filePath);
                    else
                        return null;
                }
                else
                    return this._dictationDocument;
            }
            set
            {
                if (value == null)
                {
                    this._dictationDocument = null;
                    File.Delete(this.DictationDocumentFilePath);
                }
                else
                {
                    this._dictationDocument = value;

                    this.SaveDocument(this.DictationDocument, this.DictationDocumentFilePath);
                }
            }
        }

        [NonSerialized]
        EpisodeContent _content = null;

        [XmlIgnore]
        public EpisodeContent Content
        {
            get
            {
                var filePath = this.GetFilePath(EpisodeFileTypes.ContentFile);
                if (this._content == null)
                {
                    if (File.Exists(filePath) == true)
                    {
                        return new EpisodeContent(XElement.Load(filePath));
                    }
                }

                return this._content;
            }
            set
            {
                this._content = value;

                var filePath = this.GetFilePath(EpisodeFileTypes.ContentFile);
                value.Save(filePath, SaveOptions.None);
            }
        }

        [NonSerialized]
        Lyrics _lrc = null;

        [XmlIgnore]
        public Lyrics Lrc
        {
            get
            {
                if (this._lrc == null)
                {
                    string lrcFile = this.SubtitleFilePath;

                    if (File.Exists(lrcFile) == true)
                        this._lrc = new Lyrics(lrcFile,false);
                }
                return this._lrc;
            }
            set
            {
                this._lrc = value;

                string lrcFile = this.SubtitleFilePath;
                File.WriteAllText(lrcFile, this._lrc.ToString());
            }
        }

        #endregion

        public LocalEpisode()
        {

        }

        public LocalEpisode(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            var folder = filePath;

            if (string.IsNullOrEmpty(extension) == false)
                folder = Path.GetDirectoryName(filePath);

            this.ID = fileNameWithoutExt;
            this.CurrentFolder = folder;
        }

        public LocalEpisode(string id, string album, string targetFolder)
        {
            this.ID = id;
            this.CurrentFolder = Path.Combine(targetFolder, album, id);
        }

        [XmlIgnore]
        public string CurrentFolder { get; set; }

        private string GetFilePath(EpisodeFileTypes extType)
        {
            if (String.IsNullOrEmpty(this.CurrentFolder) || Directory.Exists(this.CurrentFolder) == false)
                throw new Exception("Episode's CurrentFolder is null or not exists.");
            return Path.Combine(this.CurrentFolder, this.ID + extType.ToExt());
        }

        public void ReloadLyrics()
        {
            this._lrc = null;
        }

        public void ReloadSyncDocument()
        {
            this._syncDocument = null;
        }

        public void ReloadDictationDocument()
        {
            this._dictationDocument = null;
        }
    }
}
