////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ParallaxBackground.cs                                                                                                  //
// Author: Aidan Harries                                                                                                  //
// Date: 11/10/23                                                                                                         //
// Description: This class manages a parallax background effect with moving stars.                                        //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Proj5
{
    /// <summary>
    /// Manages a parallax background with moving stars
    /// </summary>
    public class ParallaxBackground
    {
        // List of positions for each star in the background
        private List<Vector2> _starPositions;

        /// <summary>
        /// Speed at which the stars move to create the parallax effect
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// Size of each star in the background
        /// </summary>
        public float StarSize { get; set; }

        /// <summary>
        /// Constructor for ParallaxBackground
        /// </summary>
        /// <param name="numberOfStars">Number of stars to create</param>
        /// <param name="speed">Speed of the stars for the parallax effect</param>
        /// <param name="starSize">Size of the stars</param>
        /// <param name="screenWidth">Width of the screen</param>
        /// <param name="screenHeight">Height of the screen</param>
        public ParallaxBackground(int numberOfStars, float speed, float starSize, int screenWidth, int screenHeight)
        {
            Speed = speed;
            StarSize = starSize;
            _starPositions = new List<Vector2>();

            Random rand = new Random();
            for (int i = 0; i < numberOfStars; i++)
            {
                float x = rand.Next(screenWidth);
                float y = rand.Next(screenHeight);
                _starPositions.Add(new Vector2(x, y));
            }
        }

        /// <summary>
        /// Draws the parallax stars on the screen
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch for drawing</param>
        /// <param name="starTexture">Texture of the stars</param>
        /// <param name="offset">Offset to apply for parallax effect</param>
        public void Draw(SpriteBatch spriteBatch, Texture2D starTexture, Vector2 offset)
        {
            foreach (var position in _starPositions)
            {
                Vector2 adjustedPosition = position + offset * Speed;
                spriteBatch.Draw(starTexture, adjustedPosition, null, Color.White, 0f, Vector2.Zero, StarSize, SpriteEffects.None, 0);
            }
        }
    }
}
