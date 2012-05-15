using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Library.Animate
{
    /// <summary>
    /// The skeleton reader reads the loaded binary skeleton data and translates it back into a functional skeleton instance.
    /// </summary>
    public class SkeletonReader : ContentTypeReader<Skeleton>
    {
        /// <summary>
        /// Read the binary skeleton data and translate it into a funsctional skeleton instance.
        /// </summary>
        /// <param name="input">The content reader.</param>
        /// <param name="existingInstance">The existing skeleton instance.</param>
        /// <returns>The loaded and translated skeleton instance.</returns>
        protected override Skeleton Read(ContentReader input, Skeleton existingInstance)
        {
            //Create the skeleton.
            Skeleton skeleton = existingInstance;

            //Return the loaded and translated skeleton.
            return skeleton;
        }
    }
}
