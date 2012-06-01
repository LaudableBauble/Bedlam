using System.Xml;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace ContentPipelineExtension
{
    /// <summary>
    /// An skeleton content instance keeps track of all the important information of a skeleton and can be transformed back into
    /// a skeleton whenever the opportunity arises.
    /// </summary>
    [ContentSerializerRuntimeType("Library.Animate.Skeleton, Library")]
    public class SkeletonContent
    {
        #region Properties
        /// <summary>
        /// The file name that this animation was loaded from.
        /// </summary>
        [ContentSerializerIgnore]
        public string Filename { get; set; }
        /// <summary>
        /// The directory from where this animation was loaded from.
        /// </summary>
        [ContentSerializerIgnore]
        public string Directory { get; set; }
        /// <summary>
        /// The list of bones.
        /// </summary>
        public List<BoneContent> _Bones;
        /// <summary>
        /// The list of animation names.
        /// </summary>
        public List<string> _Animations;
        /// <summary>
        /// The sprites of the skeleton.
        /// </summary>
        //public SpriteManager _Sprites;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the skeleton content.
        /// </summary>
        /// <param name="xmlDocument">The Xml document to get the information from.</param>
        /// <param name="context">The content importer context.</param>
        public SkeletonContent(XmlDocument xmlDocument, ContentImporterContext context)
        {
            //You gotta' work harder!
        }
        #endregion
    }
}