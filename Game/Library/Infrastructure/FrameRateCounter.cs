using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Library.Infrastructure
{
    public class FrameRateCounter : DrawableGameComponent
    {
        #region Fields
        private int _FrameRate;
        private int _FrameCounter;
        private TimeSpan _ElapsedTime;
        #endregion

        #region Constructors
        /// <summary>
        /// Create the frame rate counter.
        /// </summary>
        /// <param name="game">The game instance to derive from.</param>
        public FrameRateCounter(Game game)
            : base(game)
        {
            //Initialize some variables.
            _FrameRate = 0;
            _FrameCounter = 0;
            _ElapsedTime = TimeSpan.Zero;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Update the frame rate counter.
        /// </summary>
        /// <param name="gameTime">The time to use.</param>
        public override void Update(GameTime gameTime)
        {
            //Update the elapsed time.
            _ElapsedTime += gameTime.ElapsedGameTime;

            //Keep the counter in check.
            if (_ElapsedTime > TimeSpan.FromSeconds(1))
            {
                _ElapsedTime -= TimeSpan.FromSeconds(1);
                _FrameRate = _FrameCounter;
                _FrameCounter = 0;
            }
        }
        /// <summary>
        /// Draw the frame rate counter.
        /// </summary>
        /// <param name="gameTime">The time to use.</param>
        public override void Draw(GameTime gameTime)
        {
            //Increment the frame counter.
            _FrameCounter++;

            //Write the FPS into the game window title.
            Game.Window.Title = string.Format("FPS: {0}", _FrameRate);
        }
        #endregion
    }
}
