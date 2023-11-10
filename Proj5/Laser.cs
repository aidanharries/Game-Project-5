////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Laser.cs                                                                                                               //
// Author: Aidan Harries                                                                                                  //
// Date: 11/10/23                                                                                                         //
// Description: This class defines the Laser class. It handles the initialization, movement, and rendering of lasers.     //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Proj5
{
    /// <summary>
    /// Represents a laser
    /// </summary>
    public class Laser
    {
        // Speed constant for the laser movement
        private const float SPEED = 1000;

        // Public fields for laser properties
        public Vector2 position;
        public Vector2 direction;
        public float angle;

        // Texture for the laser
        private Texture2D _texture;

        /// <summary>
        /// Constructor for the Laser class
        /// </summary>
        /// <param name="startPosition">Initial position of the laser</param>
        /// <param name="startDirection">Initial direction of the laser</param>
        /// <param name="angle">Initial angle of the laser</param>
        public Laser(Vector2 startPosition, Vector2 startDirection, float angle)
        {
            position = startPosition;
            direction = startDirection;
            this.angle = angle;
        }

        /// <summary>
        /// Loads the laser texture
        /// </summary>
        /// <param name="content">ContentManager to load the texture</param>
        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("laser");
        }

        /// <summary>
        /// Updates the laser's position based on its speed and direction
        /// </summary>
        /// <param name="gameTime">Game time information</param>
        public void Update(GameTime gameTime)
        {
            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += direction * SPEED * t;
        }

        /// <summary>
        /// Draws the laser on the screen
        /// </summary>
        /// <param name="gameTime">Game time information</param>
        /// <param name="spriteBatch">SpriteBatch for drawing the laser</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, position, null, Color.White, angle, new Vector2(_texture.Width / 2, _texture.Height / 2), 1.5f, SpriteEffects.None, 0);
        }
    }
}
