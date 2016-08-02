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
    [Designer(typeof(BookmarkResultActivityDesigner))]
    public sealed class BookmarkResultActivity<TResult> : NativeActivity<TResult>
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
            context.CreateBookmark(bookmarkName, new BookmarkCallback(ResumeBookmarkCallback));
        }

        void ResumeBookmarkCallback(NativeActivityContext context, Bookmark bookmark, object result)
        {
            // Return value
            this.Result.Set(context, (TResult)result);
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

            // Register Out arguments
            RuntimeArgument resultArg = new RuntimeArgument("Result", typeof(TResult), ArgumentDirection.Out);
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
