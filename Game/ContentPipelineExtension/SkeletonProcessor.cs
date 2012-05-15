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
    /// The skeleton processor tranforms the raw imported data into a workable skeleton instance.
    /// </summary>
    [ContentProcessor(DisplayName = "Skeleton Processor")]
    public class SkeletonProcessor : ContentProcessor<SkeletonContent, SkeletonContent>
    {
        /// <summary>
        /// Process the raw skeleton data.
        /// </summary>
        /// <param name="content">The raw and imported skeleton data.</param>
        /// <param name="context">The content context.</param>
        /// <returns>The processed skeleton data.</returns>
        public override SkeletonContent Process(SkeletonContent content, ContentProcessorContext context)
        {
            //Return the processed data.
            return content;
        }
    }
}