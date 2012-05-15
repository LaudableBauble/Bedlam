using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace ContentPipelineExtension
{
    /// <summary>
    /// The skeleton writer saves the animation in a .xnb format so that the game can load its content at its own leisure.
    /// </summary>
    [ContentTypeWriter]
    public class SkeletonWriter : ContentTypeWriter<SkeletonContent>
    {
        /// <summary>
        /// Write down the skeleton to a .xnb format.
        /// </summary>
        /// <param name="output">The content writer responsible for writing the output data.</param>
        /// <param name="content">The skeleton data to write.</param>
        protected override void Write(ContentWriter output, SkeletonContent content)
        {
            //Write the data.
        }
        /// <summary>
        /// Get the runtime skeleton reader.
        /// </summary>
        /// <param name="targetPlatform">The targeted platform.</param>
        /// <returns>The skeleton reader.</returns>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(Library.Animate.SkeletonReader).AssemblyQualifiedName;
        }
        /// <summary>
        /// Get the runtime type.
        /// </summary>
        /// <param name="targetPlatform">The targeted platform.</param>
        /// <returns>The runtime type.</returns>
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(Library.Animate.Skeleton).AssemblyQualifiedName;
        }
    }
}
