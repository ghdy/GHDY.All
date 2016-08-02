using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using GHDY.Core.Episode;
using GHDY.Workflow.Recognize.Interface;
using GHDY.Core.DocumentModel;

namespace GHDY.Workflow.Recognize
{

    public sealed class ReferenceSoundMarkupActivity : NativeActivity
    {
        [RequiredArgument]
        public InArgument<LocalEpisode> LocalEpisode { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            var notifyRefSoundMarkup = context.GetExtension<INotifyReferenceSoundMarkup>();
            var localEpisode = context.GetValue(this.LocalEpisode);

            notifyRefSoundMarkup.NotifySyncDocument(localEpisode.SyncDocumentFilePath);

            if (localEpisode.Lrc != null)
            {
                notifyRefSoundMarkup.NotifyLyrics(localEpisode.SubtitleFilePath);
            }
            else
            {
                notifyRefSoundMarkup.NotifyDictation(localEpisode.DictationDocumentFilePath);
            }
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            metadata.RequireExtension<INotifyReferenceSoundMarkup>();

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
