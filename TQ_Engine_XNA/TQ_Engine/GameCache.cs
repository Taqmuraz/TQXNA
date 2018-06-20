using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rendering;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TQ.TQ_Engine
{
    public struct ModelContainer
    {
        public string name;
        public Model model;

        public ModelContainer(string _name, Model _model)
        {
            name = _name;
            model = _model;
        }
    }
    public class ArrowsView
    {
        public Renderer right { get; private set; }
        public Renderer up { get; private set; }
        public Renderer forward { get; private set; }
        public Renderer center { get; private set; }

        public ArrowsView(Matrix4x4 _target)
        {
            target = _target;
            right = new Renderer(Color.Red, new Matrix4x4(), GameCache.modelsAsset[0]);
            up = new Renderer(Color.Yellow, new Matrix4x4(), GameCache.modelsAsset[0]);
            forward = new Renderer(Color.Blue, new Matrix4x4(), GameCache.modelsAsset[0]);
            center = new Renderer(Color.Green, new Matrix4x4(), GameCache.modelsAsset[0]);

            right.matrix.scale = new Vector(0.2f, 0.2f, 0.2f);
            up.matrix.scale = new Vector(0.2f, 0.2f, 0.2f);
            forward.matrix.scale = new Vector(0.2f, 0.2f, 0.2f);
            center.matrix.scale = new Vector(0.2f, 0.2f, 0.2f);
        }

        public Matrix4x4 target { get; private set; }

        public void Update()
        {

            
            right.matrix.globalPosition = target.right;
            up.matrix.globalPosition = target.up;
            forward.matrix.globalPosition = target.forward;
            center.matrix.globalPosition = Vector.zero;
        }
    }
    public struct SpriteContainer
    {
        public string name;
        public Texture2D sprite;

        public SpriteContainer(string _name, Texture2D _sprite)
        {
            name = _name;
            sprite = _sprite;
        }
    }

    public class GameCache
    {
        public List<Renderer> renderers { get; private set; }
        public Camera camera { get; private set; }
        public ArrowsView arrows { get; private set; }


        public GameCache(Camera cam, List<Renderer> rends)
        {
            renderers = rends;
            camera = cam;
        }
        public GameCache(Camera cam)
        {
            renderers = new List<Renderer>();
            camera = cam;
        }

        public static GameCache currentBuffer { get; private set; }

        public static List<ModelContainer> modelsContent { get; private set; }
        public static List<SpriteContainer> spritesContent { get; private set; }

        public static string[] modelsAsset
        {
            get
            {
                return new string[] { 
                "Models/Cap",
                "Models/Nin"
                };
            }
        }
        public static string[] spritesAsset
        {
            get
            {
                return new string[] {
                    "Sprites/UI/Bone"
                };
            }
        }

        private static void PostInit()
        {

            Renderer r = new Renderer(new Matrix4x4(Vector.up * 2, new Vector(0, 0, 0), Vector.one * 0.01f), modelsAsset[1]);
            
            IncBehaviour.incBehaviours = new List<IncBehaviour>();

            foreach (var inc in IncBehaviour.incBehaviours)
            {
                inc.Update();
            }

            Vector rot = r.matrix.localEulerAngles;
            Vector pos = Vector.zero;

            currentBuffer.arrows = new ArrowsView(r.matrix);

            IncBehaviour rotator = new IncBehaviour(() =>
            {
                r.matrix.globalEulerAngles = rot;

                if (Input.IsKeyPressed(Keys.Left))
                {
                    rot -= Vector.forward * 2;
                }
                if (Input.IsKeyPressed(Keys.Right))
                {
                    rot += Vector.forward * 2;
                }
                if (Input.IsKeyPressed(Keys.Up))
                {
                    rot += Vector.right * 2;
                }
                if (Input.IsKeyPressed(Keys.Down))
                {
                    rot -= Vector.right * 2;
                }
                if (Input.IsKeyPressed(Keys.PageUp))
                {
                    rot += Vector.up * 2;
                }
                if (Input.IsKeyPressed(Keys.PageDown))
                {
                    rot -= Vector.up * 2;
                }
                //pos += Vector.forward * 0.1f;

                //Program.currentEngine.Window.Title = r.matrix.localEulerAngles.ToString();

                });
        }

        public static void Init()
        {
            Camera cam = new Camera(1000, -Vector.forward * 15 + Vector.up * 2, Vector.zero);
            currentBuffer = new GameCache(cam);
            modelsContent = new List<ModelContainer>();
            spritesContent = new List<SpriteContainer>();
            try
            {
                PostInit();
            }
            catch (Exception ex)
            {
                Debugger.Log(ex.StackTrace);
            }
        }
    }
}
