using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;

namespace GHDY.Workflow
{
    /// <summary>
    /// Activity based on NativeActivity<TResult>
    /// </summary>
    ///
    [Designer(typeof(BookmarkActivityDesigner))]
    public sealed class BookmarkActivity : NativeActivity
    {
        // Define an activity input argument of Type String
        [RequiredArgument]
        public InArgument<String> BookmarkName { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="context">WF context</param>
        /// <returns></returns>
        protected override void Execute(NativeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            String bookmarkName = context.GetValue(this.BookmarkName);
            
            // TODO : Code this activity
            context.CreateBookmark(bookmarkName);
        }

        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Register activity's metadata
        /// </summary>
        /// <param name="metadata"></param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            // Register In arguments
            RuntimeArgument bookmarkArg = new RuntimeArgument("BookmarkName", typeof(String), ArgumentDirection.In);
            metadata.AddArgument(bookmarkArg);
            metadata.Bind(this.BookmarkName, bookmarkArg);

            // [BookmarkName] Argument must be set
            if (this.BookmarkName == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[BookmarkName] argument must be set!",
                        false,
                        "BookmarkName"));
            }

            // TODO : Add arguments ... etc ...
        }
    }
}
