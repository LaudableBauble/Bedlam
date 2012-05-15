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
    /// The animation writer saves the animation in a .xnb format so that the game can load its content at its own leisure.
    /// </summary>
    [ContentTypeWriter]
    public class AnimationWriter : ContentTypeWriter<AnimationContent>
    {
        /// <summary>
        /// Write down the animation to a .xnb format.
        /// </summary>
        /// <param name="output">The content writer responsible for writing the output data.</param>
        /// <param name="content">The animation data to write.</param>
        protected override void Write(ContentWriter output, AnimationContent content)
        {
            //Write the data.
            output.Write(content.FrameTime);
            output.Write(content.NumberOfFrames);
            output.WriteObject(content.Keyframes);
        }
        /// <summary>
        /// Get the runtime animation reader.
        /// </summary>
        /// <param name="targetPlatform">The targeted platform.</param>
        /// <returns>The animation reader.</returns>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(Library.Animate.AnimationReader).AssemblyQualifiedName;
        }
        /// <summary>
        /// Get the runtime type.
        /// </summary>
        /// <param name="targetPlatform">The targeted platform.</param>
        /// <returns>The runtime type.</returns>
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(Library.Animate.Animation).AssemblyQualifiedName;
        }
    }
}
