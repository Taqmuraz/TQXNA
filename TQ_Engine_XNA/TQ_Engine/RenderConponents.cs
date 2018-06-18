using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TQ.TQ_Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TQ;


namespace Rendering
{

    public struct Material
    {
        public ColorS color;

        public Material(ColorS c)
        {
            color = c;
        }
    }

	public class Renderer
	{
		public Matrix4x4 matrix { get; private set; }
        public AnimWorks animations { get; private set; }
        public Material material { get; private set; }

        public bool loadColorFromMesh { get; private set; }

        public Model model
        {
            get
            {
                return LoadModelByName(modelName);
            }
        }
        public string modelName { get; private set; }

		public Renderer (Matrix4x4 _matrix, string _name) {
            loadColorFromMesh = true;
            material = new Material(Color.White);
			matrix = _matrix;
            modelName = _name;
            animations = new AnimWorks(this);
            GameCache.currentBuffer.renderers.Add(this);
		}
        public Renderer(ColorS color, Matrix4x4 _matrix, string _name)
        {
            loadColorFromMesh = false;
            material = new Material(color);
            matrix = _matrix;
            modelName = _name;
            animations = new AnimWorks(this);
            GameCache.currentBuffer.renderers.Add(this);
        }

		public void Destroy () {
            GameCache.currentBuffer.renderers.Remove(this);
		}


        public static Model LoadModelByName(string name)
        {
            return GameCache.modelsContent.FirstOrDefault((ModelContainer mc) => mc.name == name).model;
        }
	}

	public class Camera : Matrix4x4
	{
        public float drawDistance { get; private set; }
        public static float aspectRatio
        {
            get
            {
                return screenWidth / screenHeight;
            }
        }
        public const float FOV = MathHelper.PiOver4;
        public static float screenWidth { get; private set; }
        public static float screenHeight { get; private set; }

        public Ray ScreenPointToRay(Vector point)
        {
            Vector near = new Vector(point.x, point.y, 0);
            Vector far = new Vector(point.x, point.y, 1);
            Matrix world = Matrix.CreateTranslation(Vector.zero);
            Matrix view = viewMatrix;
            Matrix proj = projectionMatrix;
            GraphicsDevice dev = Program.currentEngine.GraphicsDevice;
            Vector nearPoint = dev.Viewport.Unproject(near, proj, view, world);
            Vector farPoint = dev.Viewport.Unproject(far, proj, view, world);
            Vector direction = (far - near).normalized;
            return new Ray(globalPosition, direction);
        }

        public Vector target
        {
            get
            {
                return globalPosition + forward;
            }
        }

        public Matrix viewMatrix
        {
            get
            {
                return Matrix.CreateLookAt(globalPosition, target, up);
            }
        }
        public Matrix projectionMatrix
        {
            get
            {
                return Matrix.CreatePerspectiveFieldOfView(FOV, aspectRatio, 0.01f, drawDistance);
            }
        }

        public static void SetResolution(float width, float height)
        {
            screenWidth = width;
            screenHeight = height;
        }


		public void MoveAt (Vector at) {
			globalPosition = at * this;
		}

		public Camera (float dist, Vector _position, Vector euler) {
			drawDistance = dist;
			globalPosition = _position;
            globalEulerAngles = euler;
			scale = Vector.one;
		}
	}
}

