using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using GHDY.Core.DocumentModel;
using GHDY.Workflow.Recognize.Interface;
using GHDY.Core.Episode;
using System.Windows.Documents;

namespace GHDY.Workflow.Recognize
{
    /// <summary>
    /// Activity based on NativeActivity<TResult>
    /// </summary>
    public sealed class CreateDocumentModelActivity : NativeActivity
    {
        // Define an activity input argument of Type String
        [RequiredArgument]
        public InArgument<SplitedDocument> SplitedDocument { get; set; }

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
            SplitedDocument sDoc = context.GetValue(this.SplitedDocument);
            var localEpisode = context.GetValue(this.LocalEpisode);

            // TODO : Code this activity
            DMDocument doc = new DMDocument();

            int index = 0;
            foreach (var para in sDoc.Paragraphs)
            {

                DMParagraph paragraph = new DMParagraph();

                for (int i = 0; i < para.Sentences.Count; i++)
                {
                    var text = para.Sentences[i];
                    
                    var sentence = new DMSentence() {Index =index };
                    sentence.Initialize(text);
                    paragraph.Inlines.Add(sentence);
                    paragraph.Inlines.Add(new Run(" "));

                    index += 1;
                }
                doc.Blocks.Add(paragraph);
            }

            doc.Save(localEpisode.SyncDocumentFilePath);
            localEpisode.ReloadSyncDocument();
        }

        /// <summary>
        /// Register activity's metadata
        /// </summary>
        /// <param name="metadata"></param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            // Register In arguments
            RuntimeArgument dictArg = new RuntimeArgument("SplitedDocument", typeof(SplitedDocument), ArgumentDirection.In);
            metadata.AddArgument(dictArg);
            metadata.Bind(this.SplitedDocument, dictArg);

            // [ParaSenDictionary] Argument must be set
            if (this.SplitedDocument == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[SplitedDocument] argument must be set!",
                        false,
                        "SplitedDocument"));
            }
            // Register In arguments
            RuntimeArgument localEpisodeArg = new RuntimeArgument("LocalEpisode", typeof(LocalEpisode), ArgumentDirection.In);
            metadata.AddArgument(localEpisodeArg);
            metadata.Bind(this.LocalEpisode, localEpisodeArg);

            // [ParaSenDictionary] Argument must be set
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
