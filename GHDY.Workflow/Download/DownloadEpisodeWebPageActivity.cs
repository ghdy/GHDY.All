using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using System.IO;
using GHDY.Core.LearningContentProviderCore;

namespace GHDY.Workflow.Download
{
    /// <summary>
    /// Activity based on CodeActivity
    /// </summary>
    public sealed class DownloadEpisodeWebPageActivity : CodeActivity
    {
        // Define an activity input argument of type string
        [RequiredArgument]
        public InArgument<XEpisode> CurrentEpisode { get; set; }

        [RequiredArgument]
        public InArgument<BaseTarget> CurrentTarget { get; set; }

        [RequiredArgument]
        public InArgument<bool> ReDownload { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="context">WF context</param>
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            var episode = context.GetValue<XEpisode>(this.CurrentEpisode);
            var target = context.GetValue<BaseTarget>(this.CurrentTarget);
            bool reDownload = context.GetValue<bool>(this.ReDownload);
            // TODO : Code this activity

            var filePath = target.GetDownloadEpisodeWebPageFilePath(episode.ID, episode.AlbumID);

            if (reDownload == true || File.Exists(filePath) == false)
                target.DownloadEpisode(episode);
            else
                target.CheckEpisode(episode);

            target.SaveContent(episode.ID, episode.AlbumID);
        }

        /// <summary>
        /// Register activity's metadata
        /// </summary>
        /// <param name="metadata"></param>
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            // Register In arguments
            RuntimeArgument episodeArg = new RuntimeArgument("CurrentEpisode", typeof(XEpisode), ArgumentDirection.In);
            metadata.AddArgument(episodeArg);
            metadata.Bind(this.CurrentEpisode, episodeArg);

            RuntimeArgument targetArg = new RuntimeArgument("CurrentTarget", typeof(BaseTarget), ArgumentDirection.In);
            metadata.AddArgument(targetArg);
            metadata.Bind(this.CurrentTarget, targetArg);

            RuntimeArgument reDownloadArg = new RuntimeArgument("ReDownload", typeof(bool), ArgumentDirection.In);
            metadata.AddArgument(reDownloadArg);
            metadata.Bind(this.ReDownload, reDownloadArg);

            // [CurrentEpisode] Argument must be set

            // TODO : Add arguments ... etc ...
            if (this.CurrentEpisode == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[CurrentEpisode] argument must be set!",
                        false,
                        "CurrentEpisode"));
            }

            if (this.CurrentTarget == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[CurrentTarget] argument must be set!",
                        false,
                        "CurrentTarget"));
            }

            if (this.ReDownload == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[ReDownload] argument must be set!",
                        false,
                        "ReDownload"));
            }
        }
    }
}
