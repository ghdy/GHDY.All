using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;
using System.IO;
using System.Windows;
using System.ComponentModel.Composition;

namespace GHDY.Core.AudioPlayer
{
    public class MediaAudioPlayer : AudioPlayerBase
    {
        MediaPlayer _player = new MediaPlayer();
        Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        double stopSeconds = 0;

        public MediaAudioPlayer()
            : base()
        {
            //Load(mediaSource);
            this.PositionChanged += new EventHandler<PositionChangedEventArgs>(MediaAudioPlayer_PositionChanged);
        }

        void MediaAudioPlayer_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            if (e.CurrentPosition.TotalSeconds >= stopSeconds)
            {
                this.Pause();
            }
        }

        protected override void DoLoad(string mediaSource)
        {
            try
            {
                Uri mediaUri = new Uri(mediaSource, UriKind.RelativeOrAbsolute);

                if (!mediaUri.IsAbsoluteUri || mediaUri.IsFile)
                    mediaUri = new Uri(new FileInfo(mediaSource).FullName, UriKind.Absolute);

                _dispatcher.Invoke(DispatcherPriority.Send, new Action(
                    delegate { _player.Open(mediaUri); }));
            }
            catch (Exception ex)
            {
                throw ex.InnerException == null ? ex : ex.InnerException;
            }
        }

        protected override void DoPlay()
        {
            this.stopSeconds = double.MaxValue;
            _dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate
            {
                _player.Play();
            }));
        }

        protected override TimeSpan GetLength()
        {
            var duration = (Duration)_dispatcher.Invoke(DispatcherPriority.Send, new Func<Duration>(delegate
            {
                return _player.NaturalDuration;
            }));

            return duration.HasTimeSpan ? duration.TimeSpan : TimeSpan.Zero;
        }

        protected override TimeSpan DoGetPosition()
        {
            return (TimeSpan)_dispatcher.Invoke(DispatcherPriority.Send, new Func<TimeSpan>(delegate
            {
                return _player.Position;
            }));
        }

        protected override void DoPause()
        {
            _dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate
            {
                _player.Pause(); Console.Beep(37, 1);
            }));
        }

        protected override void DoSeek(TimeSpan time)
        {
            _dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate
            {
                _player.Pause(); Console.Beep(37, 1);
                _player.Position = time;
            }));

            if (IsPlaying) DoPlay();
        }

        protected override void DoStop()
        {
            _dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate
            {
                _player.Stop();
            }));
        }

        protected override void DoPlayRange(TimeSpan beginTime, TimeSpan endTime)
        {
            this.Play(beginTime);
            this.stopSeconds = endTime.TotalSeconds;
        }

        public override void Dispose()
        {
            DoStop();
            this._player.Close();
            base.Dispose();
        }
    }
}
