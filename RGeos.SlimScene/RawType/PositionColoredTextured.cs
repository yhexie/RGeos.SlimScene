using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Runtime.InteropServices;
using SlimDX.Direct3D9;

namespace CustomVertex
{
    public struct PositionColoredTextured
    {
        public Vector3 Position;
        public int Color;
        public float Tu;
        public float Tv;
        public static int SizeBytes
        {
            get { return Marshal.SizeOf(typeof(PositionColoredTextured)); }
        }
        public static VertexFormat Format
        {
            get
            {
                return VertexFormat.Diffuse | VertexFormat.Position | VertexFormat.Texture1;

            }

        }
    }
}
