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
        List<IRenderableFeature> mRenderGeometries = null;
        public RenderLayer(string name)
            : base(name)
        {
            mFeatureClass = new FeatureClass();
            mRenderGeometries = new List<IRenderableFeature>();
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
                for (int i = 0; i < mFeatureClass.Features.Count; i++)
                {
                    Feature feat = mFeatureClass.Features[i];
                    IGeometry geo = feat.Shape;
                    IRenderableFeature renderGeo = new RenderableFeature(feat);
                    mRenderGeometries.Add(renderGeo);
                }
            }
            this.isInitialized = true;
        }
        bool AddingNew { get; set; }
        public override void Update(DrawArgs drawArgs)
        {
            if (!this.isInitialized)
            {
                this.Initialize(drawArgs);
            }

            //移除不可见到的要素
            RemoveInvisibleGeometries(drawArgs);

            for (int i = 0; i < mRenderGeometries.Count; i++)
            {
                IRenderableFeature tmpGeo = mRenderGeometries[i];
                IRenderableFeature renderGeo = ShowObjects[tmpGeo.Feature.FID] as IRenderableFeature;
                if (renderGeo != null)
                {
                    renderGeo.Update(drawArgs);
                    continue;
                }
                if (drawArgs.WorldCamera.ViewFrustum.Intersects(mRenderGeometries[i].BoundingBox))
                {
                    lock (ShowObjects.SyncRoot)
                    {
                        ShowObjects.Add(tmpGeo.Feature.FID, mRenderGeometries[i]);
                        tmpGeo.Update(drawArgs);
                    }
                }
            }
        }
        protected void RemoveInvisibleGeometries(DrawArgs drawArgs)
        {
            ArrayList deletionList = new ArrayList();

            lock (ShowObjects.SyncRoot)
            {
                //不在裁剪体内的加入删除集合
                foreach (int key in ShowObjects.Keys)
                {
                    IRenderableFeature qt = (IRenderableFeature)ShowObjects[key];
                    if (!drawArgs.WorldCamera.ViewFrustum.Intersects(qt.BoundingBox))
                        deletionList.Add(key);
                }
                //从总体中删除
                foreach (int deleteThis in deletionList)
                {
                    IRenderableFeature qt = (IRenderableFeature)ShowObjects[deleteThis];
                    if (qt != null)
                    {
                        ShowObjects.Remove(deleteThis);
                        qt.Dispose();
                    }
                }
            }
        }
        public override void Render(DrawArgs drawArgs)
        {
            if (!this.IsOn || !this.isInitialized) return;

            foreach (IRenderableFeature qt in ShowObjects.Values)
                qt.Render(drawArgs);

            for (int i = 0; i < ShowObjects.Count; i++)
            {
                Line line = ShowObjects[i] as Line;
                if (line != null)
                {

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
