using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RGeos.SlimScene.Core;
using SlimDX;
using CustomVertex;
using SlimDX.Direct3D9;

namespace RGeos.AppScene.Renderable
{
    public class IGeometry
    {
    }
    //要素渲染包装类
    public interface IRenderableFeature : IDisposable
    {
        Feature Feature { get; set; }
        BoundingBox BoundingBox { get; set; }
        void Initialize(DrawArgs drawArgs);
        void Update(DrawArgs drawArgs);
        void Render(DrawArgs drawArgs);
        void Dispose();
    }
    public class RenderableFeature : IRenderableFeature
    {
        private Feature mFeature;

        public Feature Feature
        {
            get { return mFeature; }
            set { mFeature = value; }
        }
        public BoundingBox BoundingBox { get; set; }
        public RenderableFeature(Feature geo)
        {
            this.mFeature = geo;
        }
        public void Initialize(DrawArgs drawArgs)
        {


        }
        public void Update(DrawArgs drawArgs)
        {

        }

        public void Render(DrawArgs drawArgs)
        {
            if (mFeature.Shape is Line)
            {
                Line line = mFeature.Shape as Line;
                PositionColored[] axisX = new PositionColored[2];
                axisX[0].Position = line.v1;

                axisX[0].Color = System.Drawing.Color.Green.ToArgb();
                axisX[1].Position = line.v2;
                axisX[1].Color = System.Drawing.Color.Green.ToArgb();
                drawArgs.Device.DrawUserPrimitives(PrimitiveType.LineStrip, 1, axisX);
            }

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
