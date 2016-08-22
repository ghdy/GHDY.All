using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using GHDY.NLP;

namespace GHDY.Workflow
{
    /// <summary>
    /// Activity based on NativeActivity<TResult>
    /// </summary>
    public sealed class Split2SentencesActivity : NativeActivity<List<String>>
    {
        // Define an activity input argument of Type String
        [RequiredArgument]
        public InArgument<String> Transcript { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="context">WF context</param>
        /// <returns></returns>
        protected override void Execute(NativeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            String transcript = context.GetValue(this.Transcript);

            // TODO : Code this activity
            var sentenceArray = NlpUtilities.DetectSentences(transcript);
            Console.WriteLine("Split 2 Sentences:" + sentenceArray.Length.ToString());

            // Return value
            this.Result.Set(context, sentenceArray.ToList());
        }

        /// <summary>
        /// Register activity's metadata
        /// </summary>
        /// <param name="metadata"></param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            // Register In arguments
            RuntimeArgument textArg = new RuntimeArgument("Transcript", typeof(String), ArgumentDirection.In);
            metadata.AddArgument(textArg);
            metadata.Bind(this.Transcript, textArg);

            // [Text] Argument must be set
            if (this.Transcript == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[Transcript] argument must be set!",
                        false,
                        "Transcript"));
            }

            // Register Out arguments
            RuntimeArgument resultArg = new RuntimeArgument("Result", typeof(List<String>), ArgumentDirection.Out);
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
