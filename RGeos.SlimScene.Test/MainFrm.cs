using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RGeos.SlimScene.Renderable;
using SlimDX.Direct3D9;
using SlimDX;

namespace RGeos.SlimScene.Test
{
    public partial class MainFrm : Form
    {
        public MainFrm()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            this.Text = "第一个程序";
            mSceneControl.Dock = DockStyle.Fill;
            Earth earth = new Earth("地球", 100, 36, 36);
            earth.IsOn = true;
            mSceneControl.CurrentWorld.RenderableObjects.ChildObjects.Add(earth);
           
        }
    }
}
