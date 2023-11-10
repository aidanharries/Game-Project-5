////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Asteroid.cs                                                                                                            //
// Author: Aidan Harries                                                                                                  //
// Date: 11/10/23                                                                                                         //
// Description: This class defines the Asteroid. It handles asteroid movement, collisions, and rendering.                 //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System;

namespace Proj5
{
    /// <summary>
    /// Represents an asteroid
    /// </summary>
    public class Asteroid
    {
        private Texture2D _texture;     // Texture for the asteroid
        private Vector2 _position;      // Current position of the asteroid
        private Vector2 _velocity;      // Current velocity of the asteroid
        private Game _game;             // Reference to the main game class
        private float _radius;          // Radius of the asteroid
        private bool _insideViewport;   // Flag to check if the asteroid is within the viewport

        private Texture2D _explosionTexture;        // Texture for the explosion animation
        private const float TIME_PER_FRAME = 0.1f;  // Time per frame for the explosion animation
        private int _frameCount = 8;                // Number of frames in the explosion animation
        private SoundEffect _explosionSound;        // Sound effect for the explosion
        private float _rotationAngle = 0f;          // Rotation angle for the spinning asteroid

        // Public properites for managing asteroid state
        public bool isExploding = false;
        public bool toBeRemoved = false;
        public bool isInvulnerable = false;
        public int currentFrame;
        public float timeSinceLastFrame;

        /// <summary>
        /// Constructor for Asteroid class
        /// </summary>
        /// <param name="game">Reference to the main game class</param>
        public Asteroid(Game game)
        {
            _game = game;
            var viewport = game.GraphicsDevice.Viewport;
            var rand = new Random();

            int edge = rand.Next(4);

            switch (edge)
            {
                case 0: // Top edge
                    _position = new Vector2(rand.Next(viewport.Width), -50);
                    break;
                case 1: // Bottom edge
                    _position = new Vector2(rand.Next(viewport.Width), viewport.Height + 50);
                    break;
                case 2: // Left edge
                    _position = new Vector2(-50, rand.Next(viewport.Height));
                    break;
                case 3: // Right edge
                    _position = new Vector2(viewport.Width + 50, rand.Next(viewport.Height));
                    break;
            }

            Vector2 center = new Vector2(viewport.Width / 2, viewport.Height / 2);
            _velocity = Vector2.Normalize(center - _position) * 100;

            _radius = 48f;
            _insideViewport = false;
        }

        /// <summary>
        /// Loads content for the asteroid
        /// </summary>
        /// <param name="content">ContentManager to load the texture</param>
        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("asteroid");
            _explosionTexture = content.Load<Texture2D>("asteroid_explode");
            _radius = _texture.Width / 2; 
            _explosionSound = content.Load<SoundEffect>("Explosion");
        }

        /// <summary>
        /// Updates the asteroid's state
        /// </summary>
        /// <param name="gameTime">Timing snapshot</param>
        public void Update(GameTime gameTime)
        {
            var viewport = _game.GraphicsDevice.Viewport;

            _position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            float rotationSpeedMultiplier = 0.25f;
            _rotationAngle += (float)(2 * Math.PI * gameTime.ElapsedGameTime.TotalSeconds * rotationSpeedMultiplier);

            if (!_insideViewport &&
                _position.X + _radius * 2 > 0 && _position.X < viewport.Width &&
                _position.Y + _radius * 2 > 0 && _position.Y < viewport.Height)
            {
                _insideViewport = true;
            }

            if (_insideViewport)
            {
                if ((_position.X <= 0 && _velocity.X < 0) || (_position.X + _radius * 2 >= viewport.Width && _velocity.X > 0))
                {
                    _velocity.X = -_velocity.X;
                }
                if ((_position.Y <= 0 && _velocity.Y < 0) || (_position.Y + _radius * 2 >= viewport.Height && _velocity.Y > 0))
                {
                    _velocity.Y = -_velocity.Y;
                }
            }

            if (isExploding)
            {
                if (currentFrame == 0)
                {
                    _explosionSound.Play(0.06f, 0.1f, 0.0f);
                }

                timeSinceLastFrame += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (timeSinceLastFrame >= TIME_PER_FRAME)
                {
                    currentFrame++;
                    timeSinceLastFrame = 0;
                    if (currentFrame >= _frameCount)
                    {
                        isExploding = false;
                        currentFrame = 0;

                        toBeRemoved = true;
                    }
                }
            }
        }

        /// <summary>
        /// Renders the asteroid
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch for drawing textures</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isExploding)
            {
                Vector2 origin = new Vector2(_radius, _radius); 
                spriteBatch.Draw(_texture, _position + origin, null, Color.White, _rotationAngle, origin, 1.5f, SpriteEffects.None, 0f);
            }
            else
            {
                int frameSize = 120; 
                float scale = 1.5f;
                int scaledFrameSize = (int)(frameSize * scale);
                int offset = (int)(30 * scale); 
                Rectangle sourceRect = new Rectangle(currentFrame * frameSize, 0, frameSize, frameSize);
                spriteBatch.Draw(_explosionTexture, new Vector2(_position.X, _position.Y), sourceRect, Color.White, 0f, new Vector2(offset, offset), scale, SpriteEffects.None, 0f);
            }
        }

        /// <summary>
        /// Checks for collision between the asteroid and a laser.
        /// </summary>
        /// <param name="laser">Laser to check collision on</param>
        /// <returns>Returns true if the laser has collided with an asteroid</returns>
        public bool CheckCollision(Laser laser)
        {
            if (isInvulnerable || isExploding)
            {
                return false;
            }

            float scale = 1.5f;

            float scaledRadius = _radius * scale;

            float distance = Vector2.Distance(this._position + new Vector2(scaledRadius, scaledRadius), laser.position);

            return distance < scaledRadius;
        }

        /// <summary>
        /// Checks for collision between the asteroid and the player's ship
        /// </summary>
        /// <param name="player">Player to check collision on</param>
        /// <returns>Returns true if the player has collided with an asteroid</returns>
        public bool CheckCollision(PlayerShip player)
        {
            Rectangle asteroidRect = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
            Rectangle playerRect = new Rectangle((int)(player.position.X - player.texture.Width / 2), (int)(player.position.Y - player.texture.Height / 2), (int)(player.texture.Width-24), (int)(player.texture.Height-24));

            return asteroidRect.Intersects(playerRect);
        }

        /// <summary>
        /// Checks for collision between the asteroid and planet
        /// </summary>
        /// <param name="planet">Planet to check collision on</param>
        /// <returns>Returns true if the planet has collided with an asteroid</returns>
        public bool CheckCollision(Planet planet)
        {
            float asteroidScale = 1.5f; 
            float asteroidScaledRadius = _radius * asteroidScale;
            Vector2 asteroidCenter = this._position + new Vector2(asteroidScaledRadius, asteroidScaledRadius);

            Vector2 windowCenter = new Vector2(_game.GraphicsDevice.Viewport.Width / 2, _game.GraphicsDevice.Viewport.Height / 2);

            float distance = Vector2.Distance(asteroidCenter, windowCenter);

            System.Diagnostics.Debug.WriteLine("Asteroid-Window Center Distance: " + distance);

            float planetRadius = planet.GetRadius();

            return distance < (asteroidScaledRadius + planetRadius);
        }

        /// <summary>
        /// Sets the velocity of the asteroid
        /// </summary>
        /// <param name="newVelocity">New velocity to set</param>
        public void SetVelocity(Vector2 newVelocity)
        {
            _velocity = newVelocity;
        }
    }
}
