using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;

namespace RGeos.SlimScene.Core
{
    public class Arcball
    {
        private const float Epsilon = 1.0e-5f;

        private Vector3 StVec; //Saved click vector
        private Vector3 EnVec; //Saved drag vector
        private float adjustWidth; //Mouse bounds width
        private float adjustHeight; //Mouse bounds height

        public Arcball(float NewWidth, float NewHeight)
        {
            StVec = new Vector3();
            EnVec = new Vector3();
            SetBounds(NewWidth, NewHeight);
        }

        private void MapToSphere(Point point, ref Vector3 vector)
        {
            PointF tempPoint = new PointF(point.X, point.Y);

            //Adjust point coords and scale down to range of [-1 ... 1]
            tempPoint.X = (tempPoint.X * this.adjustWidth) - 1.0f;
            tempPoint.Y = 1.0f - (tempPoint.Y * this.adjustHeight);

            //Compute square of the length of the vector from this point to the center
            float length = (tempPoint.X * tempPoint.X) + (tempPoint.Y * tempPoint.Y);

            //If the point is mapped outside the sphere... (length > radius squared)
            if (length > 1.0f)
            {
                //Compute a normalizing factor (radius / sqrt(length))
                float norm = (float)(1.0 / Math.Sqrt(length));

                //Return the "normalized" vector, a point on the sphere
                vector.X = tempPoint.X * norm;
                vector.Y = tempPoint.Y * norm;
                vector.Z = 0.0f;
            }
            //Else it's inside
            else
            {
                //Return a vector to a point mapped inside the sphere sqrt(radius squared - length)
                vector.X = tempPoint.X;
                vector.Y = tempPoint.Y;
                vector.Z = (float)System.Math.Sqrt(1.0f - length);
            }
        }

        public void SetBounds(float NewWidth, float NewHeight)
        {
            //Set adjustment factor for width/height
            adjustWidth = 1.0f / ((NewWidth - 1.0f) * 1f);
            adjustHeight = 1.0f / ((NewHeight - 1.0f) * 1f);
        }

        //Mouse down
        public virtual void click(Point NewPt)
        {
            MapToSphere(NewPt, ref StVec);
        }

        //Mouse drag, calculate rotation
        public void drag(Point NewPt, ref Quat4f NewRot)
        {
            //Map the point to the sphere
            this.MapToSphere(NewPt, ref EnVec);

            //Return the quaternion equivalent to the rotation
            if (NewRot != null)
            {
                Vector3 Perp = new Vector3();

                //Compute the vector perpendicular to the begin and end vectors
                Perp = Vector3.Cross(StVec, EnVec);

                //Compute the length of the perpendicular vector
                if (Perp.Length() > Epsilon)
                //if its non-zero
                {
                    //We're ok, so return the perpendicular vector as the transform after all
                    NewRot.x = Perp.X;
                    NewRot.y = Perp.Y;
                    NewRot.z = Perp.Z;
                    //In the quaternion values, w is cosine (theta / 2), where theta is the rotation angle
                    NewRot.w = Vector3.Dot(StVec, EnVec);
                }
                //if it is zero
                else
                {
                    //The begin and end vectors coincide, so return an identity transform
                    NewRot.x = NewRot.y = NewRot.z = NewRot.w = 0.0f;
                }
            }
        }
    }
    public class Quat4f
    {
        public float x, y, z, w;
    }
}
