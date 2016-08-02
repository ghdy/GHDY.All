using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using GHDY.Workflow.Recognize.Interface;
using GHDY.Core.Episode;
using GHDY.Core.DocumentModel;

namespace GHDY.Workflow.Recognize
{
    /// <summary>
    /// Activity based on CodeActivity
    /// </summary>
    public sealed class MergeRefSentence2DocumentActivity : CodeActivity
    {
        // Define an activity input argument of type string
        [RequiredArgument]
        public InArgument<IEnumerable<RefSentence>> RefSentences { get; set; }

        [RequiredArgument]
        public InArgument<LocalEpisode> LocalEpisode { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="context">WF context</param>
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            IEnumerable<RefSentence> sentences = context.GetValue(this.RefSentences);
            LocalEpisode localEpisode = context.GetValue(this.LocalEpisode);

            // TODO : Code this activity
            var dmDoc = DMDocument.Load(localEpisode.SyncDocumentFilePath);

            foreach (var refSentence in sentences)
            {
                var dmSentence = dmDoc.Sentences.Single((sentence) => {
                    if (sentence.Index == refSentence.Index)
                        return true;
                    else return false;
                });

                dmSentence.BeginTime = refSentence.Begin;
                dmSentence.EndTime = refSentence.End;
            }

            dmDoc.Save(localEpisode.SyncDocumentFilePath);

            localEpisode.ReloadSyncDocument();
        }

        /// <summary>
        /// Register activity's metadata
        /// </summary>
        /// <param name="metadata"></param>
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            // Register In arguments
            RuntimeArgument refSentencesArg = new RuntimeArgument("RefSentences", typeof(IEnumerable<RefSentence>), ArgumentDirection.In);
            metadata.AddArgument(refSentencesArg);
            metadata.Bind(this.RefSentences, refSentencesArg);

            // [Text] Argument must be set
            if (this.RefSentences == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[RefSentences] argument must be set!",
                        false,
                        "RefSentences"));
            }

            RuntimeArgument localEpisodeArg = new RuntimeArgument("LocalEpisode", typeof(LocalEpisode), ArgumentDirection.In);
            metadata.AddArgument(localEpisodeArg);
            metadata.Bind(this.LocalEpisode, localEpisodeArg);

            // [Text] Argument must be set
            if (this.LocalEpisode == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[LocalEpisode] argument must be set!",
                        false,
                        "LocalEpisode"));
            }

            // TODO : Add arguments ... etc ...
        }
    }
}
