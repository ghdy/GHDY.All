using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using GHDY.Core.Episode;
using GHDY.Workflow.Recognize.Interface;

namespace GHDY.Workflow.Recognize
{
    /// <summary>
    /// Activity based on NativeActivity<TResult>
    /// </summary>
    public sealed class PronunciationMarkupActivity : NativeActivity
    {
        // Define an activity input argument of Type String
        [RequiredArgument]
        public InArgument<LocalEpisode> LocalEpisode { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="context">WF context</param>
        /// <returns></returns>
        protected override void Execute(NativeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            var localEpisode = context.GetValue(this.LocalEpisode);

            var notifyPronunciationMarkup = context.GetExtension<INotifyPronunciationMarkup>();

            notifyPronunciationMarkup.LoadSyncDocument(localEpisode.SyncDocumentFilePath);

            // TODO : Code this activity
        }

        /// <summary>
        /// Register activity's metadata
        /// </summary>
        /// <param name="metadata"></param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            //metadata.RequireExtension<INotifyPronunciationMarkup>();

            // Register In arguments
            RuntimeArgument episodeArg = new RuntimeArgument("LocalEpisode", typeof(LocalEpisode), ArgumentDirection.In);
            metadata.AddArgument(episodeArg);
            metadata.Bind(this.LocalEpisode, episodeArg);

            // [Text] Argument must be set
            if (this.LocalEpisode == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[LocalEpisode] argument must be set!",
                        false,
                        "LocalEpisode"));
            }
        }
    }
}
