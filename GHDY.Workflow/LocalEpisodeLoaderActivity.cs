using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using GHDY.Core.Episode;
using GHDY.Core;

namespace GHDY.Workflow
{
    /// <summary>
    /// Activity based on CodeActivity
    /// </summary>
    public sealed class LocalEpisodeLoaderActivity : CodeActivity<LocalEpisodeProvider>
    {
        // Define an activity input argument of type string
        [RequiredArgument]
        public InArgument<String> Folder { get; set; }

        public InArgument<List<EpisodeFileTypes>> MustHaveFileTypes { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="context">WF context</param>
        protected override LocalEpisodeProvider Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            String folder = context.GetValue(this.Folder);

            // TODO : Code this activity
            //this.EpisodeProvider = new OutArgument<LocalEpisodeProvider>();
            //this.Result.Set(context, new LocalEpisodeProvider(folder));

            return new LocalEpisodeProvider(folder);
        }

        /// <summary>
        /// Register activity's metadata
        /// </summary>
        /// <param name="metadata"></param>
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            // Register In arguments
            RuntimeArgument folderArg = new RuntimeArgument("Folder", typeof(String), ArgumentDirection.In);
            metadata.AddArgument(folderArg);
            metadata.Bind(this.Folder, folderArg);

            // [Text] Argument must be set
            if (this.Folder == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[Folder] argument must be set!",
                        false,
                        "Folder"));
            }

            // TODO : Add arguments ... etc ...

            RuntimeArgument epArg = new RuntimeArgument("Result", typeof(LocalEpisodeProvider), ArgumentDirection.Out);
            metadata.AddArgument(epArg);
            metadata.Bind(this.Result, epArg);

            if (this.Result == null)
            {
                metadata.AddValidationError(new System.Activities.Validation.ValidationError(
                    "[Result] argument must be set!", 
                    false, 
                    "Result"));
            }
        }
    }
}
