using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using GHDY.Core.DocumentModel;
using GHDY.Core.Episode;

namespace GHDY.Workflow.Recognize
{
    /// <summary>
    /// Activity based on CodeActivity
    /// </summary>
    public sealed class ReloadDMDocumentActivity : CodeActivity
    {
        // Define an activity input argument of type string
        [RequiredArgument]
        public InArgument<LocalEpisode> Episode { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="context">WF context</param>
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            var episode = context.GetValue(this.Episode);

            // TODO : Code this activity
            episode.SyncDocument = DMDocument.Load(episode.SyncDocumentFilePath);
            
        }

        /// <summary>
        /// Register activity's metadata
        /// </summary>
        /// <param name="metadata"></param>
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            // Register In arguments
            RuntimeArgument episodeArg = new RuntimeArgument("Episode", typeof(LocalEpisode), ArgumentDirection.In);
            metadata.AddArgument(episodeArg);
            metadata.Bind(this.Episode, episodeArg);

            // [Text] Argument must be set
            if (this.Episode == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[Episode] argument must be set!",
                        false,
                        "Episode"));
            }

            // TODO : Add arguments ... etc ...
        }
    }
}
