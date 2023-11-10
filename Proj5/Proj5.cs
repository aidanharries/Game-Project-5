////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Proj5.cs                                                                                                               //
// Author: Aidan Harries                                                                                                  //
// Date: 11/10/23                                                                                                         //
// Description: This script handles the main game loop, including initialization, content loading, updates, and rendering.//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SharpDX.XInput;

namespace Proj5
{
    /// <summary>
    /// The main game class for Proj5
    /// </summary>
    public class Proj5 : Game
    {
        // Screen dimensions constants
        private const int SCREEN_WIDTH = 1920;
        private const int SCREEN_HEIGHT = 1080;

        // Graphics device manager for handling screen settings
        private GraphicsDeviceManager _graphics;

        // SpriteBatch for rendering textures
        private SpriteBatch _spriteBatch;

        // Different game states
        private MainMenu _mainMenu;
        private HowToPlay _howToPlay;
        private Gameplay _gameplay;

        // Current state of the game
        private GameState _currentState;

        // Background music
        private Song _backgroundMusic;

        /// <summary>
        /// Accessor for screen width
        /// </summary>
        public static int ScreenWidth => SCREEN_WIDTH;

        /// <summary>
        /// Accessor for screen height
        /// </summary>
        public static int ScreenHeight => SCREEN_HEIGHT;

        /// <summary>
        /// Constructor for the Proj5 game class
        /// </summary>
        public Proj5()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;

            _graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            _graphics.ApplyChanges();

            _currentState = GameState.MainMenu;
        }

        /// <summary>
        /// Loads game content
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _mainMenu = new MainMenu(Content, GraphicsDevice);
            _howToPlay = new HowToPlay(Content, GraphicsDevice);
            _gameplay = new Gameplay(Content, this);

            _backgroundMusic = Content.Load<Song>("Retro_Platforming");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.25f;
            MediaPlayer.Play(_backgroundMusic);
        }

        /// <summary>
        /// Updates the game state
        /// </summary>
        /// <param name="gameTime">Timing snapshot</param>
        protected override void Update(GameTime gameTime)
        {
            // Exit the game when Escape is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // State management logic
            switch (_currentState)
            {
                case GameState.MainMenu:
                    if (_mainMenu.Update(gameTime))
                    {
                        _currentState = GameState.HowToPlay;
                    }
                    break;

                case GameState.HowToPlay:
                    if (_howToPlay.Update(gameTime))
                    {
                        _currentState = GameState.Gameplay;
                    }
                    break;

                case GameState.Gameplay:
                    _gameplay.Update(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the game content
        /// </summary>
        /// <param name="gameTime">Timing snapshot</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Render current state
            switch (_currentState)
            {
                case GameState.MainMenu:
                    _mainMenu.Draw(_spriteBatch);
                    break;

                case GameState.HowToPlay:
                    _howToPlay.Draw(_spriteBatch);
                    break;

                case GameState.Gameplay:
                    _gameplay.Draw(gameTime, _spriteBatch);
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}