using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using GHDY.Workflow.Recognize.Interface;

namespace GHDY.Workflow.Recognize
{
    /// <summary>
    /// Activity based on NativeActivity<TResult>
    /// </summary>
    public sealed class RecognizeStateChangedActivity : NativeActivity
    {
        // Define an activity input argument of Type String
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<RecognizeTransition> Transition { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="context">WF context</param>
        /// <returns></returns>
        protected override void Execute(NativeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            var transition = context.GetValue(this.Transition);

            // TODO : Code this activity
            var notifyStateChanged = context.GetExtension<INotifyRecognizeStateChanged>();
            notifyStateChanged.NotifyRecognizeStateChanged((RecognizeTransition)Enum.Parse(typeof(RecognizeTransition), transition.ToString()));
        }

        /// <summary>
        /// Register activity's metadata
        /// </summary>
        /// <param name="metadata"></param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            metadata.RequireExtension<INotifyRecognizeStateChanged>();

            // Register In arguments
            RuntimeArgument transitionArg = new RuntimeArgument("Transition", typeof(RecognizeTransition), ArgumentDirection.In);
            metadata.AddArgument(transitionArg);
            metadata.Bind(this.Transition, transitionArg);

            // [Text] Argument must be set
            if (this.Transition == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[Transition] argument must be set!",
                        false,
                        "Transition"));
            }

            // TODO : Add arguments ... etc ...
        }
    }
}
