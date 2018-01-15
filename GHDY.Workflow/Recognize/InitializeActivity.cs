using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using GHDY.Core.LearningContentProviderCore;
using GHDY.Core.Episode;
using GHDY.Workflow.Recognize.Interface;
using System.IO;

namespace GHDY.Workflow.Recognize
{
    /// <summary>
    /// Activity based on NativeActivity<TResult>
    /// </summary>
    public sealed class InitializeActivity : NativeActivity<LocalEpisode>
    {
        // Define an activity input argument of Type String
        [RequiredArgument]
        public InArgument<XEpisode> Episode { get; set; }

        [RequiredArgument]
        public InArgument<BaseTarget> Target { get; set; }

        private INotifyInitialize _notifyInitialize;

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="context">WF context</param>
        /// <returns></returns>
        protected override void Execute(NativeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            var episode = context.GetValue(this.Episode);
            var target = context.GetValue(this.Target);

            // TODO : Code this activity
            var episodeID = episode.ID;
            var albumID = episode.AlbumID;

            this._notifyInitialize = context.GetExtension<INotifyInitialize>();
            this._notifyInitialize.NotifyMessage("Start:");

            var episodeContentFolder = target.GetDownloadEpisodeContentFolderPath(episodeID, albumID);
            var localEpisode = new LocalEpisode(episodeContentFolder);

            if (localEpisode.Content == null)
            {
                this._notifyInitialize.NotifyMessage("1.EpisodeContent [not] Found.");
                var content = target.Reader.GetEpisodeContent(episodeID, albumID);
                if (content != null)
                {
                    localEpisode.Content = content;
                    this._notifyInitialize.NotifyMessage(">>Create EpisodeContent [Completed].");
                }
                else
                    this._notifyInitialize.NotifyMessage(">>Create EpisodeContent [Fail].");

            }
            else
                this._notifyInitialize.NotifyMessage("1.EpisodeContent is [OK].");

            if (File.Exists(localEpisode.AudioFilePath))
                this._notifyInitialize.NotifyMessage("2.Audio is [OK].");
            else
                this._notifyInitialize.NotifyMessage("2.Audio [not] Found.");

            this._notifyInitialize.NotifyEpisode(localEpisode);
            // Return value
            this.Result.Set(context, localEpisode);
        }

        /// <summary>
        /// Register activity's metadata
        /// </summary>
        /// <param name="metadata"></param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            metadata.RequireExtension<INotifyInitialize>();

            // Register In arguments
            var episodeArg = new RuntimeArgument(nameof(Episode), typeof(XEpisode), ArgumentDirection.In);
            metadata.AddArgument(episodeArg);
            metadata.Bind(this.Episode, episodeArg);

            // [Episode] Argument must be set
            if (this.Episode == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[Episode] argument must be set!",
                        false,
                        nameof(Episode)));
            }

            var targetArg = new RuntimeArgument(nameof(Target), typeof(BaseTarget), ArgumentDirection.In);
            metadata.AddArgument(targetArg);
            metadata.Bind(this.Target, targetArg);

            // [Target] Argument must be set
            if (this.Target == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[Target] argument must be set!",
                        false,
                        nameof(Target)));
            }


            // Register Out arguments
            var resultArg = new RuntimeArgument("Result", typeof(LocalEpisode), ArgumentDirection.Out);
            metadata.AddArgument(resultArg);
            metadata.Bind(this.Result, resultArg);

            // [Result] Argument must be set
            if (this.Result == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[Result] argument must be set!",
                        false,
                        "Result"));
            }

            // TODO : Add arguments ... etc ...
        }
    }
}
