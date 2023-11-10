////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Gameplay.cs                                                                                                            //
// Author: Aidan Harries                                                                                                  //
// Date: 11/10/23                                                                                                         //
// Description: This class defines the gameplay logic. It controls player movement, asteroids, collision, and rendering.  //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Proj5
{
    /// <summary>
    /// Manages the gameplay mechanics
    /// </summary>
    public class Gameplay
    {
        // Variables for fading effect
        private float _fadeAlpha = 1.0f;
        private float _fadeSpeed = 0.02f;

        // Game entities
        private PlayerShip _playerShip;
        private Asteroid _asteroid;

        // Game instance and content manager
        private Game _gameInstance;
        private ContentManager _contentManager;

        // Parallax backgrounds for stars
        private List<ParallaxBackground> _starLayers;
        private Texture2D _starTexture;

        // Planet and Proj5 instance
        private Planet _planet;
        private Proj5 _proj5;

        /// <summary>
        /// Constructor initializing gameplay elements
        /// </summary>
        /// <param name="content">ContentManager to load the texture</param>
        /// <param name="game">Reference to the main game class</param>
        public Gameplay(ContentManager content, Proj5 game)
        {
            _starTexture = content.Load<Texture2D>("star");

            _playerShip = new PlayerShip(game);
            _playerShip.LoadContent(content);

            _asteroid = new Asteroid(game);
            _asteroid.LoadContent(content);

            _gameInstance = game;
            _contentManager = content;

            // Initialize star layers for parallax effect
            _starLayers = new List<ParallaxBackground>
            {
                // Backmost layer
                new ParallaxBackground(100, 0f, 0.25f, Proj5.ScreenWidth + 20, Proj5.ScreenHeight + 20),
                // Middle layer
                new ParallaxBackground(150, 0.01f, 0.5f, Proj5.ScreenWidth + 20, Proj5.ScreenHeight + 20),
                // Frontmost layer
                new ParallaxBackground(200, 0.03f, 1f, Proj5.ScreenWidth + 20, Proj5.ScreenHeight + 20),
            };

            _proj5 = game;
            _planet = new Planet(game);
        }

        /// <summary>
        /// Updates the state of gameplay elements
        /// </summary>
        /// <param name="gameTime">Timing snapshot</param>
        public void Update(GameTime gameTime)
        {
            // Fade effect logic
            if (_fadeAlpha > 0)
            {
                _fadeAlpha -= _fadeSpeed;
                _fadeAlpha = MathHelper.Clamp(_fadeAlpha, 0f, 1f);
            }

            // Update game entities
            _playerShip.Update(gameTime);
            _asteroid.Update(gameTime);
            _planet.Update(gameTime);

            // Collision handling logic
            Laser collidedLaser = null;
            foreach (var laser in _playerShip.GetLasers())
            {
                if (_asteroid.CheckCollision(laser))
                {
                    collidedLaser = laser;
                    break;
                }
            }

            if (collidedLaser != null)
            {
                _playerShip.RemoveLaser(collidedLaser);

                if (!_asteroid.isExploding && !_asteroid.isInvulnerable)
                {
                    _asteroid.isInvulnerable = true; 
                    _asteroid.isExploding = true; 
                    _asteroid.currentFrame = 0; 
                    _asteroid.timeSinceLastFrame = 0;  

                    _asteroid.SetVelocity(Vector2.Zero);
                }
            }

            if (_asteroid.CheckCollision(_playerShip))
            {
                if (!_asteroid.isExploding && !_asteroid.isInvulnerable)
                {
                    _playerShip.SetToBeRed();

                    _asteroid.isInvulnerable = true; 
                    _asteroid.isExploding = true;    
                    _asteroid.currentFrame = 0;      
                    _asteroid.timeSinceLastFrame = 0;  

                    _asteroid.SetVelocity(Vector2.Zero);
                }
            }

            if (_asteroid.CheckCollision(_planet))
            {
                if (!_asteroid.isExploding && !_asteroid.isInvulnerable)
                {
                    _asteroid.isInvulnerable = true; 
                    _asteroid.isExploding = true;    
                    _asteroid.currentFrame = 0;      
                    _asteroid.timeSinceLastFrame = 0;  

                    _asteroid.SetVelocity(Vector2.Zero);
                }
            }

            if (_asteroid.toBeRemoved)
            {
                _asteroid = new Asteroid(_gameInstance);
                _asteroid.LoadContent(_contentManager);
            }
        }

        /// <summary>
        /// Draws the game content
        /// </summary>
        /// <param name="gameTime">Timing snapshot</param>
        /// <param name="spriteBatch">SpriteBatch for drawing textures</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 playerMovementOffset = _playerShip.velocity;

            spriteBatch.End(); 

            foreach (var layer in _starLayers)
            {
                Matrix transformMatrix = Matrix.CreateTranslation(new Vector3(playerMovementOffset * layer.Speed, 0));
                spriteBatch.Begin(transformMatrix: transformMatrix);
                layer.Draw(spriteBatch, _starTexture, playerMovementOffset);
                spriteBatch.End();
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            if (_fadeAlpha > 0)
            {
                Color fadeColor = new Color(0, 0, 0, _fadeAlpha);
                spriteBatch.Draw(_starTexture, new Rectangle(0, 0, Proj5.ScreenWidth, Proj5.ScreenHeight), fadeColor);
            }

            spriteBatch.End();

            spriteBatch.Begin(); 

            _planet.Draw();
            _playerShip.Draw(gameTime, spriteBatch);
            _asteroid.Draw(spriteBatch);
        }
    }
}
