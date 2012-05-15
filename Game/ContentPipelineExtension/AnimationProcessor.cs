using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace ContentPipelineExtension
{
    /// <summary>
    /// The animation processor tranforms the raw imported data into a workable animation instance.
    /// </summary>
    [ContentProcessor(DisplayName = "Animation Processor")]
    public class AnimationProcessor : ContentProcessor<AnimationContent, AnimationContent>
    {
        /// <summary>
        /// Process the raw animation data.
        /// </summary>
        /// <param name="content">The raw and imported animation data.</param>
        /// <param name="context">The content context.</param>
        /// <returns>The processed animation data.</returns>
        public override AnimationContent Process(AnimationContent content, ContentProcessorContext context)
        {
            //Return the processed data.
            return content;
        }
    }
}