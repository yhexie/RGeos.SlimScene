using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D9;

namespace CustomVertex
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PositionTextured
    {
        public Vector3 Position;
        public float Tu;
        public float Tv;
        public static int SizeBytes
        {
            get { return Marshal.SizeOf(typeof(PositionTextured)); }
        }

        public static VertexFormat Format
        {
            get { return VertexFormat.Texture1 | VertexFormat.Position; }
            // get { return VertexFormat.PositionRhw | VertexFormat.Texture0 | VertexFormat.Diffuse; }
        }
    }
}
