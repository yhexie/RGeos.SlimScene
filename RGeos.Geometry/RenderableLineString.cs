using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RGeos.SlimScene.Core;
using SlimDX.Direct3D9;
using SlimDX;

namespace RGeos.Geometry
{
    public class RenderableLineString : RenderableObject
    {
        #region Static Members
        #endregion

        #region Private Members
        double m_distanceAboveSurface = 0;
        Point3d[] m_points = null;
        CustomVertex.PositionColoredTextured[] m_wallVertices = null;

        CustomVertex.PositionColored[] m_topVertices = null;
        CustomVertex.PositionColored[] m_bottomVertices = null;
        CustomVertex.PositionColored[] m_sideVertices = null;

        System.Drawing.Color m_lineColor = System.Drawing.Color.Black;
        float m_verticalExaggeration = 1;
        double m_minimumDisplayAltitude = 0;
        double m_maximumDisplayAltitude = double.MaxValue;
        string m_imageUri = null;
        Texture m_texture = null;
        System.Drawing.Color m_polygonColor = System.Drawing.Color.Black;
        bool m_outline = true;
        float m_lineWidth = 1.0f;
        bool m_extrude = false;
        AltitudeMode m_altitudeMode = AltitudeMode.Absolute;
        long m_numPoints = 0;
        #endregion

        /// <summary>
        /// Boolean indicating whether or not the line needs rebuilding.
        /// </summary>
        public bool NeedsUpdate = true;

        public bool Extrude
        {
            get { return m_extrude; }
            set { m_extrude = value; }
        }

        public AltitudeMode AltitudeMode
        {
            get { return m_altitudeMode; }
            set { m_altitudeMode = value; }
        }

        public System.Drawing.Color LineColor
        {
            get { return m_lineColor; }
            set
            {
                m_lineColor = value;
                NeedsUpdate = true;
            }
        }

        public float LineWidth
        {
            get { return m_lineWidth; }
            set
            {
                m_lineWidth = value;
                NeedsUpdate = true;
            }
        }

        public double DistanceAboveSurface
        {
            get { return m_distanceAboveSurface; }
            set
            {
                m_distanceAboveSurface = value;
                if (m_topVertices != null)
                {
                    NeedsUpdate = true;
                    //UpdateVertices();
                }
            }
        }

        public System.Drawing.Color PolygonColor
        {
            get { return m_polygonColor; }
            set
            {
                m_polygonColor = value;
                if (m_topVertices != null)
                {
                    NeedsUpdate = true;
                    //UpdateVertices();
                }
            }
        }

        public bool Outline
        {
            get { return m_outline; }
            set
            {
                m_outline = value;
                if (m_topVertices != null)
                {
                    NeedsUpdate = true;
                    //UpdateVertices();
                }
            }
        }

        public Point3d[] Points
        {
            get
            {
                // if the array size is correct just return it
                if (m_numPoints == m_points.LongLength)
                    return m_points;

                // return an array the correct size.
                Point3d[] points = new Point3d[m_numPoints];
                for (int i = 0; i < m_numPoints; i++)
                {
                    points[i] = m_points[i];
                }
                return points;
            }
            set
            {
                m_points = value;
                m_numPoints = m_points.LongLength;
                NeedsUpdate = true;
            }
        }

        public long NumPoints
        {
            get { return m_numPoints; }
        }

        public double MinimumDisplayAltitude
        {
            get { return m_minimumDisplayAltitude; }
            set { m_minimumDisplayAltitude = value; }
        }

        public double MaximumDisplayAltitude
        {
            get { return m_maximumDisplayAltitude; }
            set { m_maximumDisplayAltitude = value; }
        }

        public override byte Opacity
        {
            get
            {
                return base.Opacity;
            }
            set
            {
                base.Opacity = value;
                if (m_topVertices != null)
                {
                    UpdateVertices();
                }
            }
        }

        public RenderableLineString(string name, World parentWorld, Point3d[] points, System.Drawing.Color lineColor)
            : base(name, parentWorld)
        {
            m_points = points;
            m_lineColor = lineColor;
            m_polygonColor = lineColor;
            m_numPoints = m_points.LongLength;

            // RenderPriority = WorldWind.Renderable.RenderPriority.LinePaths;
        }

        public RenderableLineString(string name, World parentWorld, Point3d[] points, string imageUri)
            : base(name, parentWorld)
        {
            m_points = points;
            m_imageUri = imageUri;
            m_numPoints = m_points.LongLength;

            // RenderPriority = WorldWind.Renderable.RenderPriority.LinePaths;
        }

        public override void Dispose()
        {
            if (m_texture != null && !m_texture.Disposed)
            {
                m_texture.Dispose();
                m_texture = null;
            }

            if (m_lineString != null)
            {
                m_lineString.Remove = true;
                m_lineString = null;
            }
            NeedsUpdate = true;
        }

        public override void Initialize(DrawArgs drawArgs)
        {
            if (m_points == null)
            {
                isInitialized = true;
                return;
            }

            if (m_imageUri != null)
            {
                //load image
                //if (m_imageUri.ToLower().StartsWith("http://"))
                //{
                //    string savePath = string.Format("{0}\\image", ConfigurationLoader.GetRenderablePathString(this));
                //    System.IO.FileInfo file = new System.IO.FileInfo(savePath);
                //    if (!file.Exists)
                //    {
                //        WorldWind.Net.WebDownload download = new WorldWind.Net.WebDownload(m_imageUri);

                //        if (!file.Directory.Exists)
                //            file.Directory.Create();

                //        download.DownloadFile(file.FullName, WorldWind.Net.DownloadType.Unspecified);
                //    }

                //    m_texture = ImageHelper.LoadTexture(file.FullName);
                //}
                //else
                //{
                //    m_texture = ImageHelper.LoadTexture(m_imageUri);
                //}
            }

            UpdateVertices();

            isInitialized = true;
        }

        /// <summary>
        /// Adds a point to the line at the end of the line.
        /// </summary>
        /// <param name="point">The Point3d object to add.</param>
        public void AddPoint(Point3d point)
        {
            // if the array is too small grow it.
            if (m_numPoints >= m_points.LongLength)
            {
                long growSize = m_points.LongLength / 2;
                if (growSize < 10) growSize = 10;

                Point3d[] points = new Point3d[m_points.LongLength + growSize];

                for (int i = 0; i < m_numPoints; i++)
                {
                    points[i] = m_points[i];
                }
                m_points = points;
            }
            m_points[m_numPoints] = point;
            m_numPoints++;
            NeedsUpdate = true;
        }

        private void UpdateVertices()
        {
            try
            {
                // m_verticalExaggeration = World.Settings.VerticalExaggeration;

                UpdateTexturedVertices();

                if (m_lineString != null && m_outline && m_wallVertices != null && m_wallVertices.Length > m_topVertices.Length)
                {
                    UpdateOutlineVertices();
                }

                NeedsUpdate = false;
            }
            catch (Exception ex)
            {
                Utility.Log.Write(ex);
            }
        }

        private void UpdateOutlineVertices()
        {
            m_bottomVertices = new CustomVertex.PositionColored[m_numPoints];
            m_sideVertices = new CustomVertex.PositionColored[m_numPoints * 2];

            for (int i = 0; i < m_numPoints; i++)
            {
                m_sideVertices[2 * i] = m_topVertices[i];

                Vector3 xyzVertex = new Vector3(
                    m_wallVertices[2 * i + 1].Position.X,
                    m_wallVertices[2 * i + 1].Position.Y,
                    m_wallVertices[2 * i + 1].Position.Z);

                m_bottomVertices[i].Position.X = xyzVertex.X;
                m_bottomVertices[i].Position.Y = xyzVertex.Y;
                m_bottomVertices[i].Position.Z = xyzVertex.Z;
                m_bottomVertices[i].Color = m_lineColor.ToArgb();

                m_sideVertices[2 * i + 1] = m_bottomVertices[i];
            }
        }

        LineString m_lineString = null;
        private void UpdateTexturedVertices()
        {
            if (m_altitudeMode == AltitudeMode.ClampedToGround)
            {
                if (m_lineString != null)
                {
                    m_lineString.Remove = true;
                    m_lineString = null;
                }

                m_lineString = new LineString();
                m_lineString.Coordinates = Points;
                m_lineString.Color = LineColor;
                m_lineString.LineWidth = LineWidth;
                m_lineString.ParentRenderable = this;
                // this.World.ProjectedVectorRenderer.Add(m_lineString);

                if (m_wallVertices != null)
                    m_wallVertices = null;

                return;
            }

            if (m_extrude || m_altitudeMode == AltitudeMode.RelativeToGround)
            {
                m_wallVertices = new CustomVertex.PositionColoredTextured[m_numPoints * 2];
            }

            float textureCoordIncrement = 1.0f / (float)(m_numPoints - 1);
            // m_verticalExaggeration = World.Settings.VerticalExaggeration;
            int vertexColor = m_polygonColor.ToArgb();

            m_topVertices = new CustomVertex.PositionColored[m_numPoints];

            for (int i = 0; i < m_numPoints; i++)
            {
                double terrainHeight = 0;


                Vector3 xyzVertex = new Vector3((float)m_points[i].X, (float)m_points[i].Y, (float)m_points[i].Z);

                m_topVertices[i].Position.X = xyzVertex.X;
                m_topVertices[i].Position.Y = xyzVertex.Y;
                m_topVertices[i].Position.Z = xyzVertex.Z;
                m_topVertices[i].Color = m_lineColor.ToArgb();

                if (m_extrude || m_altitudeMode == AltitudeMode.RelativeToGround)
                {
                    m_wallVertices[2 * i].Position.X = xyzVertex.X;
                    m_wallVertices[2 * i].Position.Y = xyzVertex.Y;
                    m_wallVertices[2 * i].Position.Z = xyzVertex.Z;
                    m_wallVertices[2 * i].Color = vertexColor;
                    m_wallVertices[2 * i].Tu = i * textureCoordIncrement;
                    m_wallVertices[2 * i].Tv = 1.0f;

                    m_wallVertices[2 * i + 1].Position.X = xyzVertex.X;
                    m_wallVertices[2 * i + 1].Position.Y = xyzVertex.Y;
                    m_wallVertices[2 * i + 1].Position.Z = xyzVertex.Z;
                    m_wallVertices[2 * i + 1].Color = vertexColor;
                    m_wallVertices[2 * i + 1].Tu = i * textureCoordIncrement;
                    m_wallVertices[2 * i + 1].Tv = 0.0f;
                }
            }
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return false;
        }

        public override void Update(DrawArgs drawArgs)
        {
            if (drawArgs.WorldCamera.Distance >= m_minimumDisplayAltitude && drawArgs.WorldCamera.Distance <= m_maximumDisplayAltitude)
            {
                if (!isInitialized)
                    Initialize(drawArgs);

                if (NeedsUpdate)
                    UpdateVertices();
            }

        }

        public override void Render(DrawArgs drawArgs)
        {
            if (!isInitialized || drawArgs.WorldCamera.Distance < m_minimumDisplayAltitude || drawArgs.WorldCamera.Distance > m_maximumDisplayAltitude)
            {
                return;
            }

            try
            {
                if (m_lineString != null)
                    return;

                int currentCull = drawArgs.Device.GetRenderState(RenderState.CullMode);
                drawArgs.Device.SetRenderState(RenderState.CullMode, Cull.None);

                if (m_wallVertices != null)
                {
                    drawArgs.Device.SetRenderState(RenderState.ZEnable, true);

                    if (m_texture != null && !m_texture.Disposed)
                    {
                        drawArgs.Device.SetTexture(0, m_texture);
                        drawArgs.Device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Modulate);
                        drawArgs.Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Add);
                        drawArgs.Device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Texture);
                    }
                    else
                    {
                        drawArgs.Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Disable);
                    }

                    drawArgs.Device.VertexFormat = CustomVertex.PositionColoredTextured.Format;

                    drawArgs.Device.DrawUserPrimitives(PrimitiveType.TriangleStrip, m_wallVertices.Length - 2, m_wallVertices);

                    if (m_outline)
                    {

                        drawArgs.Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Disable);
                        drawArgs.Device.VertexFormat = CustomVertex.PositionColored.Format;
                        drawArgs.Device.DrawUserPrimitives(PrimitiveType.LineStrip, m_topVertices.Length - 1, m_topVertices);

                        if (m_bottomVertices != null)
                            drawArgs.Device.DrawUserPrimitives(PrimitiveType.LineStrip, m_bottomVertices.Length - 1, m_bottomVertices);

                        if (m_sideVertices != null)
                            drawArgs.Device.DrawUserPrimitives(PrimitiveType.LineList, m_sideVertices.Length / 2, m_sideVertices);

                    }
                }
                else
                {
                    drawArgs.Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Disable);
                    drawArgs.Device.VertexFormat = CustomVertex.PositionColored.Format;
                    drawArgs.Device.DrawUserPrimitives(PrimitiveType.LineStrip, m_topVertices.Length - 1, m_topVertices);
                }

                drawArgs.Device.SetTransform(TransformState.World, drawArgs.WorldCamera.WorldMatrix);
                drawArgs.Device.SetRenderState(RenderState.CullMode, currentCull);
            }
            catch//(Exception ex)
            {
                //Utility.Log.Write(ex);
            }
        }
    }
}
