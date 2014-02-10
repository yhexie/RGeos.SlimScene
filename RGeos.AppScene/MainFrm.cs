using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RGeos.SlimScene.Controls;
using RGeos.AppScene.Renderable;
using RGeos.SlimScene.Core;

namespace RGeos.AppScene
{
    public partial class MainFrm : Form
    {
        public UcSceneControlEx mSceneControl = null;
        public MainFrm()
        {
            InitializeComponent();
            mSceneControl = new UcSceneControlEx();

            mSceneControl.Size = new Size(800, 600);
            mSceneControl.Dock = DockStyle.Fill;
        }
        TreeNode Root = null;
        private void MainFrm_Load(object sender, EventArgs e)
        {
            this.panel1.Controls.Add(mSceneControl);
            Root=new TreeNode("渲染对象");
           
            treeView1.Nodes.Add(Root);
            mSceneControl.CurrentWorld.Scene.ItemAdded += new ItemAdded_Event(ItemAdded);
            // mSceneControl.mCamera.Position = new SlimDX.Vector3(100.0f, 100.0f, -100.0f);
            Plane earth = new Plane(5, 36, "Grid");
            earth.IsOn = true;
            mSceneControl.CurrentWorld.RenderableObjects.ChildObjects.Add(earth);
            RModelParams model = new RModelParams();
            model.FileName = string.Format(@"{0}\model\tiny.x", Application.StartupPath);
            XModel tiger = new XModel("老虎");
            tiger.IsOn = true;
            tiger.ModelParams = model;
            tiger.RenderPriority = RenderPriority.Custom;
            mSceneControl.CurrentWorld.Scene.AddLayer(tiger as ILayer);
        }

        public void ItemAdded(ILayer layer)
        {
            TreeNode layerNode = new TreeNode();
            layerNode.Text = layer.LayerName;
            Root.Nodes.Add(layerNode);
            Root.ExpandAll();
        }
    }
}
