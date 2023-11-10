////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// HowToPlay.cs                                                                                                           //
// Author: Aidan Harries                                                                                                  //
// Date: 11/10/23                                                                                                         //
// Description: This class defines the 'How to Play' screen.                                                              //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace Proj5
{
    /// <summary>
    /// Manages the 'How to Play' screen
    /// </summary>
    public class HowToPlay
    {
        // Variables for fade animation
        private float _fadeAlpha = 1.0f;
        private float _fadeSpeed = 0.02f;

        // Fonts for displaying text
        private SpriteFont _font;
        private SpriteFont _largeFont;

        // Text for instructions and titles
        private string _titleText = "HOW TO PLAY";
        private string _promptText = "Press Enter to Play";
        private string _instruction1 = "Controls:"; 
        private string _instruction2 = "Forward:";
        private string _instruction3 = "'Up' / 'W' / Right Trigger";
        private string _instruction4 = "Back:";
        private string _instruction5 = "'Down' / 'S' / Left Trigger";
        private string _instruction6 = "Rotate Left:";
        private string _instruction7 = "'Left' / 'A' / Left Stick";
        private string _instruction8 = "Rotate Right:";
        private string _instruction9 = "'Right' / 'D' / Left Stick";
        private string _instruction10 = "Shoot:";
        private string _instruction11 = "'Space' / 'A Button'";

        // Positions for text rendering
        private Vector2 _titlePosition;
        private Vector2 _promptPosition;
        private Vector2 _instruction1Position;
        private Vector2 _instruction2Position;
        private Vector2 _instruction3Position;
        private Vector2 _instruction4Position;
        private Vector2 _instruction5Position;
        private Vector2 _instruction6Position;
        private Vector2 _instruction7Position;
        private Vector2 _instruction8Position;
        private Vector2 _instruction9Position;
        private Vector2 _instruction10Position;
        private Vector2 _instruction11Position;

        // Variables for prompt animation
        private float _promptBaselineY;
        private float _oscillationSpeed = 2.0f;

        // Variables to handle enter key press
        private bool _enterPressed = false;
        private TimeSpan _enterPressTime;

        // Fade effect variables
        private float _fadeValue = 0f; 
        private TimeSpan _fadeDuration = TimeSpan.FromSeconds(1);

        // Sound effect for when 'Play' is selected
        private SoundEffect _playSound;

        // Parallax background stars
        private List<ParallaxBackground> _starLayers;
        private Texture2D _starTexture;
        private Texture2D _fadeTexture;

        /// <summary>
        /// Constructor for HowToPlay
        /// </summary>
        /// <param name="content">ContentManager to load the texture</param>
        /// <param name="graphicsDevice">Graphics device for rendering</param>
        public HowToPlay(ContentManager content, GraphicsDevice graphicsDevice)
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
                Proj5.ScreenHeight * 1 / 16
            );
            _promptPosition = new Vector2(
                (Proj5.ScreenWidth - _font.MeasureString(_promptText).X) / 2,
                Proj5.ScreenHeight * 12 / 16
            );
            _promptBaselineY = _promptPosition.Y;
            _instruction1Position = new Vector2(
                (Proj5.ScreenWidth - _font.MeasureString(_instruction1).X) / 2,
                Proj5.ScreenHeight * 4 / 16
            );
            _instruction2Position = new Vector2(
                (Proj5.ScreenWidth - _font.MeasureString(_instruction2).X) / 3.66f,
                Proj5.ScreenHeight * 5 / 16
            );
            _instruction3Position = new Vector2(
                (Proj5.ScreenWidth - _font.MeasureString(_instruction3).X) / 2,
                Proj5.ScreenHeight * 5 / 16
            );
            _instruction4Position = new Vector2(
                (Proj5.ScreenWidth - _font.MeasureString(_instruction4).X) / 3.33f,
                Proj5.ScreenHeight * 6 / 16
            );
            _instruction5Position = new Vector2(
                (Proj5.ScreenWidth - _font.MeasureString(_instruction5).X) / 2,
                Proj5.ScreenHeight * 6 / 16
            );
            _instruction6Position = new Vector2(
                (Proj5.ScreenWidth - _font.MeasureString(_instruction6).X) / 4,
                Proj5.ScreenHeight * 7 / 16
            );
            _instruction7Position = new Vector2(
                (Proj5.ScreenWidth - _font.MeasureString(_instruction7).X) / 2,
                Proj5.ScreenHeight * 7 / 16
            );
            _instruction8Position = new Vector2(
                (Proj5.ScreenWidth - _font.MeasureString(_instruction8).X) / 4.1f,
                Proj5.ScreenHeight * 8 / 16
            );
            _instruction9Position = new Vector2(
                (Proj5.ScreenWidth - _font.MeasureString(_instruction9).X) / 2,
                Proj5.ScreenHeight * 8 / 16
            );
            _instruction10Position = new Vector2(
                (Proj5.ScreenWidth - _font.MeasureString(_instruction10).X) / 3.4f,
                Proj5.ScreenHeight * 9 / 16
            );
            _instruction11Position = new Vector2(
                (Proj5.ScreenWidth - _font.MeasureString(_instruction11).X) / 2,
                Proj5.ScreenHeight * 9 / 16
            );
            _playSound = content.Load<SoundEffect>("Play");
        }

        /// <summary>
        /// Updates the HowToPlay screen state
        /// </summary>
        /// <param name="gameTime">Timing snapshot</param>
        /// <returns>True if screen should transition to gameplay.</returns>
        public bool Update(GameTime gameTime)
        {
            if (_fadeAlpha > 0)
            {
                _fadeAlpha -= _fadeSpeed;
                _fadeAlpha = MathHelper.Clamp(_fadeAlpha, 0f, 1f);
            }

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
        /// Draws the HowToPlay screen contents
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch for drawing textures</param>
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
            spriteBatch.DrawString(_font, _instruction1, _instruction1Position, Color.Gold);
            spriteBatch.DrawString(_font, _instruction2, _instruction2Position, Color.Gold);
            spriteBatch.DrawString(_font, _instruction3, _instruction3Position, Color.White);
            spriteBatch.DrawString(_font, _instruction4, _instruction4Position, Color.Gold);
            spriteBatch.DrawString(_font, _instruction5, _instruction5Position, Color.White);
            spriteBatch.DrawString(_font, _instruction6, _instruction6Position, Color.Gold);
            spriteBatch.DrawString(_font, _instruction7, _instruction7Position, Color.White);
            spriteBatch.DrawString(_font, _instruction8, _instruction8Position, Color.Gold);
            spriteBatch.DrawString(_font, _instruction9, _instruction9Position, Color.White);
            spriteBatch.DrawString(_font, _instruction10, _instruction10Position, Color.Gold);
            spriteBatch.DrawString(_font, _instruction11, _instruction11Position, Color.White);

            Color promptColor = _enterPressed ? Color.Gold : Color.White;
            spriteBatch.DrawString(_font, _promptText, _promptPosition, promptColor);

            spriteBatch.End(); 
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            if (_fadeAlpha > 0)
            {
                Color fadeColor = new Color(0, 0, 0, _fadeAlpha);
                spriteBatch.Draw(_starTexture, new Rectangle(0, 0, Proj5.ScreenWidth, Proj5.ScreenHeight), fadeColor);
            }

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
