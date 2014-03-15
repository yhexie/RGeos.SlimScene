using System;
using System.Collections.Generic;
using System.Text;
using SlimDX.Direct3D9;
using SlimDX;
using System.Drawing;
using RGeos.SlimScene.Core;

namespace RGeos.AppScene.Renderable
{
    /// <summary>
    /// 精灵
    /// </summary>
    public class SpriteTest : RenderableObject
    {
        private string mFile;//纹理路径
        private Texture mTexture;//纹理
        private Vector3 mPosition;//位置
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="position">坐标(X,Y,Z)</param>
        /// <param name="file">纹理路径</param>
        public SpriteTest(string name, Vector3 position, string file)
            : base(name)
        {
            mPosition = position;
            this.mFile = file;
        }

        public override void Initialize(DrawArgs drawArgs)
        {
            if (System.IO.File.Exists(mFile))
            {
                //纹理
                this.mTexture = Texture.FromFile(drawArgs.Device, mFile, 0, 0, 0, Usage.None, Format.Dxt1, Pool.Managed, Filter.Box, Filter.Box, 0);
            }
            this.isInitialized = true;
        }


        public override void Render(DrawArgs drawArgs)
        {
            if (!this.isOn || !this.isInitialized) return;
            //获取z缓冲
            int zbuff = drawArgs.Device.GetRenderState(RenderState.ZEnable);
            //获取当前的DepthBias
            int depth = drawArgs.Device.GetRenderState(RenderState.DepthBias);
            try
            {
                Vector3 target = new Vector3(0f, 0f, 0f);
                //精灵的世界坐标
                Vector3 translationVector = this.mPosition - target;
                drawArgs.WorldCamera.ComputeMatrix(drawArgs.Device);
                //精灵的屏幕坐标
                Vector3 projectedPoint = drawArgs.WorldCamera.Project(translationVector);
                drawArgs.DefaultSprite.Transform = Matrix.Identity;
                if (drawArgs.WorldCamera.Distance < 500)
                {
                    //设置DepthBias
                    drawArgs.Device.SetRenderState(RenderState.DepthBias, 0);
                    //设置精灵的转换矩阵
                    drawArgs.DefaultSprite.Transform *= Matrix.Translation(projectedPoint.X, projectedPoint.Y, projectedPoint.Z);
                }
                else
                {
                    drawArgs.Device.SetRenderState(RenderState.DepthBias, 0);
                    drawArgs.DefaultSprite.Transform *= Matrix.Translation(projectedPoint.X, projectedPoint.Y, 0f);
                }

                //设置z缓冲
                drawArgs.Device.SetRenderState(RenderState.ZEnable, true);
                //开始渲染精灵
                drawArgs.DefaultSprite.Begin(SpriteFlags.AlphaBlend);

                drawArgs.DefaultSprite.Draw(this.mTexture, new Vector3(0, 0, 0), Vector3.Zero, new Color4(Color.White));
                //渲染字体             
                drawArgs.DefaultDrawingFont.DrawString(drawArgs.DefaultSprite, name, 10, 10, new Color4(Color.White));
            }
            catch (Exception caught)
            {
                Utility.Log.Write(caught);
            }
            finally
            {
                //结束精灵渲染，并恢复渲染状态
                drawArgs.DefaultSprite.End();
                drawArgs.Device.SetRenderState(RenderState.ZEnable, zbuff);
                drawArgs.Device.SetRenderState(RenderState.DepthBias, depth);
                drawArgs.DefaultSprite.Transform = Matrix.Identity;
            }
        }


        public override void Update(DrawArgs drawArgs)
        {
            if (!this.isInitialized)
            {
                this.Initialize(drawArgs);
                return;
            }
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        public override void Dispose()
        {
            if (this.mTexture != null)
            {
                this.mTexture.Dispose();
                this.mTexture = null;
            }
            this.isInitialized = false;
        }


        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            throw new NotImplementedException();
        }
    }
}
