using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

namespace GHDY.Core.Episode
{
    public class LocalEpisodeProvider : IEpisodeProvider
    {
        EpisodeFileTypes[] _mustHaveFiles = new EpisodeFileTypes[] {EpisodeFileTypes.AudioFile };

        public string ID { get; set; }

        public string RootFolder { get; set; }

        #region IEpisodeProvider

        ObservableCollection<IEpisode> _episodeCollection = null;

        public ObservableCollection<IEpisode> EpisodeCollection
        {
            get
            {
                if (String.IsNullOrEmpty(this.RootFolder) == true)
                {
                    this.RootFolder = Path.Combine(Environment.CurrentDirectory, "Resources");
                    if (Directory.Exists(this.RootFolder) == false)
                        Directory.CreateDirectory(this.RootFolder);
                }

                List<string> episodeNameList = new List<string>();

                if (this._episodeCollection.Count <= 0)
                {
                    foreach (var fileType in this._mustHaveFiles)
                    {
                        var files = Directory.GetFiles(this.RootFolder, "*" + fileType.ToExt());
                        foreach (var file in files)
                        {
                            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
                            if (episodeNameList.Contains(fileNameWithoutExt) == false)
                            {
                                this._episodeCollection.Add(new LocalEpisode(file));
                                episodeNameList.Add(fileNameWithoutExt);
                            }
                        }
                    }
                }

                return _episodeCollection;
            }
        }

        #endregion

        public LocalEpisodeProvider()
        {
            this._episodeCollection = new ObservableCollection<IEpisode>();

            this._episodeCollection.CollectionChanged += EpisodeCollection_CollectionChanged;
        }

        public LocalEpisodeProvider(string rootFolder)
            : this()
        {
            this.RootFolder = rootFolder;
            this.ID = Path.GetFileName(this.RootFolder);
        }

        public LocalEpisodeProvider(string rootFolder, params EpisodeFileTypes[] mustHaveFiles)
            : this(rootFolder)
        {
            this.RootFolder = rootFolder;
            this.ID = Path.GetFileName(this.RootFolder);

            this._mustHaveFiles = mustHaveFiles;
        }

        void EpisodeCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
