using System.Xml;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace ContentPipelineExtension
{
    /// <summary>
    /// A keyframe content instance keeps track of all the important information of a keyframe and can be transformed back into
    /// a keyframe whenever the urge feels undeniable.
    /// </summary>
    [ContentSerializerRuntimeType("Library.Animate.Keyframe, Library")]
    public class KeyframeContent
    {
        #region Properties
        /// <summary>
        /// The keyframe's number in the animation.
        /// </summary>
        public int FrameNumber { get; set; }
        /// <summary>
        /// The list of bones to be updated.
        /// </summary>
        public List<BoneContent> BonesToBe { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Create the keyframe content.
        /// </summary>
        /// <param name="node">The xml node to get the information from.</param>
        /// <param name="context">The content importer context.</param>
        public KeyframeContent(XmlNode node, ContentImporterContext context)
        {
            //Create and instantiate the various variables.
            BonesToBe = new List<BoneContent>();

            //Parse the xml data.
            FrameNumber = int.Parse(node.Attributes["FrameNumber"].Value);

            //Go through all bone nodes in the xml node.
            foreach (XmlNode boneNode in node.SelectNodes("Bone"))
            {
                //Create a new bone content instance object and add it to the list.
                BonesToBe.Add(new BoneContent(boneNode, context));
            }
        }
        #endregion
    }
}