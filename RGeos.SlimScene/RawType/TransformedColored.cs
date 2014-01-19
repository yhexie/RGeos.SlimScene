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
    public struct TransformedColored
    {
        public static readonly int Stride;
        public static readonly VertexFormat Format;
        public Vector4 Position;
        public int Color;
        static TransformedColored()
        {
            Stride = Marshal.SizeOf(typeof(TransformedColored));
            Format = VertexFormat.Diffuse | VertexFormat.PositionRhw;
        }



    }



}
