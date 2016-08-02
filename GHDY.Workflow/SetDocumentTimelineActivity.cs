using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using GHDY.Core;
using GHDY.Core.DocumentModel;
using System.IO;

namespace GHDY.Workflow
{
    /// <summary>
    /// Activity based on NativeActivity<TResult>
    /// </summary>
    public sealed class SetDocumentTimelineActivity : NativeActivity<Boolean>
    {
        // Define an activity input argument of Type String
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<Lyrics> Lyrics { get; set; }

        [RequiredArgument]
        [DefaultValue("")]
        public InArgument<string> DocumentFilePath { get; set; }

        [DefaultValue(true)]
        public InArgument<bool> NeedLoadDocument { get; set; }

        [DefaultValue(true)]
        public InArgument<bool> NeedSaveDocument { get; set; }

        [DefaultValue(null)]
        public InArgument<DMDocument> Document { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="context">WF context</param>
        /// <returns></returns>
        protected override void Execute(NativeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            var lrc = context.GetValue(this.Lyrics);
            var filePath = context.GetValue(this.DocumentFilePath);
            var doc = context.GetValue(this.Document);
            var needSave = context.GetValue(this.NeedSaveDocument);
            var needload = context.GetValue(this.NeedLoadDocument);

            // TODO : Code this activity
            if (needload == true)
                doc = DMDocument.Load(filePath);

            bool result = false;

            if (doc == null)
            {
                if (File.Exists(filePath) == true)
                {
                    doc = DMDocument.Load(filePath);

                    if (doc.Sentences.Count() != lrc.Phrases.Count)
                    {
                        result = false;
                    }
                    else
                    {
                        int index = 0;
                        foreach (var sentence in doc.Sentences)
                        {
                            var phrase = lrc.Phrases[index];

                            sentence.BeginTime = phrase.BeginTime;
                            sentence.EndTime = phrase.EndTime;
                            index += 1;
                        }
                        result = true;
                    }
                }
                else
                    doc = BuildDocument(lrc);
                result = true;
            }

            try
            {
                if (result == true && needSave == true)
                    doc.Save(filePath);
            }
            catch
            { 
            
            }
            // Return value
            this.Result.Set(context, result);
        }

        private static DMDocument BuildDocument(Core.Lyrics lrc)
        {
            var doc = new DMDocument();
            DMParagraph para = new DMParagraph();
            int index = 0;
            foreach (var phrase in lrc.Phrases)
            {
                var sentence = new DMSentence(index)
                {
                    BeginTime = phrase.BeginTime,
                    EndTime = phrase.EndTime,
                };
                sentence.Initialize(lrc.Transcript);

                para.Inlines.Add(sentence);

                index += 1;
            }
            doc.Blocks.Add(para);
            return doc;
        }

        /// <summary>
        /// Register activity's metadata
        /// </summary>
        /// <param name="metadata"></param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            // Register In arguments
            RuntimeArgument lrcArg = new RuntimeArgument("Lyrics", typeof(Lyrics), ArgumentDirection.In);
            metadata.AddArgument(lrcArg);
            metadata.Bind(this.Lyrics, lrcArg);

            // [Lyrics] Argument must be set
            if (this.Lyrics == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[Lyrics] argument must be set!",
                        false,
                        "Lyrics"));
            }

            RuntimeArgument docPathArg = new RuntimeArgument("DocumentFilePath", typeof(string), ArgumentDirection.In);
            metadata.AddArgument(docPathArg);
            metadata.Bind(this.DocumentFilePath, docPathArg);

            // [DocumentFilePath] Argument must be set
            if (this.DocumentFilePath == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[DocumentFilePath] argument must be set!",
                        false,
                        "DocumentFilePath"));
            }


            RuntimeArgument needLoadArg = new RuntimeArgument("NeedLoadDocument", typeof(bool), ArgumentDirection.In);
            metadata.AddArgument(needLoadArg);
            metadata.Bind(this.NeedLoadDocument, needLoadArg);


            RuntimeArgument needSaveArg = new RuntimeArgument("NeedSaveDocument", typeof(bool), ArgumentDirection.In);
            metadata.AddArgument(needSaveArg);
            metadata.Bind(this.NeedSaveDocument, needSaveArg);

            RuntimeArgument docArg = new RuntimeArgument("Document", typeof(DMDocument), ArgumentDirection.In);
            metadata.AddArgument(docArg);
            metadata.Bind(this.Document, docArg);


            // Register Out arguments
            RuntimeArgument resultArg = new RuntimeArgument("Result", typeof(bool), ArgumentDirection.Out);
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
