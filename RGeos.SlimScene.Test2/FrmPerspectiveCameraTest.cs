using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RGeos.SlimScene.Controls;
using RGeos.SlimScene.Renderable;

namespace RGeos.SlimScene.Test2
{
    public partial class FrmPerspectiveCameraTest : Form
    {
        public FrmPerspectiveCameraTest()
        {
            InitializeComponent();
            mSceneControl = new UcSceneControlEx();
            mSceneControl.Size = new Size(800, 600);
            mSceneControl.Dock = DockStyle.Fill;
        }
        public UcSceneControlEx mSceneControl = null;
        private void FrmPerspectiveCameraTest_Load(object sender, EventArgs e)
        {
            Text = "使用相机控制观察";
            this.Controls.Add(mSceneControl);
            Earth earth = new Earth("地球", 100, 36, 36);
            earth.IsOn = true;
            mSceneControl.CurrentWorld.RenderableObjects.ChildObjects.Add(earth);
        }
    }
}
