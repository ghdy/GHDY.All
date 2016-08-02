using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using GHDY.Core;
using GHDY.Workflow.Recognize.Interface;
using GHDY.NLP;

namespace GHDY.Workflow.Recognize
{
    /// <summary>
    /// Activity based on NativeActivity<TResult>
    /// </summary>
    public sealed class SplitEpisodeActivity : NativeActivity
    {
        // Define an activity input argument of Type String
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<EpisodeContent> EpisodeContent { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="context">WF context</param>
        /// <returns></returns>
        protected override void Execute(NativeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            var content = context.GetValue(this.EpisodeContent);

            // TODO : Code this activity
            var notifyParaSplited = context.GetExtension<INotifyParagraphSplited>();

            var pIndex = 0;
            foreach (var para in content.Paragraphs)
            {
                var sentences = TextUtility.DetectSentences(para);
                try
                {
                    notifyParaSplited.NotifyParagraphSplited(new SplitedParagraph(para,sentences));
                }
                catch (Exception ex)
                {
                    throw new Exception("Error in SplitEpisodeActivity : notifyParaSplited.NotifyParagraphSplited", ex);
                }
                pIndex += 1;
            }
        }

        /// <summary>
        /// Register activity's metadata
        /// </summary>
        /// <param name="metadata"></param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            metadata.RequireExtension<INotifyParagraphSplited>();

            // Register In arguments
            RuntimeArgument ecArg = new RuntimeArgument("EpisodeContent", typeof(EpisodeContent), ArgumentDirection.In);
            metadata.AddArgument(ecArg);
            metadata.Bind(this.EpisodeContent, ecArg);

            // [Text] Argument must be set
            if (this.EpisodeContent == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[EpisodeContent] argument must be set!",
                        false,
                        "EpisodeContent"));
            }

            // TODO : Add arguments ... etc ...
        }
    }
}
