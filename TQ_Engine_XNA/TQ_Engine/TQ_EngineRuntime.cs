using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using TQ.TQ_Engine;
using Rendering;

namespace TQ
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TQ_EngineRuntime : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public TQ_EngineRuntime()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            SetResolution();
            base.Initialize();
        }
        private void SetResolution()
        {
            GraphicsAdapter adapter = graphics.GraphicsDevice.CreationParameters.Adapter;
            //graphics.ToggleFullScreen();
            Camera.SetResolution(adapter.CurrentDisplayMode.Width,
                adapter.CurrentDisplayMode.Height);
            Rectangle r = Window.ClientBounds;
            Window.AllowUserResizing = true;
            Point center = Window.ClientBounds.Center;
            Mouse.SetPosition(center.X, center.Y);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            try
            {
                foreach (var n in GameCache.modelsAsset)
                {
                    Model m = Content.Load<Model>(n);
                    GameCache.modelsContent.Add(new ModelContainer(n, m));
                }
                foreach (var n in GameCache.spritesAsset)
                {
                    Texture2D t = Content.Load<Texture2D>(n);
                    GameCache.spritesContent.Add(new SpriteContainer(n, t));
                }
            }
            catch (Exception ex)
            {
                Window.Title = ex.Message + " " + ex.StackTrace;
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            // TODO: Add your update logic here

            try
            {
                foreach (var inc in IncBehaviour.incBehaviours)
                {
                    inc.Update();
                }
            }
            catch (Exception ex)
            {
                Window.Title = ex.StackTrace;
                throw;
            }

            if (Input.IsKeyPressed(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        private void CameraControl()
        {
            Camera cam = GameCache.currentBuffer.camera;
            cam.MoveAt(Input.arrows * 0.25f);
            if (!Input.IsKeyPressed(Keys.LeftAlt))
            {
                Vector m = Input.mouse;
                cam.localEulerAngles += new Vector(m.y, -m.x) * 0.25f;
            }
        }

        private void DrawModel(Renderer rend)
        {
            Model model = rend.model;
            if (model != null)
            {
                Camera cam = GameCache.currentBuffer.camera;
                foreach (var mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        ColorS c = rend.material.color;
                        effect.DiffuseColor = rend.loadColorFromMesh ? effect.DiffuseColor : new Vector3(c.r, c.g, c.b);
                        effect.View = cam.viewMatrix;
                        effect.Projection = cam.projectionMatrix;
                        effect.World = mesh.ParentBone.Transform * rend.matrix.ToXNA();
                        effect.CommitChanges();
                    }
                    mesh.Draw();
                }
            }
            else
            {
                Window.Title = "Null!";
            }
        }

        private void SpriteDrawUpdate()
        {
            spriteBatch.Begin();


            // для отрисовки спрайтов (на будущее)


            spriteBatch.End();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            CameraControl();

            // TODO: Add your drawing code here

            SpriteDrawUpdate();

            GameCache.currentBuffer.arrows.Update();

            foreach (var rend in GameCache.currentBuffer.renderers)
            {
                DrawModel(rend);
            }

            base.Draw(gameTime);
        }
    }
}
