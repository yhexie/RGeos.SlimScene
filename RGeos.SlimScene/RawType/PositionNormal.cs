using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using System.Runtime.InteropServices;

namespace CustomVertex
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PositionNormal
    {
        public static readonly int Stride;
        public static readonly VertexFormat Format;
        public Vector3 Position;
        public Vector3 Normal;

        static PositionNormal()
        {
            Stride = Marshal.SizeOf(typeof(PositionNormal));
            Format = VertexFormat.PositionNormal;
        }
    }
}
