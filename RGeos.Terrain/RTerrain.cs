using System;
using RGeos.SlimScene.Core;
using SlimDX.Direct3D9;
using System.Drawing;
using SlimDX;
using CustomVertex;
using System.IO;

namespace RGeos.Terrain
{
    public class RTerrain : RenderableObject
    {
        Device device = null;
        private Bitmap bitMap = null;
        private RasterBandData DataDem = null;
        //定义行列数目
        private int Cols = 5, Rows = 4;
        //定义像素的大小 
        private float cellHeight = 10f, cellWidth = 10f;
        //纹理
        private Texture texture = null;
        //材质
        private Material material;
        //顶点缓冲变量
        private VertexBuffer vertexBuffer;
        //索引缓冲变量
        private IndexBuffer indexBuffer;
        // 顶点变量
        private CustomVertex.PositionTextured[] vertices;
        //索引号变量
        private int[] indices;

        public RTerrain(string name, RasterBandData dem, Bitmap bitmap)
            : base(name)
        {
            DataDem = dem;
            Cols = dem.Columns;
            Rows = dem.Rows;
            bitMap = bitmap;
        }

        public override void Initialize(DrawArgs drawArgs)
        {
            try
            {
                device = drawArgs.Device;
                LoadTexturesAndMaterials();
                VertexDeclaration();
                IndicesDeclaration();//定义索引缓冲 
                isInitialized = true;
            }
            catch (Exception ex)
            {
                isInitialized = false;
                throw ex;
            }

        }
        //导入贴图和材质
        private void LoadTexturesAndMaterials() 
        {
            material = new Material();
            material.Diffuse = Color.White;
            material.Specular = Color.LightGray;
            material.Power = 15.0F;
            device.Material = material;
            System.IO.MemoryStream memory = new System.IO.MemoryStream();
            bitMap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Seek(0, SeekOrigin.Begin);
            texture = Texture.FromStream(device, memory);
        }

        public override void Update(DrawArgs drawArgs)
        {
            if (!isInitialized && isOn)
                Initialize(drawArgs);
        }
        //定义顶点
        private void VertexDeclaration() 
        {
            vertexBuffer = new VertexBuffer(device, Cols * Rows * CustomVertex.PositionTextured.SizeBytes, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);
            DataStream vs = vertexBuffer.Lock(0, Cols * Rows * CustomVertex.PositionTextured.SizeBytes, LockFlags.None);
            vertices = new CustomVertex.PositionTextured[Cols * Rows];//定义顶点 

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    //Color color = bitMap.GetPixel((int)(j * cellWidth), (int)(i * cellHeight));
                    float height = (float)DataDem.data[i * Cols + j];
                    if (height == DataDem.NoDataValue)
                    {
                        height = 0;
                    }
                    vertices[j + i * Cols].Position = new Vector3(j * cellWidth, height, -i * cellHeight);
                    vertices[j + i * Cols].Tu = (float)j / (Cols - 1);
                    vertices[j + i * Cols].Tv = (float)i / (Rows - 1);
                }
            }
            vs.WriteRange(vertices);
            vertexBuffer.Unlock();
            vs.Dispose();
        }
        //定义索引
        private void IndicesDeclaration() 
        {
            indexBuffer = new IndexBuffer(device, 32 * 6 * (Cols - 1) * (Rows - 1), Usage.WriteOnly, Pool.Default, true);
            DataStream ds = indexBuffer.Lock(0, 32 * 6 * (Cols - 1) * (Rows - 1), LockFlags.None);
            indices = new int[6 * (Cols - 1) * (Rows - 1)];
            int index = 0;
            for (int i = 0; i < Rows - 1; i++)
            {
                for (int j = 0; j < Cols - 1; j++)
                {
                    indices[index] = j + i * (Cols);
                    indices[index + 1] = j + (i + 1) * Cols;
                    indices[index + 2] = j + i * Cols + 1;
                    indices[index + 3] = j + i * Cols + 1;
                    indices[index + 4] = j + (i + 1) * Cols;
                    indices[index + 5] = j + (i + 1) * Cols + 1;
                    index += 6;
                }
            }
            ds.WriteRange(indices);
            indexBuffer.Unlock();
            ds.Dispose();
        }

        public override void Render(DrawArgs drawArgs)
        {
            if (!isInitialized || !isOn)
                return;
            VertexFormat curFormat = drawArgs.Device.VertexFormat;
            int curZEnable = drawArgs.Device.GetRenderState(RenderState.ZEnable);
            int curLighting = drawArgs.Device.GetRenderState(RenderState.Lighting);
            int curCull = drawArgs.Device.GetRenderState(RenderState.CullMode);
            int curAlphaBlendEnable = drawArgs.Device.GetRenderState(RenderState.AlphaBlendEnable);
            int curDepthBias = drawArgs.Device.GetRenderState(RenderState.DepthBias);
            int curColorOperation = drawArgs.Device.GetTextureStageState(0, TextureStage.ColorOperation);
            try
            {
                drawArgs.Device.SetRenderState(RenderState.Lighting, false);
                //深度偏移
                // drawArgs.Device.SetRenderState(RenderState.DepthBias, drawArgs.CurrentWorld.CurrentDepthBias - 0.00001f);//防止闪烁
                drawArgs.Device.VertexFormat = PositionTextured.Format;
                drawArgs.Device.SetRenderState(RenderState.ZEnable, true);
                drawArgs.Device.SetRenderState(RenderState.CullMode, Cull.None);
                drawArgs.Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
                drawArgs.Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
                drawArgs.Device.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Diffuse);
                drawArgs.Device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Disable);
                device.SetTexture(0, texture);//设置贴图 

                device.SetStreamSource(0, vertexBuffer, 0, PositionTextured.SizeBytes);
                device.Indices = indexBuffer;
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (Cols * Rows), 0, indices.Length / 3);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
            }
            finally
            {
                drawArgs.Device.VertexFormat = curFormat;
                drawArgs.Device.SetRenderState(RenderState.ZEnable, curZEnable);
                drawArgs.Device.SetRenderState(RenderState.Lighting, curLighting);
                drawArgs.Device.SetRenderState(RenderState.CullMode, curCull);
                drawArgs.Device.SetRenderState(RenderState.AlphaBlendEnable, curAlphaBlendEnable);
                drawArgs.Device.SetRenderState(RenderState.DepthBias, curDepthBias);
                drawArgs.Device.SetTextureStageState(0, TextureStage.ColorOperation, curColorOperation);
            }
        }

        public override void Dispose()
        {
            if (vertexBuffer != null)
            {
                vertexBuffer.Dispose();
                vertexBuffer = null;
                texture = null;
                vertices = null;
            }
            if (indexBuffer != null)
            {
                indexBuffer.Dispose();
                indexBuffer = null;
                indices = null;
            }
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            throw new NotImplementedException();
        }
    }
}
