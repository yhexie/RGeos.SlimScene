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
using RGeos.SlimScene.Renderable;
using SlimDX;
using RGeos.Terrain;

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
            Root = new TreeNode("渲染对象");

            treeView1.Nodes.Add(Root);
            mSceneControl.CurrentWorld.Scene.ItemAdded += new ItemAdded_Event(ItemAdded);

            mSceneControl.DrawArgs.WorldCamera.Position = new SlimDX.Vector3(300.0f, 300.0f, -300.0f);
            RGeos.AppScene.Renderable.Plane plane = new RGeos.AppScene.Renderable.Plane(20, 50, "Grid");
            plane.IsOn = true;
            mSceneControl.CurrentWorld.RenderableObjects.ChildObjects.Add(plane);
            string path = string.Format(@"{0}\data\Icons\定位.png", Application.StartupPath);
            SpriteTest sprite = new SpriteTest("你好", new Vector3(10f, 0f, 0f), path);
            mSceneControl.CurrentWorld.RenderableObjects.ChildObjects.Add(sprite);

            SpriteTest sprite2 = new SpriteTest("你好2", new Vector3(40f, 10f, 0f), path);
            mSceneControl.CurrentWorld.RenderableObjects.ChildObjects.Add(sprite2);
        }

        private void tspSelect_Click(object sender, EventArgs e)
        {
            mSceneControl.CurrentWorld.RenderableObjects.ChildObjects.Add(renderline);
            foreach (object item in mSceneControl.CurrentWorld.RenderableObjects.ChildObjects)
            {
                if (item is Earth)
                {
                    Earth earth = item as Earth;
                    bool flag = earth.PerformSelectionAction(mSceneControl.DrawArgs);
                    earth.Selected += Selected;
                    lines.ShapeType = ShapeType.Polyline;
                    renderline.FeatureClass = lines;

                }
            }
        }
        RenderLayer renderline = new RenderLayer("Line");
        FeatureClass lines = new FeatureClass();
        Feature feat = null;
        int num;
        int lineID;
        public void Selected(float dd, Vector3 vec)
        {
            // MessageBox.Show(dd.ToString() + vec.ToString());
            string path = string.Format(@"{0}\data\Icons\定位.png", Application.StartupPath);
            SpriteTest sprite2 = new SpriteTest("1", vec, path);
            mSceneControl.CurrentWorld.RenderableObjects.ChildObjects.Add(sprite2);

            if (num == 0)
            {
                feat = new Feature();
                Line line = new Line();
                feat.Shape = line as IGeometry;
                feat.FID = lineID;
                line.Name = lineID.ToString();
                lineID++;
                line.v1 = vec;
                num++;
            }
            else
            {
                num = 0;
                if (feat != null && feat.Shape != null)
                {
                    Line line = feat.Shape as Line;
                    line.v2 = vec;
                    lines.AddFeature(feat);
                }


            }

        }
        private void tspOpenXModel_Click(object sender, EventArgs e)
        {
            RModelParams model = new RModelParams();
            model.FileName = string.Format(@"{0}\model\汽车.X", Application.StartupPath);
            XModel car = new XModel("汽车");
            car.ModelParams = model;
            // car.RenderPriority = RenderPriority.Custom;
            car.IsOn = true;
            mSceneControl.CurrentWorld.RenderableObjects.ChildObjects.Add(car);

            //RModelParams model2 = new RModelParams();
            //model2.FileName = string.Format(@"{0}\model\tiny.x", Application.StartupPath);
            //XModel tiger = new XModel("tiny");
            //tiger.IsOn = true;
            //tiger.ModelParams = model2;
            //tiger.RenderPriority = RenderPriority.Custom;
            //mSceneControl.CurrentWorld.Scene.AddLayer(tiger as ILayer);
        }

        private void tspOpenEarth_Click(object sender, EventArgs e)
        {
            Earth earth = new Earth("地球", 100, 36, 36);
            earth.IsOn = true;
            mSceneControl.CurrentWorld.RenderableObjects.ChildObjects.Add(earth);
        }
        string __ImagePath = string.Empty;
        private OSGeo.GDAL.Dataset __Geodataset;
        private int[] __DisplayBands;
        private void tspTerrain_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "";
            dlg.Filter = "Img(*.img)|*.img";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                OSGeo.GDAL.Gdal.AllRegister();
                __ImagePath = dlg.FileName;
                string NameOf = System.IO.Path.GetFileNameWithoutExtension(__ImagePath);
                //txtPath.Text = __ImagePath;
                OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(__ImagePath, OSGeo.GDAL.Access.GA_ReadOnly);
                __Geodataset = dataset;
                if (__Geodataset != null)
                {
                    if (__Geodataset.RasterCount >= 3)
                        __DisplayBands = new int[3] { 1, 2, 3 };
                    else
                        __DisplayBands = new int[3] { 1, 1, 1 };
                }
                double[] dd = new double[4];
                dataset.GetGeoTransform(dd);
                string prj = dataset.GetProjection();

                string str = string.Format("波段数目:{0}\n行数：{1};列数：{2}\n坐标参考：{3},{4},{5},{6}\n", __Geodataset.RasterCount, __Geodataset.RasterXSize, __Geodataset.RasterYSize, dd[0], dd[1], dd[2], dd[3]);
                str += prj + "\n";
                for (int i = 1; i <= __Geodataset.RasterCount; ++i)
                {
                    OSGeo.GDAL.Band band = dataset.GetRasterBand(i);
                    str += "波段1：" + band.DataType.ToString();
                    //dataset.ReadRaster(
                }
                //  richTextBox1.Text = str;
                RasterHelper rester = new RasterHelper(__Geodataset, __DisplayBands);
                Bitmap __BitMap = rester.InitialIMG(this.Width, this.Height);
                Vector3 position = new Vector3(-100f, 0f, 100f);
                SimpleRasterShow simRaster = new SimpleRasterShow(NameOf, position, __BitMap.Width, __BitMap.Height);
                simRaster.IsOn = true;
                simRaster.RenderPriority = RenderPriority.Custom;
                simRaster.bitmap = __BitMap;
                mSceneControl.CurrentWorld.RenderableObjects.ChildObjects.Add(simRaster);
            }
        }

        public void ItemAdded(ILayer layer)
        {
            TreeNode layerNode = new TreeNode();
            layerNode.Text = layer.LayerName;
            Root.Nodes.Add(layerNode);
            Root.ExpandAll();
        }

        private void tspLoadDEM_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "";
            dlg.Filter = "Img(*.img)|*.img";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string file = dlg.FileName;
                string NameOf = System.IO.Path.GetFileNameWithoutExtension(file);
                DemHelper dem = new DemHelper();
                dem.Start();
                dem.Read(file);
                RasterBandData bandata = dem.ReadDate(50, 40);
                Bitmap bitmap = dem.MakeGrayScale(50, 40);
                Vector3 position = new Vector3(-100f, 0f, 100f);
                //SimpleRasterShow simRaster = new SimpleRasterShow(NameOf, position, bitmap.Width, bitmap.Height);
                //simRaster.IsOn = true;
                //simRaster.RenderPriority = RenderPriority.Custom;
                //simRaster.bitmap = bitmap;
                //mSceneControl.CurrentWorld.RenderableObjects.ChildObjects.Add(simRaster);
                RTerrain terrain = new RTerrain(NameOf, bandata, bitmap);
                terrain.IsOn = true;
                mSceneControl.CurrentWorld.RenderableObjects.ChildObjects.Add(terrain);
            }
        }
    }
}
