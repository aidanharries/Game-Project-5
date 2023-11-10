////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// PlayerShip.cs                                                                                                          //
// Author: Aidan Harries                                                                                                  //
// Date: 11/10/23                                                                                                         //
// Description: This class represents the player's ship. It handles the ship's movement, shooting, and rendering.         //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Proj5
{
    /// <summary>
    /// Represents the player's ship
    /// </summary>
    public class PlayerShip
    {
        // Constants for ship's behavior
        const float SPEED = 600;
        const float ANGULAR_ACCELERATION = 12;
        const float LINEAR_DRAG = 0.97f;
        const float ANGULAR_DRAG = 0.93f;
        const float LASER_COOLDOWN = 0.33f;
        const float RED_TIME = 0.5f;

        // Ship's texture and physics properties
        public Texture2D texture;
        public Vector2 position;
        public Vector2 velocity;

        // Game context and ship's movement attributes
        private Game _game;
        private Vector2 _direction;
        private float _angle;
        private float _angularVelocity;

        // Lasers fired by the ship
        private List<Laser> _lasers;

        // Timers for laser firing and visual effects
        private float _timeSinceLastLaser = 0;
        private float _timeToBeRed = 0;

        // Sound effect for shooting
        private SoundEffect _shootingSound;

        /// <summary>
        /// Constructor for the PlayerShip
        /// </summary>
        /// <param name="game">Reference to the main game object</param>
        public PlayerShip(Game game)
        {
            this._game = game;
            position = new Vector2(375, 250);
            _direction = -Vector2.UnitY;
            _angularVelocity = 0;
            _lasers = new List<Laser>();
        }

        /// <summary>
        /// Loads the content for the player ship
        /// </summary>
        /// <param name="content">ContentManager for loading resources</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("ship");
            _shootingSound = content.Load<SoundEffect>("shoot");
        }

        /// <summary>
        /// Updates the state of the player ship
        /// </summary>
        /// <param name="gameTime">Timing snapshot</param>
        public void Update(GameTime gameTime)
        {
            // Input handling and movement logic
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 acceleration = new Vector2(0, 0);
            float angularAcceleration = 0;

            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                acceleration += _direction * SPEED;
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                acceleration -= _direction * SPEED;
            }

            if (gamePadState.Triggers.Right > 0)
            {
                acceleration += _direction * SPEED * gamePadState.Triggers.Right;
            }
            if (gamePadState.Triggers.Left > 0)
            {
                acceleration -= _direction * SPEED * gamePadState.Triggers.Left;
            }

            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A) || gamePadState.ThumbSticks.Left.X < 0)
            {
                angularAcceleration -= ANGULAR_ACCELERATION;
            }
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D) || gamePadState.ThumbSticks.Left.X > 0)
            {
                angularAcceleration += ANGULAR_ACCELERATION;
            }

            // Laser shooting and removal logic
            _timeSinceLastLaser += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if ((keyboardState.IsKeyDown(Keys.Space) || gamePadState.Buttons.A == ButtonState.Pressed) && _timeSinceLastLaser > LASER_COOLDOWN)
            {
                Laser newLaser = new Laser(this.position, this._direction, this._angle);
                newLaser.LoadContent(this._game.Content);
                _lasers.Add(newLaser);
                _timeSinceLastLaser = 0;

                _shootingSound.Play(0.20f, 0.2f, 0.0f);
            }

            List<Laser> lasersToRemove = new List<Laser>();
            var viewport = _game.GraphicsDevice.Viewport;
            foreach (var laser in _lasers)
            {
                laser.Update(gameTime);

                if (laser.position.Y < 0 || laser.position.Y > viewport.Height ||
                    laser.position.X < 0 || laser.position.X > viewport.Width)
                {
                    lasersToRemove.Add(laser);
                }
            }

            if (_timeToBeRed > 0)
            {
                _timeToBeRed -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            foreach (var laser in lasersToRemove)
            {
                _lasers.Remove(laser);
            }

            velocity += acceleration * t;
            position += velocity * t;

            _angularVelocity += angularAcceleration * t;
            _angle += _angularVelocity * t;

            _direction.X = (float)Math.Sin(_angle);
            _direction.Y = -(float)Math.Cos(_angle);

            velocity *= LINEAR_DRAG;
            _angularVelocity *= ANGULAR_DRAG;

            // Ship wrapping logic
            if (position.Y < 0) position.Y = viewport.Height;
            if (position.Y > viewport.Height) position.Y = 0;
            if (position.X < 0) position.X = viewport.Width;
            if (position.X > viewport.Width) position.X = 0;
        }

        /// <summary>
        /// Draws the player ship and its lasers
        /// </summary>
        /// <param name="gameTime">Timing snapshot</param>
        /// <param name="spriteBatch">SpriteBatch for drawing textures</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw ship
            Color color = _timeToBeRed > 0 ? Color.Red : Color.White;
            spriteBatch.Draw(texture, position, null, color, _angle, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0);

            // Draw lasers
            foreach (var laser in _lasers)
            {
                laser.Draw(gameTime, spriteBatch);
            }
        }

        /// <summary>
        /// List of lasers fired by the ship
        /// </summary>
        /// <returns>The list of lasers</returns>
        public List<Laser> GetLasers()
        {
            return _lasers;
        }

        /// <summary>
        /// Removes specified laser from the list
        /// </summary>
        /// <param name="laser">The laser to remove</param>
        public void RemoveLaser(Laser laser)
        {
            _lasers.Remove(laser);
        }

        /// <summary>
        /// Sets the ships color to red for a short duration
        /// </summary>
        public void SetToBeRed()
        {
            this._timeToBeRed = RED_TIME;
        }

    }
}
