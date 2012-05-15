using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace ContentPipelineExtension
{
    /// <summary>
    /// The animation importer is the first step in the process of creating a finished animation out of a premade xml file with instructions.
    /// </summary>
    [ContentImporter(".anim", DisplayName = "Animation Importer", DefaultProcessor = "AnimationProcessor")]
    public class AnimationImporter : ContentImporter<AnimationContent>
    {
        /// <summary>
        /// Import an animation.
        /// </summary>
        /// <param name="filename">The path to the animation file.</param>
        /// <param name="context">The content context.</param>
        /// <returns>The animation content.</returns>
        public override AnimationContent Import(string filename, ContentImporterContext context)
        {
            //Load the file.
            XmlDocument file = new XmlDocument();
            file.Load(filename);

            //Create the animation content.
            AnimationContent content = new AnimationContent(file, context);
            //Set the file name and directory.
            content.Filename = filename;
            content.Directory = filename.Remove(filename.LastIndexOf('\\'));

            //Return the imported animation content.
            return content;
        }
    }
}
