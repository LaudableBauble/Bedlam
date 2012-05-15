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
    /// The skeleton importer is the first step in the process of creating a finished skeleton out of a premade xml file with instructions.
    /// </summary>
    [ContentImporter(".skel", DisplayName = "Skeleton Importer", DefaultProcessor = "SkeletonProcessor")]
    public class SkeletonImporter : ContentImporter<SkeletonContent>
    {
        /// <summary>
        /// Import a skeleton.
        /// </summary>
        /// <param name="filename">The path to the skeleton file.</param>
        /// <param name="context">The content context.</param>
        /// <returns>The animation content.</returns>
        public override SkeletonContent Import(string filename, ContentImporterContext context)
        {
            //Load the file.
            XmlDocument file = new XmlDocument();
            file.Load(filename);

            //Create the skeleton content.
            SkeletonContent content = new SkeletonContent(file, context);
            //Set the file name and directory.
            content.Filename = filename;
            content.Directory = filename.Remove(filename.LastIndexOf('\\'));

            //Return the imported skeleton content.
            return content;
        }
    }
}
