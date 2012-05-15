using System.Xml;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace ContentPipelineExtension
{
    /// <summary>
    /// A bone content instance keeps track of all the important information of a bone and can be transformed back into
    /// a boe whenever the crave can't be staved off any longer.
    /// </summary>
    [ContentSerializerRuntimeType("Library.Animate.Bone, Library")]
    public class BoneContent
    {
        #region Properties
        /// <summary>
        /// The name of this bone.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The index of this bone.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// The index of this bone's parent.
        /// </summary>
        public int ParentIndex { get; set; }
        /// <summary>
        /// The position.
        /// </summary>
        public Vector2 Position { get; set; }
        /// <summary>
        /// The rotation.
        /// </summary>
        public float Rotation { get; set; }
        /// <summary>
        /// The bone's scale.
        /// </summary>
        public Vector2 Scale { get; set; }
        /// <summary>
        /// The length of the bone.
        /// </summary>
        public float Length { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Create the bone content.
        /// </summary>
        /// <param name="node">The xml node to get the information from.</param>
        /// <param name="context">The content importer context.</param>
        public BoneContent(XmlNode node, ContentImporterContext context)
        {
            //Parse the xml data.
            Name = node.Attributes["Name"].Value;
            Index = int.Parse(node.SelectSingleNode("Index").InnerText);
            ParentIndex = int.Parse(node.SelectSingleNode("ParentIndex").InnerText);
            Position = new Vector2(float.Parse(node.SelectSingleNode("Position/X").InnerText),
                float.Parse(node.SelectSingleNode("Position/Y").InnerText));
            Rotation = float.Parse(node.SelectSingleNode("Rotation").InnerText);
            Scale = new Vector2(float.Parse(node.SelectSingleNode("Scale/X").InnerText), float.Parse(node.SelectSingleNode("Scale/Y").InnerText));
            Length = float.Parse(node.SelectSingleNode("Length").InnerText);
        }
        #endregion
    }
}