using System;
using System.ComponentModel.Composition;
using System.Windows.Threading;

namespace GHDY.Core.AudioPlayer
{
    [InheritedExport(typeof(AudioPlayerBase))]
    public abstract class AudioPlayerBase : IDisposable
    {
        public event EventHandler<PositionChangedEventArgs> PositionChanged = null;

        private readonly DispatcherTimer Timer = null;

        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get
            {
                return this._isPlaying;
            }
            private set
            {
                this._isPlaying = value;
                this.Timer.IsEnabled = this._isPlaying;
            }
        }

        public TimeSpan Length { get { return GetLength(); } }

        public TimeSpan Position
        {
            get { return DoGetPosition(); }
            set { DoSeek(value); }
        }

        public AudioPlayerBase()
        {
            this.Timer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromMilliseconds(10) };
            this.Timer.Tick += new EventHandler(Timer_Tick);
            this.Timer.IsEnabled = false;
            this.Timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (this.IsPlaying == true && this.PositionChanged != null)
                this.PositionChanged(this, new PositionChangedEventArgs(this.Position));
        }

        protected abstract TimeSpan DoGetPosition();
        protected abstract TimeSpan GetLength();

        protected abstract void DoLoad(string mediaSource);
        protected abstract void DoPlay();
        protected abstract void DoPlayRange(TimeSpan beginTime, TimeSpan endTime);
        protected abstract void DoSeek(TimeSpan time);
        protected abstract void DoPause();
        protected abstract void DoStop();

        public void Load(Uri mediaSource)
        {
            if (mediaSource.IsFile)
                Load(mediaSource.OriginalString);
            else
                Load(mediaSource.ToString());
        }

        public void Load(string mediaSource)
        {
            DoLoad(mediaSource);
            this.IsPlaying = false;
        }

        public void Play()
        {
            DoPlay();
            this.IsPlaying = true;
        }

        public void Play(TimeSpan time)
        {
            Seek(time);
            Play();
        }

        public void PlayRange(TimeSpan beginTime, TimeSpan endTime)
        {
            DoPlayRange(beginTime, endTime);
            this.IsPlaying = true;
        }

        public void Seek(TimeSpan time)
        {
            DoSeek(time);

            if (this.IsPlaying) Play();

        }

        public void Pause()
        {
            DoPause();
            this.IsPlaying = false;
        }

        public void Stop()
        {
            DoStop();
            this.IsPlaying = false;
        }

        public virtual void Dispose()
        {
            
        }
    }


    public class PositionChangedEventArgs : EventArgs
    {
        public TimeSpan CurrentPosition { get; private set; }

        public PositionChangedEventArgs(TimeSpan position)
            : base()
        {
            this.CurrentPosition = position;
        }
    }
}
