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
    public struct PositionColored
    {
        public Vector3 Position;
        public int Color;
        public static VertexFormat Format
        {
            get
            {
                return VertexFormat.Diffuse | VertexFormat.Position;

            }

        }
    }
}
