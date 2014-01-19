using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace RGeos.SlimScene.Core
{
    class MatrixArcBall
    {
        private float[,] M;
        private float scl = 1.0f;
        private Vector3 pan = new Vector3();

        public MatrixArcBall()
        {
            SetIdentity();
        }

        public void get_Renamed(float[] dest)
        {
            int k = 0;
            for (int i = 0; i <= 3; i++)
                for (int j = 0; j <= 3; j++)
                {
                    dest[k] = this.M[j, i];
                    k++;
                }
        }

        public Matrix get_Renamed()
        {
            Matrix dest = Matrix.Identity;
            int k = 0;
            for (int i = 0; i <= 3; i++)
                for (int j = 0; j <= 3; j++)
                {
                    dest[i,j] = this.M[j, i];
                    k++;
                }
            return dest;
        }

        public void SetIdentity()
        {
            this.M = new float[4, 4]; // set to zero
            for (int i = 0; i <= 3; i++) this.M[i, i] = 1.0f;
        }

        public void set_Renamed(MatrixArcBall m1)
        {
            this.M = m1.M;
        }

        public void MatrixMultiply(MatrixArcBall m1, MatrixArcBall m2)
        {
            float[] MulMat = new float[16];
            float elMat = 0.0f;
            int k = 0;

            for (int i = 0; i <= 3; i++)
                for (int j = 0; j <= 3; j++)
                {
                    for (int l = 0; l <= 3; l++) elMat += m1.M[i, l] * m2.M[l, j];
                    MulMat[k] = elMat;
                    elMat = 0.0f;
                    k++;
                }

            k = 0;
            for (int i = 0; i <= 3; i++)
                for (int j = 0; j <= 3; j++)
                {
                    m1.M[i, j] = MulMat[k];
                    k++;
                }
        }

        public Quat4f Rotation
        {
            set
            {
                float n, s;
                float xs, ys, zs;
                float wx, wy, wz;
                float xx, xy, xz;
                float yy, yz, zz;

                M = new float[4, 4];

                n = (value.x * value.x) + (value.y * value.y) + (value.z * value.z) + (value.w * value.w);
                s = (n > 0.0f) ? 2.0f / n : 0.0f;

                xs = value.x * s;
                ys = value.y * s;
                zs = value.z * s;
                wx = value.w * xs;
                wy = value.w * ys;
                wz = value.w * zs;
                xx = value.x * xs;
                xy = value.x * ys;
                xz = value.x * zs;
                yy = value.y * ys;
                yz = value.y * zs;
                zz = value.z * zs;

                // rotation
                M[0, 0] = 1.0f - (yy + zz);
                M[0, 1] = xy - wz;
                M[0, 2] = xz + wy;

                M[1, 0] = xy + wz;
                M[1, 1] = 1.0f - (xx + zz);
                M[1, 2] = yz - wx;

                M[2, 0] = xz - wy;
                M[2, 1] = yz + wx;
                M[2, 2] = 1.0f - (xx + yy);

                M[3, 3] = 1.0f;

                // translation (pan)
                M[0, 3] = pan.X;
                M[1, 3] = pan.Y;

                // scale (zoom)
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        M[i, j] *= scl;


            }
        }

        public float Scale
        {
            set { scl = value; }

        }

        public Vector3 Pan
        {
            set { pan = value; }

        }

    }
}
