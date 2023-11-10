////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// MainMenu.cs                                                                                                            //
// Author: Aidan Harries                                                                                                  //
// Date: 11/10/23                                                                                                         //
// Description: This class defines the MainMenu. It handles the main menu's appearance, interaction, and transitions.     //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace Proj5
{
    /// <summary>
    /// Represents the main menu 
    /// </summary>
    public class MainMenu
    {
        private SpriteFont _font;           // Font for regular text
        private SpriteFont _largeFont;      // Font for larger text

        private string _titleText = "PROXIMA CENTAURI";         // Title text
        private string _promptText = "Press Enter to Play";     // Prompt text

        private Vector2 _titlePosition;     // Position of the title text
        private Vector2 _promptPosition;    // Position of the prompt text

        private float _promptBaselineY;             // Base Y position for prompt
        private float _oscillationSpeed = 2.0f;     // Position of the prompt

        private bool _enterPressed = false;     // Flag to check if enter is pressed
        private TimeSpan _enterPressTime;       // Time when enter was pressed

        private float _fadeValue = 0f;                              // Value for fade effect
        private TimeSpan _fadeDuration = TimeSpan.FromSeconds(1);   // Duration of fade

        private SoundEffect _playSound;     // Sound effect when play is initiated

        private List<ParallaxBackground> _starLayers;   // Background layers for parallax effect
        private Texture2D _starTexture;                 // Texture for stars
        private Texture2D _fadeTexture;                 // Texture for fade effect

        /// <summary>
        /// Constructor for MainMenu
        /// </summary>
        /// <param name="content">Content manager for loading assets</param>
        /// <param name="graphicsDevice">Graphics device for rendering</param>
        public MainMenu(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _starTexture = content.Load<Texture2D>("star");

            _fadeTexture = new Texture2D(graphicsDevice, 1, 1);
            _fadeTexture.SetData(new[] { Color.Black });

            _starLayers = new List<ParallaxBackground>
            {
                new ParallaxBackground(100, 0f, 0.25f, Proj5.ScreenWidth + 20, Proj5.ScreenHeight + 20),     // Backmost layer
                new ParallaxBackground(150, 0f, 0.5f, Proj5.ScreenWidth + 20, Proj5.ScreenHeight + 20),
                new ParallaxBackground(200, 0f, 1f, Proj5.ScreenWidth + 20, Proj5.ScreenHeight + 20)      // Frontmost layer
            };

            _font = content.Load<SpriteFont>("LanaPixel");
            _largeFont = content.Load<SpriteFont>("LanaPixelLarge");

            _titlePosition = new Vector2(
                (Proj5.ScreenWidth - _largeFont.MeasureString(_titleText).X) / 2,
                Proj5.ScreenHeight * 4 / 16
            );
            _promptPosition = new Vector2(
                (Proj5.ScreenWidth - _font.MeasureString(_promptText).X) / 2,
                Proj5.ScreenHeight * 8 / 16
            );
            _promptBaselineY = _promptPosition.Y;

            _playSound = content.Load<SoundEffect>("Play");
        }

        /// <summary>
        /// Updates the state of the main menu
        /// </summary>
        /// <param name="gameTime">Game timing snapshot</param>
        /// <returns>True if the game state needs to change, false otherwise</returns>
        public bool Update(GameTime gameTime)
        {
            if (!_enterPressed)
            {
                _promptPosition.Y = _promptBaselineY + (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * _oscillationSpeed) * 20.0f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !_enterPressed)
            {
                _playSound.Play(0.25f, 0.0f, 0.0f);
                _enterPressed = true;
                _enterPressTime = gameTime.TotalGameTime;
            }

            if (_enterPressed)
            {
                float fadeProgress = (float)(gameTime.TotalGameTime - _enterPressTime).TotalSeconds / (float)_fadeDuration.TotalSeconds;
                _fadeValue = MathHelper.Clamp(fadeProgress, 0f, 1f);

                if (_fadeValue >= 1f)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Draws the main menu to the screen
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for rendering</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var layer in _starLayers)
            {
                layer.Draw(spriteBatch, _starTexture, Vector2.Zero);
            }

            Color black = new Color(0, 0, 0);
            Color darkBlue = new Color(0, 150, 255);

            float outlineThickness = 2.0f;

            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition + new Vector2(-outlineThickness, 0), darkBlue);  // Left
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition + new Vector2(outlineThickness, 0), darkBlue);  // Right
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition + new Vector2(0, -outlineThickness), darkBlue);  // Up
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition + new Vector2(0, outlineThickness), darkBlue);  // Down
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition + new Vector2(-outlineThickness, -outlineThickness), darkBlue);  // Up-Left
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition + new Vector2(outlineThickness, -outlineThickness), darkBlue);  // Up-Right
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition + new Vector2(-outlineThickness, outlineThickness), darkBlue);  // Down-Left
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition + new Vector2(outlineThickness, outlineThickness), darkBlue);  // Down-Right

            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition, black);
            Color promptColor = _enterPressed ? Color.Gold : Color.White;
            spriteBatch.DrawString(_font, _promptText, _promptPosition, promptColor);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            if (_enterPressed)
            {
                byte alphaValue = (byte)(_fadeValue * 255);
                Color fadeColor = new Color((byte)255, (byte)255, (byte)255, alphaValue);
                spriteBatch.Draw(_fadeTexture, new Rectangle(0, 0, Proj5.ScreenWidth, Proj5.ScreenHeight), fadeColor);
            }

            spriteBatch.End();
            spriteBatch.Begin(); 
        }
    }
}
