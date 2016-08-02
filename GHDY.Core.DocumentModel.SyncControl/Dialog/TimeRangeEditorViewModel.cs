using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GHDY.Core.DocumentModel.SyncControl.Dialog
{
    public class TimeRangeChangedEventArgs : EventArgs
    {
        public TimeSpan Begin { get; set; }
        public TimeSpan End { get; set; }

        public bool IsBeginChanged { get; set; }
        public bool IsEndChanged { get; set; }

        public TimeRangeChangedEventArgs(TimeSpan begin, TimeSpan end)
        {
            this.Begin = begin;
            this.End = end;

            this.IsBeginChanged = true;
            this.IsEndChanged = true;
        }
    }

    public class TimeRangeEditorViewModel : INotifyPropertyChanged, IDisposable
    {
        List<CommandBinding> _commandBindings = new List<CommandBinding>();
        List<KeyBinding> _keyBindings = new List<KeyBinding>();

        double _min = 0;
        public double Minimum
        {
            get { return this._min; }
            set
            {
                this._min = value;
                this.NotifyPropertyChanged("Minimum");
            }
        }

        double _max = 0;
        public double Maximum
        {
            get { return this._max; }
            set
            {
                this._max = value;
                this.NotifyPropertyChanged("Maximum");
            }
        }

        double _begin = 0;
        public double Begin
        {
            get { return this._begin; }
            set
            {
                this._begin = value;
                this.NotifyPropertyChanged("Begin");
                bool isBeginChanged = true;
                bool isEndChanged = false;
                NotifyTimeRangeChanged(isBeginChanged, isEndChanged);
            }
        }

        double _end = 0;
        public double End
        {
            get { return this._end; }
            set
            {
                this._end = value;
                this.NotifyPropertyChanged("End");
                bool isBeginChanged = false;
                bool isEndChanged = true;
                NotifyTimeRangeChanged(isBeginChanged, isEndChanged);
            }
        }

        double _step = 0.1;
        public double Step
        {
            get { return this._step; }
            set
            {
                this._step = value;
                this.NotifyPropertyChanged("Step");
            }
        }

        public FrameworkElement View { get; private set; }

        public event EventHandler<TimeRangeChangedEventArgs> TimeRangeChanged;

        public TimeRangeEditorViewModel()
        {

         }

        public void Init(FrameworkElement element)
        {
            this.View = element;

            BindingKey(this.CmdChangeBegin, Key.F9, ModifierKeys.None, "Left");
            BindingKey(this.CmdChangeBegin, Key.F10, ModifierKeys.None, "Right");
            BindingKey(this.CmdChangeEnd, Key.F11, ModifierKeys.None, "Left");
            BindingKey(this.CmdChangeEnd, Key.F12, ModifierKeys.None, "Right");

            BindingKey(this.CmdChangeBegin, Key.F9, ModifierKeys.Shift, "Left");
            BindingKey(this.CmdChangeBegin, Key.F10, ModifierKeys.Shift, "Right");
            BindingKey(this.CmdChangeEnd, Key.F11, ModifierKeys.Shift, "Left");
            BindingKey(this.CmdChangeEnd, Key.F12, ModifierKeys.Shift, "Right");

            this.View.CommandBindings.Add(new CommandBinding(this.CmdChangeBegin, this.CmdChangeBegin_Execute));
            this.View.CommandBindings.Add(new CommandBinding(this.CmdChangeEnd, this.CmdChangeEnd_Execute));
        }

        private void NotifyTimeRangeChanged(bool isBeginChanged, bool isEndChanged)
        {
            if (this.TimeRangeChanged != null)
            {
                var args = new TimeRangeChangedEventArgs(TimeSpan.FromSeconds(this.Begin), TimeSpan.FromSeconds(this.End))
                {
                    IsBeginChanged = isBeginChanged,
                    IsEndChanged = isEndChanged
                };

                this.TimeRangeChanged(this, args);
            }
        }


        #region Command

        RoutedUICommand _cmdChangeBegin = new RoutedUICommand();
        public RoutedUICommand CmdChangeBegin { get { return this._cmdChangeBegin; } }

        public void CmdChangeBegin_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter.ToString() == "Left")
            {
                this.Begin -= this.Step;
            }
            else if (e.Parameter.ToString() == "Right")
            {
                this.Begin += this.Step;
            }
        }

        RoutedUICommand _cmdChangeEnd = new RoutedUICommand();
        public RoutedUICommand CmdChangeEnd { get { return this._cmdChangeEnd; } }

        private void CmdChangeEnd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter.ToString() == "Left")
            {
                this.End -= this.Step;
            }
            else if (e.Parameter.ToString() == "Right")
            {
                this.End += this.Step;
            }
        }

        #endregion

        private void BindingKey(ICommand cmd, Key key, ModifierKeys mKey, string para)
        {
            KeyGesture keyG = null;
            if (mKey == ModifierKeys.None)
                keyG = new KeyGesture(key);
            else
                keyG = new KeyGesture(key, mKey);

            var keyB = new KeyBinding(cmd, keyG) { CommandParameter = para };
            this.View.InputBindings.Add(keyB);
            this._keyBindings.Add(keyB);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public void Dispose()
        {
            foreach (var kb in this._keyBindings)
            {
                this.View.InputBindings.Remove(kb);
            }

            foreach (var cb in this._commandBindings)
            {
                this.View.CommandBindings.Remove(cb);
            }
        }
    }
}
