using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RGeos.SlimScene.Core;
using CustomVertex;
using SlimDX;
using SlimDX.Direct3D9;
using System.Drawing;

namespace RGeos.AppScene.Renderable
{
    public class Plane : RenderableObject
    {
        public double Span;
        public int Num;
        public Color RgColor { get; set; }
        public Plane(double span, int num, string name)
            : base(name)
        {
            Span = span;
            Num = num;
            RgColor = Color.Silver;
        }

        public override void Initialize(DrawArgs drawArgs)
        {
            this.isInitialized = true;
        }

        public override void Update(DrawArgs drawArgs)
        {
            if (!this.isInitialized)
            {
                this.Initialize(drawArgs);
            }
        }

        public override void Render(DrawArgs drawArgs)
        {
            if (!this.IsOn || !this.isInitialized) return;
            int lines = (int)(Num / 2);
            for (int i = 0; i < lines; i++)
            {
                PositionColored[] axisX = new PositionColored[2];
                float x = (float)(-i * Span);
                float y = (float)(-(lines - 1) * Span);
                float x2 = (float)(-i * Span);
                float y2 = (float)((lines - 1) * Span);
                axisX[0].Position = new Vector3(x, y, 0);

                axisX[0].Color = RgColor.ToArgb();
                axisX[1].Position = new Vector3(x2, y2, 0);
                axisX[1].Color = RgColor.ToArgb();
                drawArgs.Device.DrawUserPrimitives(PrimitiveType.LineStrip, 1, axisX);
            }
            for (int i = 1; i < lines; i++)
            {
                PositionColored[] axisX = new PositionColored[2];
                float x = (float)(i * Span);
                float y = (float)(-(lines - 1) * Span);
                float x2 = (float)(i * Span);
                float y2 = (float)((lines - 1) * Span);
                axisX[0].Position = new Vector3(x, y, 0);

                axisX[0].Color = RgColor.ToArgb();
                axisX[1].Position = new Vector3(x2, y2, 0);
                axisX[1].Color = RgColor.ToArgb();
                drawArgs.Device.DrawUserPrimitives(PrimitiveType.LineStrip, 1, axisX);
            }
            for (int i = 0; i < lines; i++)
            {
                PositionColored[] axisX = new PositionColored[2];
                float y = (float)(i * Span);
                float x = (float)(-(lines - 1) * Span);
                float y2 = (float)(i * Span);
                float x2 = (float)((lines - 1) * Span);
                axisX[0].Position = new Vector3(x, y, 0);

                axisX[0].Color = RgColor.ToArgb();
                axisX[1].Position = new Vector3(x2, y2, 0);
                axisX[1].Color = RgColor.ToArgb();
                drawArgs.Device.DrawUserPrimitives(PrimitiveType.LineStrip, 1, axisX);
            }
            for (int i = 0; i < lines; i++)
            {
                PositionColored[] axisX = new PositionColored[2];
                float y = (float)(-i * Span);
                float x = (float)(-(lines - 1) * Span);
                float y2 = (float)(-i * Span);
                float x2 = (float)((lines - 1) * Span);
                axisX[0].Position = new Vector3(x, y, 0);

                axisX[0].Color = RgColor.ToArgb();
                axisX[1].Position = new Vector3(x2, y2, 0);
                axisX[1].Color = RgColor.ToArgb();
                drawArgs.Device.DrawUserPrimitives(PrimitiveType.LineStrip, 1, axisX);
            }
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            throw new NotImplementedException();
        }
    }
}
