using System.Xml;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace ContentPipelineExtension
{
    /// <summary>
    /// An animation content instance keeps track of all the important information of an animation and can be transformed back into
    /// an animation whenever the opportunity arises.
    /// </summary>
    [ContentSerializerRuntimeType("Library.Animate.Animation, Library")]
    public class AnimationContent
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
        /// The time every frame has onscren.
        /// </summary>
        public float FrameTime { get; set; }
        /// <summary>
        /// The number of frames in the animation.
        /// </summary>
        public int NumberOfFrames { get; set; }
        /// <summary>
        /// The list of keyframes in this animation.
        /// </summary>
        public List<KeyframeContent> Keyframes { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Create the animation content.
        /// </summary>
        /// <param name="xmlDocument">The Xml document to get the information from.</param>
        /// <param name="context">The content importer context.</param>
        public AnimationContent(XmlDocument xmlDocument, ContentImporterContext context)
        {
            //Create and instantiate the various variables.
            Keyframes = new List<KeyframeContent>();

            //Parse the xml data.
            FrameTime = float.Parse(xmlDocument.SelectSingleNode("/Animation/FrameTime").InnerText);
            NumberOfFrames = int.Parse(xmlDocument.SelectSingleNode("/Animation/NumberOfFrames").InnerText);

            //Go through all keyframe nodes in the xml document.
            foreach (XmlNode keyframeNode in xmlDocument.SelectNodes("/Animation/Keyframe"))
            {
                //Create a new keyframe content instance object and add it to the list.
                Keyframes.Add(new KeyframeContent(keyframeNode, context));
            }
        }
        #endregion
    }
}