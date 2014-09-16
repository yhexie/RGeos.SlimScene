using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using RGeos.SlimScene.Core;
using SlimDX;
using SlimDX.Direct3D9;

namespace RGeos.SlimScene.Renderable
{
    /// <summary>
    /// 此示例主要演示如何构造一个mesh网格,以及实现碰撞检测
    /// </summary>
    public class Earth : RenderableObject, ILayer
    {
        public delegate void IsSelected(float dd, Vector3 vec);
        public IsSelected Selected;
        #region 私有变量
        private Vector3 m_center;//球体球心(模型坐标)
        private float m_radius;//球体半径
        private short m_slices;//球体在水平方向的分块数目
        private short m_stacks;//球体在竖直方向的分块数目
        private CustomVertex.PositionTextured[] vertices;//定义球体网格顶点
        private short[] indices;//定义球体网格中三角形索引
        private Mesh mesh;//球体mesh网格
        public string texturePath = @"data\\Earth.jpg";//定义贴图路径 
        private Texture texture;//定义贴图变量 
        private Material material;//定义材质变量 
        #endregion

        /// <summary>
        /// 构造啊函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="param">模型参数</param>
        /// <param name="radius">球体半径</param>
        /// <param name="slices">球体在水平方向的分块数目</param>
        /// <param name="stacks">球体在竖直方向的分块数目</param>
        public Earth(string name, float radius, short slices, short stacks)
            : base(name)
        {
            this.m_radius = radius;
            this.m_slices = slices;
            this.m_stacks = stacks;
            this.isSelectable = true;
        }

        #region 构造球体
        /// <summary>
        /// 计算顶点
        /// </summary>
        /// <remarks>
        /// 球体上任意一点坐标可以通过球形坐标来表示(r半径，theta垂直角，alpha水平角)
        /// X=r*sin(theta)*cos(alpha);
        /// Y=r*cos(theta);
        /// Z=r*sin(theta)*sin(alpha);
        /// </remarks>
        private void ComputeVertexs()
        {
            vertices = new CustomVertex.PositionTextured[(m_stacks + 1) * (m_slices + 1)];
            float theta = (float)Math.PI / m_stacks;
            float alpha = 2 * (float)Math.PI / m_slices;
            for (int i = 0; i < m_slices + 1; i++)
            {
                for (int j = 0; j < m_stacks + 1; j++)
                {
                    Vector3 pt = new Vector3();
                    pt.X = m_center.X + m_radius * (float)Math.Sin(i * theta) * (float)Math.Cos(j * alpha);
                    
                    pt.Y = m_center.Z + m_radius * (float)Math.Sin(i * theta) * (float)Math.Sin(j * alpha);
                    pt.Z = m_center.Y + m_radius * (float)Math.Cos(i * theta);
                    vertices[j + i * (m_stacks + 1)].Position = pt;
                    vertices[j + i * (m_stacks + 1)].Tu = (float)j / (m_stacks);
                    vertices[j + i * (m_stacks + 1)].Tv = (float)i / (m_slices);
                    // vertices[j + i * (m_stacks + 1)].Color = Color.FromArgb(200, Color.Blue).ToArgb();
                }
            }
        }

        /// <summary>
        /// 计算索引
        /// </summary>
        private void ComputeIndices()
        {
            indices = new short[6 * m_stacks * m_slices];
            for (short i = 0; i < m_slices; i++)
            {
                for (short j = 0; j < m_stacks; j++)
                {
                    indices[6 * (j + i * m_stacks)] = (short)(j + i * (m_stacks + 1));
                    indices[6 * (j + i * m_stacks) + 1] = (short)(j + i * (m_stacks + 1) + 1);
                    indices[6 * (j + i * m_stacks) + 2] = (short)(j + (i + 1) * (m_stacks + 1));
                    indices[6 * (j + i * m_stacks) + 3] = (short)(j + i * (m_stacks + 1) + 1);
                    indices[6 * (j + i * m_stacks) + 4] = (short)(j + (i + 1) * (m_stacks + 1) + 1);
                    indices[6 * (j + i * m_stacks) + 5] = (short)(j + (i + 1) * (m_stacks + 1));
                }
            }
        }

        #endregion

        #region Renderable
        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="drawArgs">渲染参数</param>
        public override void Initialize(DrawArgs drawArgs)
        {
            //球体球心
            this.m_center = new Vector3();
            m_center.X = 0;
            m_center.Y = 0;
            m_center.Z = 0;
            LoadTexturesAndMaterials(drawArgs);//导入贴图和材质 
            ComputeVertexs();//计算顶点
            ComputeIndices();//计算索引

            //构造mesh
            mesh = new Mesh(drawArgs.Device, indices.Length / 3, vertices.Length, MeshFlags.Managed, CustomVertex.PositionTextured.Format);

            //顶点缓冲
            DataStream vs = mesh.LockVertexBuffer(LockFlags.None);
            vs.WriteRange(vertices);
            mesh.UnlockVertexBuffer();
            vs.Dispose();

            //索引缓冲
            DataStream ids = mesh.LockIndexBuffer(LockFlags.None);
            ids.WriteRange(indices);
            mesh.UnlockIndexBuffer();
            ids.Dispose();

            this.isInitialized = true;
        }
        float ang = 0.0f;
        /// <summary>
        /// 渲染对象
        /// </summary>
        /// <param name="drawArgs">渲染参数</param>
        public override void Render(DrawArgs drawArgs)
        {
            if (!this.IsOn || !this.isInitialized) return;
            //获取当前世界变换
            Matrix world = drawArgs.Device.GetTransform(TransformState.World);
            //获取当前顶点格式
            VertexFormat format = drawArgs.Device.VertexFormat;
            //获取当前的Z缓冲方式
            int zEnable = drawArgs.Device.GetRenderState(RenderState.ZEnable);
            //获取纹理状态
            int colorOper = drawArgs.Device.GetTextureStageState(0, TextureStage.ColorOperation);

            try
            {
                //获取当前转换矩阵，主要用于选择操作时，顶点的转换
                //Matrix world2 =Matrix.Identity;
                //ang -= 0.0003f;
                //world2=Matrix.RotationY(ang);
                //drawArgs.Device.SetTransform(TransformState.World, world2 * world);

                drawArgs.Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
                drawArgs.Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
                drawArgs.Device.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Diffuse);
                drawArgs.Device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Disable);

                //设置顶点格式
                drawArgs.Device.VertexFormat = CustomVertex.PositionTextured.Format;
                //设置Z缓冲
                drawArgs.Device.SetRenderState(RenderState.ZEnable, 1);
                //设置纹理状态，此处使用纹理
                drawArgs.Device.SetTexture(0, texture);//设置贴图 
                //绘制mesh网格
                mesh.DrawSubset(0);
            }
            catch (Exception e)
            {
                Utility.Log.Write(e);
            }
            finally
            {
                drawArgs.Device.SetTransform(TransformState.World, world);
                drawArgs.Device.VertexFormat = format;
                drawArgs.Device.SetRenderState(RenderState.ZEnable, zEnable);
                drawArgs.Device.SetTextureStageState(0, TextureStage.ColorOperation, colorOper);
            }
            if (disposing)
            {
                Dispose();
                disposing = false;
            }
        }
        public bool disposing = false;
        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="drawArgs">渲染参数</param>
        public override void Update(DrawArgs drawArgs)
        {
            if (!this.isInitialized)
            {
                this.Initialize(drawArgs);
            }
        }

        /// <summary>
        /// 执行选择操作
        /// </summary>
        /// <param name="X">点选X坐标</param>
        /// <param name="Y">点选Y坐标</param>
        /// <param name="drawArgs">渲染参数</param>
        /// <returns>选择返回True,否则返回False</returns>
        public bool PerformSelectionAction(int X, int Y, DrawArgs drawArgs)
        {
            if (!this.isInitialized) return false;
            Vector3 v1 = new Vector3();
            v1.X = X;
            v1.Y = Y;
            v1.Z = 0;
            Vector3 v2 = new Vector3();
            v2.X = X;
            v2.Y = Y;
            v2.Z = 1;
            drawArgs.WorldCamera.ComputeMatrix(drawArgs.Device);
            //将屏幕坐标装换为世界坐标，构造一个射线
            Vector3 rayPos = drawArgs.WorldCamera.UnProject(v1);
            Vector3 rayDir = drawArgs.WorldCamera.UnProject(v2) - rayPos;
            //判断模型是否与射线相交
            bool result = this.IntersectWithRay(rayPos, rayDir, drawArgs);
            //if (result)
            //    drawArgs.Selection.Add(this);
            return result;
        }

        /// <summary>
        /// 判断模型是否与射线相交
        /// </summary>
        /// <param name="rayPos">射线原点</param>
        /// <param name="rayDir">射线方向</param>
        /// <param name="drawArgs">绘制参数</param>
        /// <returns>如果相交返回True,否则返回False</returns>
        public bool IntersectWithRay(Vector3 rayPos, Vector3 rayDir, DrawArgs drawArgs)
        {
            if (!this.IsOn || !this.isInitialized || !this.isSelectable) return false;
            bool isSelected = false;
            try
            {
                //选中距离
                float dis;
                IntersectInformation[] insertInfo;

                //构造一条基于模型本体坐标系的射线，用于判断射线是否与模型相交
                Matrix invert = Matrix.Invert(drawArgs.WorldCamera.WorldMatrix);
                Vector3 rayPos1 = Vector3.TransformCoordinate(rayPos, invert);
                Vector3 rayPos2 = rayPos + rayDir;
                rayPos2 = Vector3.TransformCoordinate(rayPos2, invert);
                Vector3 rayDir1 = rayPos2 - rayPos1;
                Ray ray1 = new Ray(rayPos1, rayDir1);
                int faces;
                isSelected = mesh.Intersects(ray1, out dis, out faces, out insertInfo);
                this.m_selectedMinDistance = insertInfo[0].Distance;

                short[] intersectedIndices = new short[3];
                //short[] indices = (short[])mesh.LockIndexBuffer(typeof(short), LockFlags.ReadOnly, mesh.FaceCount * 3);
                DataStream ids = mesh.LockIndexBuffer(LockFlags.ReadOnly);
                short[] indices = new short[mesh.FaceCount * 3];
                ids.ReadRange(indices, 0, mesh.FaceCount * 3);
                Array.Copy(indices, insertInfo[0].FaceIndex * 3, intersectedIndices, 0, 3);
                mesh.UnlockIndexBuffer();

                // create an array to hold the vertices for the intersected face
                CustomVertex.PositionTextured[] IntersectedVertices = new CustomVertex.PositionTextured[3];

                // extract vertex data from mesh, using our indices we obtained earlier
                //CustomVertex.PositionTextured[] meshVertices = (CustomVertex.PositionTextured[])mesh.LockVertexBuffer(typeof(CustomVertex.PositionTextured), LockFlags.ReadOnly, mesh.VertexCount);
                CustomVertex.PositionTextured[] meshVertices = new CustomVertex.PositionTextured[mesh.VertexCount];

                DataStream vs = mesh.LockVertexBuffer(LockFlags.ReadOnly);
                vs.ReadRange(meshVertices, 0, mesh.VertexCount);
                //三个顶点
                IntersectedVertices[0] = meshVertices[intersectedIndices[0]];
                IntersectedVertices[1] = meshVertices[intersectedIndices[1]];
                IntersectedVertices[2] = meshVertices[intersectedIndices[2]];
                mesh.UnlockVertexBuffer();
                //2.求交点
                //三角形三个点的向量。
                Vector3 v1 = new Vector3(IntersectedVertices[0].Position.X, IntersectedVertices[0].Position.Y, IntersectedVertices[0].Position.Z);
                Vector3 v2 = new Vector3(IntersectedVertices[1].Position.X, IntersectedVertices[1].Position.Y, IntersectedVertices[1].Position.Z);
                Vector3 v3 = new Vector3(IntersectedVertices[2].Position.X, IntersectedVertices[2].Position.Y, IntersectedVertices[2].Position.Z);

                //交点 
                pickedPosition = v1 + insertInfo[0].V * (v3 - v1) + insertInfo[0].U * (v2 - v1);

            }
            catch (Exception caught)
            {
                Utility.Log.Write(caught);
            }
            return isSelected;
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        public override void Dispose()
        {
            if (this.mesh != null)
            {
                this.mesh.Dispose();
                mesh = null;
            }
            if (texture != null)
            {
                texture.Dispose();
                texture = null;
            }

            this.isInitialized = false;
            //base.Dispose();
        }
        #endregion

        private void LoadTexturesAndMaterials(DrawArgs drawArgs)//导入贴图和材质 
        {
            material = new Material();
            material.Diffuse = Color.FromArgb(255, 0, 0, 0);
            material.Specular = Color.LightGray;
            material.Power = 15.0F;
            drawArgs.Device.Material = material;
            texture = Texture.FromFile(drawArgs.Device, texturePath);
        }
        public Vector3 m_position { get; set; }

        public Matrix WorldTransform { get; set; }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            bool flag = PerformSelectionAction(DrawArgs.LastMousePosition.X, DrawArgs.LastMousePosition.Y, drawArgs);
            if (Selected != null)
            {
                Selected(m_selectedMinDistance, pickedPosition);
            }
            return flag;
        }

        public Vector3 pickedPosition;
        public float m_selectedMinDistance { get; set; }

        public string LayerName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
    }
}

