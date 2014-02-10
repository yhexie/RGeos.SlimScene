using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RGeos.SlimScene.Core;
using SlimDX.Direct3D9;
using System.Drawing;
using SlimDX;
using System.IO;

namespace RGeos.AppScene.Renderable
{
    public class XModel : RenderableObject, ILayer
    {
        Mesh mesh = null;
        Material meshMaterials;
        Texture[] meshTextures;
        Material[] meshMaterials1;
        RModelParams mModelParams;

        internal RModelParams ModelParams
        {
            get { return mModelParams; }
            set { mModelParams = value; }
        }
        public XModel(string name)
            : base(name)
        {
        }
        public override void Initialize(DrawArgs drawArgs)
        {
            meshMaterials = new Material();
            meshMaterials.Ambient = System.Drawing.Color.White;		//材质如何反射环境光
            meshMaterials.Diffuse = System.Drawing.Color.White;// Color.FromArgb(127, 255, 255, 255);//材质如何反射灯光

            drawArgs.Device.Material = meshMaterials;//指定设备的材质


            //下句从tiger.x文件中读入3D图形(立体老虎)
            mesh = Mesh.FromFile(drawArgs.Device, mModelParams.FileName, MeshFlags.SystemMemory);
            string dir = Path.GetDirectoryName(ModelParams.FileName);
            ExtendedMaterial[] materials = mesh.GetMaterials();
            if (meshTextures == null)//如果还未设置纹理，为3D图形增加纹理和材质
            {
                meshTextures = new Texture[materials.Length];//纹理数组
                meshMaterials1 = new Material[materials.Length];//材质数组
                for (int i = 0; i < materials.Length; i++)//读入纹理和材质
                {
                    meshMaterials1[i] = materials[i].MaterialD3D;
                    meshMaterials1[i].Ambient = meshMaterials1[i].Diffuse;
                    meshTextures[i] = Texture.FromFile(drawArgs.Device, dir + @"\" + materials[i].TextureFileName);
                }
            }
            this.isInitialized = true;
        }

        public override void Update(DrawArgs drawArgs)
        {
            if (!isInitialized && isOn)
            {
                Initialize(drawArgs);
            }
        }

        public override void Render(DrawArgs drawArgs)
        {
            if (!this.isOn || !this.isInitialized) return;
            VertexFormat format = drawArgs.Device.VertexFormat;
            int currentCull = drawArgs.Device.GetRenderState(RenderState.FillMode);
            int currentColorOp = drawArgs.Device.GetTextureStageState(0, TextureStage.ColorOperation);
            int zBuffer = drawArgs.Device.GetRenderState(RenderState.ZEnable);
            try
            {

                drawArgs.Device.SetRenderState(RenderState.ZEnable, true);		 	//允许使用深度缓冲
                //drawArgs.Device.SetRenderState(RenderState.Ambient, System.Drawing.Color.White.ToArgb());//设定环境光为白色
                Light light = new Light();
                light.Type = LightType.Directional;
                light.Diffuse = Color.White;
                light.Direction = new Vector3(0, -1, 0);
                //light.Update();
                //light.Enabled = true;
                drawArgs.Device.SetLight(0, light);
                //drawArgs.Device.Lights[0].Type= LightType.Directional; //设置灯光类型
                //drawArgs.Device.Lights[0].Diffuse = Color.White;			//设置灯光颜色
                //drawArgs.Device.Lights[0].Direction = new Vector3(0, -1, 0);	//设置灯光位置
                //drawArgs.Device.Lights[0].Update();						//更新灯光设置，创建第一盏灯光
                //drawArgs.Device.Lights[0].Enabled = true;					//使设置有效
                drawArgs.Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
                drawArgs.Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
                drawArgs.Device.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Diffuse);
                drawArgs.Device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Disable);
                //drawArgs.Device.TextureState[0].ColorOperation = TextureOperation.Modulate;
                //drawArgs.Device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                //drawArgs.Device.TextureState[0].ColorArgument2 = TextureArgument.Diffuse;
                //drawArgs.Device.TextureState[0].AlphaOperation = TextureOperation.Disable;

                drawArgs.Device.SetRenderState(RenderState.Ambient, Color.FromArgb(0x40, 0x40, 0x40).ToArgb());

                for (int i = 0; i < meshMaterials1.Length; i++)//Mesh中可能有多个3D图形，逐一显示
                {
                    drawArgs.Device.Material = meshMaterials1[i];//设定3D图形的材质
                    drawArgs.Device.SetTexture(0, meshTextures[i]);//设定3D图形的纹理
                    mesh.DrawSubset(i);//显示该3D图形
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                drawArgs.Device.VertexFormat = format;
                drawArgs.Device.SetRenderState(RenderState.FillMode, currentCull);
                drawArgs.Device.SetTextureStageState(0, TextureStage.ColorOperation, currentColorOp);
                drawArgs.Device.SetRenderState(RenderState.ZEnable, zBuffer);
            }

        }

        public override void Dispose()
        {
            if (mesh != null)
            {
                mesh.Dispose();
                mesh = null;
            }
            if (meshTextures != null)
            {

            }
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            throw new NotImplementedException();
        }

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
