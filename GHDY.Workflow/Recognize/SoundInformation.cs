using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Workflow.Recognize
{
    public enum SoundType { Male, Female, Music, Other }
    public class SoundInformation : INotifyPropertyChanged
    {
        private SoundType _type;
        public SoundType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                this.NotifyPropertyChanged("Type");
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                this.NotifyPropertyChanged("Name");
            }
        }

        private bool _quote;
        public bool Quote
        {
            get { return _quote; }
            set
            {
                _quote = value;
                this.NotifyPropertyChanged("Quote");
            }
        }

        public SoundInformation(SoundType type)
        {
            this.Type = type;

        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
