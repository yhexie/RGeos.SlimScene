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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            mSceneControl = new UcSceneControl2();
            mSceneControl.Size = new Size(500, 400);
            mSceneControl.Dock = DockStyle.Fill;
        }
        public UcSceneControl2 mSceneControl = null;
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Controls.Add(mSceneControl);
            Earth earth = new Earth("地球", 100, 36, 36);
            earth.IsOn = true;
            mSceneControl.CurrentWorld.RenderableObjects.ChildObjects.Add(earth);
        }
    }
}
