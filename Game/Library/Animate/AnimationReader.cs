using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Library.Animate
{
    /// <summary>
    /// The animation reader reads the loaded binary animation data and translates it back into a functional animation instance.
    /// </summary>
    public class AnimationReader : ContentTypeReader<Animation>
    {
        /// <summary>
        /// Read the binary animation data and translate it into a functional animation instance.
        /// </summary>
        /// <param name="input">The content reader.</param>
        /// <param name="existingInstance">The existing animation instance.</param>
        /// <returns>The loaded and translated animation instance.</returns>
        protected override Animation Read(ContentReader input, Animation existingInstance)
        {
            //Create the animation.
            Animation animation = existingInstance == null ? new Animation() : existingInstance;

            //Parse the data.
            animation.FrameTime = input.ReadSingle();
            animation.NumberOfFrames = input.ReadInt32();
            animation.Keyframes =  input.ReadObject<List<Keyframe>>();

            //Go through each keyframe.
            /*foreach (Keyframe keyframe in animation.Keyframes)
            {
                //Add each bone.
                for (int bone = 0; bone < keyframe.BonesToBe.Count; bone++)
                {
                    keyframe.BonesToBe[bone].Index = bone;
                }
            }*/

            //Return the loaded and translated animation.
            return animation;
        }
    }
}
