using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace RGeos.SlimScene.Camera
{
    public class Frustum
    {
        public Plane Bottom { get; set; }
        public Plane Far { get; set; }
        public Plane Left { get; set; }
        public Plane Near { get; set; }
        public Plane Right { get; set; }
        public Plane Top { get; set; }

        public Frustum()
        { }
        public bool Contains(BoundingBox bb)
        {
            return true;
        }
        public bool ContainsPoint(Vector3 v)
        {
            return true;
        }
        public bool Intersects(BoundingSphere c)
        {
            return true;
        }
        public bool Intersects(BoundingBox bb)
        {
            return true;
        }
        public override string ToString()
        {
            return "";
        }
        public void Update(Matrix m)
        { }
    }
}
