using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using SlimDX.Direct3D9;
using SlimDX;

namespace CustomVertex
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TransformedTextured
    {
        public static readonly int Stride;
        public static readonly VertexFormat Format;
        public Vector4 Position;
        public float Tu;
        public float Tv;
        static TransformedTextured()
        {
            Stride = Marshal.SizeOf(typeof(TransformedTextured));
            Format = VertexFormat.Texture1 | VertexFormat.PositionRhw;
        }


    }

 

}
