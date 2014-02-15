using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;

namespace RGeos.SlimScene.Core
{
    public class World : RenderableObject
    {
        RenderableObjectList mRenderableObjects;
        public RenderableObjectList RenderableObjects
        {
            get
            {
                return this.mRenderableObjects;
            }
            set
            {
                this.mRenderableObjects = value;
            }
        }

        Scene mScene;

        public Scene Scene
        {
            get { return mScene; }
            set { mScene = value; }
        }

        public World(string str)
            : base(str)
        {
            this.mRenderableObjects = new RenderableObjectList(this.Name);
            mScene = new Scene("地理渲染对象");
        }

        public override void Initialize(DrawArgs drawArgs)
        {
            try
            {
                if (this.isInitialized)
                    return;

                this.RenderableObjects.Initialize(drawArgs);
                mScene.Initialize(drawArgs);
            }
            catch (Exception caught)
            {
                Log.DebugWrite(caught);
            }
            finally
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

            if (this.RenderableObjects != null)
            {
                this.RenderableObjects.Update(drawArgs);
            }
            if (Scene != null)
            {
                Scene.Update(drawArgs);
            }
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return this.mRenderableObjects.PerformSelectionAction(drawArgs);
        }

        public override void Render(DrawArgs drawArgs)
        {
            try
            {
                CustomVertex.PositionColored[] vertices2 = new
 CustomVertex.PositionColored[3];//定义顶点 
                vertices2[0].Position = new Vector3(0.0f, 0.0f, 0.0f);
                vertices2[0].Color = Color.Red.ToArgb();
                vertices2[1].Position = new Vector3(0.0f, 10.0f, 0.0f);
                vertices2[1].Color = Color.Green.ToArgb();
                vertices2[2].Position = new Vector3(10.0f, 0f, 0.0f);
                vertices2[2].Color = Color.Yellow.ToArgb();
                drawArgs.Device.VertexFormat = CustomVertex.PositionColored.Format;
                drawArgs.Device.DrawUserPrimitives(PrimitiveType.TriangleList, 1, vertices2);

                Render(RenderableObjects, RenderPriority.TerrainMappedImages, drawArgs);

                //if (m_projectedVectorRenderer != null)
                //    m_projectedVectorRenderer.Render(drawArgs);


                //渲染Placenames
                Render(RenderableObjects, RenderPriority.Placenames, drawArgs);

                //绘制自定义渲染对象
                Render(RenderableObjects, RenderPriority.Custom, drawArgs);

                //Render(Scene, RenderPriority.TerrainMappedImages, drawArgs);
                ////渲染Placenames
                //Render(Scene, RenderPriority.Placenames, drawArgs);

                Render(Scene, RenderPriority.Custom, drawArgs);

                //绘制坐标轴
                if (ShowPlanetAxis)
                    this.DrawAxis(drawArgs);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private void Render(RenderableObject renderable, RenderPriority priority, DrawArgs drawArgs)
        {
            if (!renderable.IsOn || (renderable.Name != null && renderable.Name.Equals("Starfield")))
                return;
            try
            {
                if (renderable is RenderableObjectList)
                {
                    RenderableObjectList rol = (RenderableObjectList)renderable;
                    for (int i = 0; i < rol.ChildObjects.Count; i++)
                    {
                        Render((RenderableObject)rol.ChildObjects[i], priority, drawArgs);
                    }
                }
                // hack at the moment
                else if (priority == RenderPriority.TerrainMappedImages)
                {
                    if (renderable.RenderPriority == RenderPriority.SurfaceImages || renderable.RenderPriority == RenderPriority.TerrainMappedImages)
                    {
                        renderable.Render(drawArgs);
                    }
                }
                else if (renderable.RenderPriority == priority)
                {
                    renderable.Render(drawArgs);
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private void DrawAxis(DrawArgs drawArgs)
        {
            drawArgs.Device.VertexFormat = CustomVertex.PositionColored.Format;
            drawArgs.Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Disable);
            CustomVertex.PositionColored[] axisX = new CustomVertex.PositionColored[2];

            axisX[0].Position = new Vector3(0, 0, 0);

            axisX[0].Color = System.Drawing.Color.Red.ToArgb();
            axisX[1].Position = new Vector3(10, 0, 0);
            axisX[1].Color = System.Drawing.Color.Red.ToArgb();
            drawArgs.Device.DrawUserPrimitives(PrimitiveType.LineStrip, 1, axisX);
            CustomVertex.PositionColored[] axisY = new CustomVertex.PositionColored[2];

            axisY[0].Position = new Vector3(0, 0, 0);
            axisY[0].Color = System.Drawing.Color.Green.ToArgb();
            axisY[1].Position = new Vector3(0, 10, 0);
            axisY[1].Color = System.Drawing.Color.Green.ToArgb();
            drawArgs.Device.DrawUserPrimitives(PrimitiveType.LineStrip, 1, axisY);

            CustomVertex.PositionColored[] axisZ = new CustomVertex.PositionColored[2];

            axisZ[0].Position = new Vector3(0, 0, 0);
            axisZ[0].Color = System.Drawing.Color.Yellow.ToArgb();
            axisZ[1].Position = new Vector3(0, 0, 10);
            axisZ[1].Color = System.Drawing.Color.Yellow.ToArgb();
            drawArgs.Device.DrawUserPrimitives(PrimitiveType.LineStrip, 1, axisZ);
            //drawArgs.Device.Transform.World = drawArgs.WorldCamera.mWorldMatrix;

        }

        public override void Dispose()
        {
            if (this.RenderableObjects != null)
            {
                this.RenderableObjects.Dispose();
                this.RenderableObjects = null;
            }
            if (mScene!=null)
            {
                mScene.Dispose();
                mScene = null;
            }

        }

        public bool ShowPlanetAxis { get; set; }


    }
}
