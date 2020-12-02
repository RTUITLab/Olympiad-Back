using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Olympiad.Admin.Components
{
    public class ExerciseTextMarkdown : ComponentBase
    {
        [Parameter]
        public string Markdown { get; set; }

        private MarkupString _markupString = new MarkupString();

        /// <summary>
        /// Gets the <see cref="MarkdownPipeline"/> to use.
        /// </summary>
        public virtual MarkdownPipeline Pipeline => new MarkdownPipelineBuilder()
            .UseEmojiAndSmiley()
            .UseAdvancedExtensions()
            .Build();

        /// <inheritdoc/>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            builder.AddContent(0, _markupString);
        }

        /// <inheritdoc/>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _markupString = new MarkupString(Markdig.Markdown.ToHtml(Markdown, Pipeline));
        }
    }
}
