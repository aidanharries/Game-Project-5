////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Planet.cs                                                                                                              //
// Author: Aidan Harries                                                                                                  //
// Date: 11/10/23                                                                                                         //
// Description: This class defines a 3D planet model, including rotation and basic effect initialization.                 //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Proj5
{
    /// <summary>
    /// Represents a 3D planet
    /// </summary>
    public class Planet
    {
        // The 3D model of the planet
        private Model planetModel;

        // Reference to the main game class
        private Proj5 game;

        // World matrix for the planet model
        private Matrix worldMatrix;

        // Constant scale for the planet model
        private const float SCALE = 0.15f;
        
        /// <summary>
        /// Construtor for the Planet class
        /// </summary>
        /// <param name="game">Reference to the main game class</param>
        public Planet(Proj5 game)
        {
            this.game = game;
            LoadModel();
            InitializeEffect();
        }

        /// <summary>
        /// Loads the 3D model for the planet
        /// </summary>
        private void LoadModel()
        {
            planetModel = game.Content.Load<Model>("planet");
        }

        /// <summary>
        /// Initializes the basic effects for rendering the planet
        /// </summary>
        void InitializeEffect()
        {
            foreach (ModelMesh mesh in planetModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Matrix.CreateScale(SCALE);
                    effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 2), Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, game.GraphicsDevice.Viewport.AspectRatio, 0.1f, 100.0f);
                }
            }
        }

        /// <summary>
        /// Updates the planet's rotation
        /// </summary>
        /// <param name="gameTime">Timing snapshot</param>
        public void Update(GameTime gameTime)
        {
            float rotationSpeed = 0.5f;
            float deltaTime = (float)gameTime.TotalGameTime.TotalSeconds * rotationSpeed;

            worldMatrix = Matrix.CreateRotationY(deltaTime) * Matrix.CreateScale(SCALE);
        }

        /// <summary>
        /// Draws the planet model
        /// </summary>
        public void Draw()
        {
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (ModelMesh mesh in planetModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = worldMatrix;
                }
                mesh.Draw();
            }
        }

        /// <summary>
        /// Calculates and returns the radius of the planet
        /// </summary>
        /// <returns>The radius of the planet</returns>
        public float GetRadius()
        {
            return SCALE * 500;
        }
    }
}
