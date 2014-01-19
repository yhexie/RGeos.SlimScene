namespace RGeos.SlimScene.Test
{
    partial class MainFrm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.mSceneControl = new RGeos.SlimScene.Core.UcSceneControl();
            this.SuspendLayout();
            // 
            // ucSceneControl1
            // 
            this.mSceneControl.Location = new System.Drawing.Point(189, 86);
            this.mSceneControl.Name = "ucSceneControl1";
            this.mSceneControl.Size = new System.Drawing.Size(320, 219);
            this.mSceneControl.TabIndex = 0;
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 500);
            this.Controls.Add(this.mSceneControl);
            this.Name = "MainFrm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        public RGeos.SlimScene.Core.UcSceneControl mSceneControl;
    }
}

