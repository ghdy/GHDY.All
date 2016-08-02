using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Composition;
using System.Threading;
using GHDY.Core.AudioPlayer.Interops.DirectShow;

namespace GHDY.Core.AudioPlayer
{
    public class DShowAudioPlayer : AudioPlayerBase
    {
        private IGraphBuilder builder;
        private IMediaControl controller;
        private IMediaPosition seeker;
        double stopSeconds = 0;

        public DShowAudioPlayer()
            : base()
        {
            this.PositionChanged += new EventHandler<PositionChangedEventArgs>(DShowAudioPlayer_PositionChanged);
        }

        void DShowAudioPlayer_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            if (e.CurrentPosition.TotalSeconds >= stopSeconds)
            {
                this.Pause();
                seeker.StopTime = seeker.Duration;
            }
        }

        public DShowAudioPlayer(string mediaSource)
            : this()
        {
            Load(mediaSource);
        }

        protected override void DoLoad(string mediaSource)
        {
            builder = new FilterGraph() as IGraphBuilder;
            builder.RenderFile(mediaSource, null);

            controller = builder as IMediaControl;
            seeker = builder as IMediaPosition;
        }

        protected override TimeSpan GetLength()
        {
            return TimeSpan.FromSeconds(seeker.Duration);
        }

        //[Export("GetPlayingTime")]
        protected override TimeSpan DoGetPosition()
        {
            return TimeSpan.FromSeconds(seeker.CurrentPosition);
        }

        //[Export("PausePlaying")]
        protected override void DoPause()
        {
            controller.Pause();
        }

        //[Export("ResumePlaying")]
        protected override void DoPlay()
        {
            seeker.StopTime = seeker.Duration;
            stopSeconds = seeker.Duration;
            controller.Run();
        }

        protected override void DoPlayRange(TimeSpan beginTime, TimeSpan endTime)
        {
            if (controller != null && seeker != null)
            {
                seeker.CurrentPosition = beginTime.TotalSeconds;
                if (endTime > this.Length)
                    seeker.StopTime = this.Length.TotalSeconds;
                else
                    seeker.StopTime = endTime.TotalSeconds;

                stopSeconds = seeker.StopTime;

                controller.Run();

                //IMediaEvent me = controller as IMediaEvent;
                //try
                //{
                //    me.WaitForCompletion((int)(endTime - beginTime).TotalMilliseconds);
                //}
                //catch
                //{
                //}
                //finally
                //{
                //    seeker.StopTime = seeker.Duration;
                //}
            }
        }

        //[Export("SetPlayingTime")]
        protected override void DoSeek(TimeSpan time)
        {
            seeker.CurrentPosition = time.TotalSeconds;
        }

        protected override void DoStop()
        {
            controller.Stop();
        }

        public override void Dispose()
        {
            DoStop();

            builder = null;
            controller = null;
            seeker = null;

            GC.Collect();
        }
    }
}
