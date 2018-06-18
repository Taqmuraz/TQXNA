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

namespace TQ.TQ_Engine
{

    public class AnimWorks
    {
        public Renderer rend { get; private set; }

        public Model model
        {
            get
            {
                return rend.model;
            }
        }

        public AnimWorks(Renderer r)
        {
            rend = r;
        }

        public ModelBone ByName(string name)
        {
            return model.Bones[name];
        }
        public ModelBone ByIndex(int index)
        {
            return model.Bones[index];
        }

        /*public void ApplyToBone(ModelBone bone, Vector euler, Vector position, Vector scale)
        {
            Quaternion q = Matrix4x4.EulerToQuaternion(euler);
            Matrix m = Matrix.CreateWorld(position, Vector.forward, Vector.up);
            m = Matrix.CreateFromQuaternion(q) * m;
            bone.Transform = m;
        }*/
    }
}
