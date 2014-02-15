using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Utility;
using SlimDX.Direct3D9;
using RGeos.SlimScene.Core;
using SlimDX;

namespace RGeos.Terrain
{
    public class SimpleRasterShow : RenderableObject
    {
        private CustomVertex.PositionTextured[] vertices;// 定义顶点变量
        private Texture texture;//定义贴图变量 
        private Material material;//定义材质变量 
        public Bitmap bitmap = null;
        public SimpleRasterShow(string name)
            : base(name)
        {

        }
        public override void Initialize(DrawArgs drawArgs)
        {
            this.isInitialized = true;
            LoadTexturesAndMaterials(drawArgs);
            VertexDeclaration();
        }

        public override void Update(DrawArgs drawArgs)
        {
            if (!isInitialized && isOn)
            {
                if (drawArgs.WorldCamera.Distance > 500)
                {

                }
                else
                {

                }
                Initialize(drawArgs);
            }
        }

        public override void Render(DrawArgs drawArgs)
        {
            if (!isInitialized || !isOn)
                return;
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

                //drawArgs.Device.RenderState.DiffuseMaterialSource = ColorSource.Color1;
                //drawArgs.Device.RenderState.AlphaBlendEnable = true;
                //drawArgs.Device.RenderState.AlphaTestEnable = true;

                //drawArgs.Device.RenderState.ReferenceAlpha = 20;
                //drawArgs.Device.RenderState.AlphaFunction = Compare.Greater;

                //drawArgs.Device.RenderState.SourceBlend = Blend.SourceAlpha;
                //drawArgs.Device.RenderState.DestinationBlend = Blend.BothInvSourceAlpha;
                //drawArgs.Device.RenderState.BlendOperation = BlendOperation.Add;


                //drawArgs.Device.TextureState[0].ColorOperation = TextureOperation.Modulate;
                //drawArgs.Device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                //drawArgs.Device.TextureState[0].ColorArgument2 = TextureArgument.Current;
                //drawArgs.Device.TextureState[0].AlphaOperation = TextureOperation.SelectArg2;
                //drawArgs.Device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;

                //设置Z缓冲
                drawArgs.Device.SetRenderState(RenderState.ZEnable, 1);
               
                drawArgs.Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
                drawArgs.Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
                drawArgs.Device.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Diffuse);
                drawArgs.Device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Disable);
                drawArgs.Device.VertexFormat = CustomVertex.PositionTextured.Format;
                //设置纹理状态，此处使用纹理
                drawArgs.Device.SetTexture(0, texture);//设置贴图 
                drawArgs.Device.DrawUserPrimitives(PrimitiveType.TriangleList, 2, vertices);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            finally
            {
                drawArgs.Device.SetTransform(TransformState.World, world);
                drawArgs.Device.VertexFormat = format;
                drawArgs.Device.SetRenderState(RenderState.ZEnable, zEnable);
                drawArgs.Device.SetTextureStageState(0, TextureStage.ColorOperation, colorOper);
                drawArgs.Device.Indices = null;
            }
        }

        private void VertexDeclaration1()//定义顶点1 
        {
            vertices = new CustomVertex.PositionTextured[3];
            vertices[0].Position = new Vector3(10f, 0f, 10f);
            vertices[0].Tu = 1;
            vertices[0].Tv = 0;
            vertices[1].Position = new Vector3(-10f, 0f, -10f);
            vertices[1].Tu = 0;
            vertices[1].Tv = 1;
            vertices[2].Position = new Vector3(10f, 0f, -10f);
            vertices[2].Tu = 1;
            vertices[2].Tv = 1;
        }
        private void VertexDeclaration()//定义顶点 
        {
            vertices = new CustomVertex.PositionTextured[6];
            vertices[0].Position = new Vector3(10f, 0f, 10f);
            vertices[0].Tu = 1;
            vertices[0].Tv = 0;
            vertices[1].Position = new Vector3(-10f, 0f, -10f);
            vertices[1].Tu = 0;
            vertices[1].Tv = 1;
            vertices[2].Position = new Vector3(10f, 0f, -10f);
            vertices[2].Tu = 1;
            vertices[2].Tv = 1;
            vertices[3].Position = new Vector3(-10f, 0f, -10f);
            vertices[3].Tu = 0;
            vertices[3].Tv = 1;
            vertices[4].Position = new Vector3(10f, 0f, 10f);
            vertices[4].Tu = 1;
            vertices[4].Tv = 0;
            vertices[5].Position = new Vector3(-10f, 0f, 10f);
            vertices[5].Tu = 0;
            vertices[5].Tv = 0;

        }

        private void LoadTexturesAndMaterials(DrawArgs args)//导入贴图和材质 
        {
            material = new Material();
            material.Diffuse = Color.White;
            material.Specular = Color.LightGray;
            material.Power = 15.0F;
            args.Device.Material = material;
            System.IO.MemoryStream memory = new System.IO.MemoryStream();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Seek(0, SeekOrigin.Begin);
            texture = Texture.FromStream(args.Device, memory);
            //if (File.Exists(@"d:\temp.jpg"))
            //{
            //    File.Delete(@"d:\temp.jpg");
            //}
            //bitmap.Save(@"d:\temp.jpg");
            //texture = TextureLoader.FromFile(args.Device, @"d:\temp.jpg");
        }

        public override void Dispose()
        {
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return true;
        }

        public string LayerName
        {
            get
            {
                return name;
            }
            set
            {
                Name = value;
            }
        }
    }
}
