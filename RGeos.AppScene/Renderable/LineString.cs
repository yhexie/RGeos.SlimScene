using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using RGeos.SlimScene.Core;
using CustomVertex;
using System.Collections;
using SlimDX.Direct3D9;

namespace RGeos.AppScene.Renderable
{
    public class Line : IGeometry
    {
        public string Name;
        public Vector3 v1;
        public Vector3 v2;
    }
    public class Feature
    {
        public int FID;
        public IGeometry Shape;
    }
    public enum ShapeType
    {
        Point = 0,
        Polyline = 1,
        Polygon = 2
    }
    public delegate void FeaureChanged();
    public class FeatureClass
    {
        public ShapeType ShapeType;
        public List<Feature> Features;
        public FeaureChanged FeaureChanged;
        public FeatureClass()
        {
            Features = new List<Feature>();
        }
        public void AddFeature(Feature feature)
        {
            Features.Add(feature);
            if (FeaureChanged != null)
            {
                FeaureChanged();
            }
        }
    }
    public class RenderLayer : RenderableObject
    {
        FeatureClass mFeatureClass = null;

        internal FeatureClass FeatureClass
        {
            get { return mFeatureClass; }
            set { mFeatureClass = value; }
        }
        Hashtable ShowObjects = new Hashtable();

        public RenderLayer(string name)
            : base(name)
        {
            mFeatureClass = new FeatureClass();
            mFeatureClass.FeaureChanged += new FeaureChanged(mFeaureChanged);
        }
        public void mFeaureChanged()
        {
            this.isInitialized = false;
        }
        public override void Initialize(DrawArgs drawArgs)
        {
            if (mFeatureClass != null && mFeatureClass.Features.Count > 0)
            {
                this.isInitialized = true;
            }

        }

        public override void Update(DrawArgs drawArgs)
        {
            if (!this.isInitialized)
            {
                this.Initialize(drawArgs);
            }
            lock (ShowObjects.SyncRoot)
            {
                ShowObjects.Clear();
            }
            for (int i = 0; i < mFeatureClass.Features.Count; i++)
            {
                if (true)
                {
                    lock (ShowObjects.SyncRoot)
                    {
                        Feature feat = mFeatureClass.Features[i];
                        ShowObjects.Add(feat.FID, feat.Shape);
                    }
                }
            }
        }

        public override void Render(DrawArgs drawArgs)
        {
            if (!this.IsOn || !this.isInitialized) return;
            for (int i = 0; i < ShowObjects.Count; i++)
            {
                Line line = ShowObjects[i] as Line;
                if (line != null)
                {
                    PositionColored[] axisX = new PositionColored[2];
                    axisX[0].Position = line.v1;

                    axisX[0].Color = System.Drawing.Color.Green.ToArgb();
                    axisX[1].Position = line.v2;
                    axisX[1].Color = System.Drawing.Color.Green.ToArgb();
                    drawArgs.Device.DrawUserPrimitives(PrimitiveType.LineStrip, 1, axisX);
                }

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
