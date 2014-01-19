using System;
using System.Drawing;
using System.Windows.Forms;
using SlimDX.Direct3D9;
using System.Threading;
using Utility;
using System.Diagnostics;
using SlimDX;

namespace RGeos.SlimScene.Core
{
    public partial class UcSceneControl : UserControl
    {
        private Device m_Device3d;
        private PresentParameters m_presentParams;
      
        private Thread m_WorkerThread;
        private bool m_WorkerThreadRunning;
        private System.Timers.Timer m_FpsTimer = new System.Timers.Timer(250);
        private double mapWidth = 0;
        private static bool IsAppStillIdle
        {
            get
            {
                NativeMethods.Message msg;
                return !NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }
        //当前世界
        private World m_World;

        public World CurrentWorld
        {
            get { return m_World; }
            set { m_World = value; }
        }
        private DrawArgs drawArgs;

        public DrawArgs DrawArgs
        {
            get { return drawArgs; }
        }      
        public BoundingBox BoundingBox;
        // Rotation/Zoom/Pan
        private System.Object matrixLock = new System.Object();
        private Arcball arcBall = new Arcball(640.0f, 480.0f);
        private Matrix matrix = Matrix.Identity;
        private MatrixArcBall LastTransformation = new MatrixArcBall();
        private MatrixArcBall ThisTransformation = new MatrixArcBall();

        // mouse 
        private Point mouseStartDrag;
        private static bool isLeftDrag = false;
        private static bool isRightDrag = false;
        private static bool isMiddleDrag = false;

        public UcSceneControl()
        {
            base.SetStyle(ControlStyles.UserPaint | ControlStyles.Opaque, true);
            InitializeComponent();

        }

        private void UcSceneControl_Load(object sender, EventArgs e)
        {
            InitializeGraphics();
            drawArgs = new DrawArgs(m_Device3d, this);

            m_World = new World("世界");
            this.drawArgs.WorldCamera = new Camera();

            LastTransformation.SetIdentity(); // Reset Rotation
            ThisTransformation.SetIdentity(); // Reset Rotation
            matrix = ThisTransformation.get_Renamed();
            arcBall = new Arcball(Width, Height);
            arcBall.SetBounds(Width, Height);
        }

        private void InitializeGraphics()
        {
            m_presentParams = new PresentParameters();
            m_presentParams.Windowed = true;
            m_presentParams.SwapEffect = SwapEffect.Discard;
            m_presentParams.AutoDepthStencilFormat = Format.D16;
            m_presentParams.EnableAutoDepthStencil = true;
            m_presentParams.PresentationInterval = PresentInterval.Immediate;
            m_presentParams.BackBufferHeight = base.Height;
            m_presentParams.BackBufferWidth = base.Width;
            m_presentParams.DeviceWindowHandle = base.Handle;
            int adapterOrdinal = 0;

            DeviceType dType = DeviceType.Hardware;
            CreateFlags flags = CreateFlags.SoftwareVertexProcessing;

            flags |= CreateFlags.Multithreaded | CreateFlags.FpuPreserve;
            try
            {
                Direct3D direct3d = new Direct3D();
                //实例化设备对象
                m_Device3d = new Device(direct3d, adapterOrdinal, dType, this.Handle, flags, m_presentParams);
            }
            catch (Exception)
            {
                throw new NotSupportedException("无法创建Direct3D设备.");
            }

            //重置设备
            OnDeviceReset();
        }

        private void OnDeviceReset()
        {
            // Can we use anisotropic texture minify filter?
            if (base.Height<=0 ||base.Width<=0)
            {
                return;
            }
            PresentParameters presentParameters = this.m_presentParams.Clone();
            presentParameters.BackBufferHeight = this.Height;
            presentParameters.BackBufferWidth = this.Width;
            presentParameters.DeviceWindowHandle = base.Handle;
            m_Device3d.Reset(presentParameters);
            if ((m_Device3d.Capabilities.TextureFilterCaps & FilterCaps.MinLinear) == FilterCaps.MinLinear)
            {
                m_Device3d.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
                m_Device3d.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Linear);

            }
            else if ((m_Device3d.Capabilities.TextureFilterCaps & FilterCaps.MinAnisotropic) == FilterCaps.MinAnisotropic)
            {
                m_Device3d.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Anisotropic);
            }

            // What about magnify filter?
            if ((m_Device3d.Capabilities.TextureFilterCaps & FilterCaps.MagAnisotropic) == FilterCaps.MagAnisotropic)
            {
                m_Device3d.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Anisotropic);
            }
            else if ((m_Device3d.Capabilities.TextureFilterCaps & FilterCaps.MagLinear) == FilterCaps.MagLinear)
            {
                m_Device3d.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
            }

            m_Device3d.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Clamp);
            m_Device3d.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Clamp);
            m_Device3d.SetRenderState(RenderState.SrgbWriteEnable, false);

            m_Device3d.SetRenderState(RenderState.Clipping, true);
            //Clockwise不显示按顺时针绘制的三角形
            m_Device3d.SetRenderState(RenderState.CullMode, Cull.None);
            m_Device3d.SetRenderState(RenderState.Lighting, false);
            m_Device3d.SetRenderState(RenderState.Ambient,  Color.FromArgb(0x40, 0x40, 0x40).ToArgb());

            m_Device3d.SetRenderState(RenderState.ZEnable, true);
            m_Device3d.SetRenderState(RenderState.AlphaBlendEnable, true);
            m_Device3d.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            m_Device3d.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
        }

        protected void AttemptRecovery()
        {
            try
            {
                m_Device3d.TestCooperativeLevel();
            }
            catch (SlimDXException exception)
            {
                try
                {
                    if (exception.ResultCode == ResultCode.DeviceNotReset)
                    {
                       // m_Device3d.Reset(m_presentParams);
                        OnDeviceReset();
                    }
                }
                catch (SlimDXException)
                {
                    //如果恢复设备仍然失败，就不管了
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Paint the last active scene if rendering is disabled to keep the ui responsive
            try
            {
                if (m_Device3d == null)
                {
                    e.Graphics.Clear(SystemColors.Control);
                    return;
                }
                Render();
                m_Device3d.Present();
            }
            catch (SlimDXException)
            {
                try
                {
                    AttemptRecovery();
                    Render();
                    m_Device3d.Present();
                }
                catch (Exception)
                {
                    // Ignore a 2nd failure
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            if (this.Size.Width == 0 || this.Size.Height == 0)
            {
                return;
            }
            if (drawArgs != null)
            {             
                //m_Device3d.Reset(m_presentParams);
                //m_Device3d.Viewport = new Viewport(Left, Top, Width, this.Height);
                OnDeviceReset();//窗体大小改变时，重置设备
                this.drawArgs.screenHeight = this.Height;
                this.drawArgs.screenWidth = this.Width;
            }
            base.OnResize(e);
        }

        public void Render()
        {
            try
            {
                this.drawArgs.BeginRender();
                System.Drawing.Color backgroundColor = System.Drawing.Color.DarkSlateBlue;
                m_Device3d.Clear(ClearFlags.Target | ClearFlags.ZBuffer, backgroundColor, 1.0f, 0);

                if (m_World == null)
                {
                    m_Device3d.BeginScene();
                    m_Device3d.EndScene();
                    m_Device3d.Present();
                    Thread.Sleep(25);
                    return;
                }

                if (m_WorkerThread == null)
                {
                    m_WorkerThreadRunning = true;
                    m_WorkerThread = new Thread(new ThreadStart(WorkerThreadFunc));
                    m_WorkerThread.Name = "RGeos.SlimScene.WorkerThreadFunc";
                    m_WorkerThread.IsBackground = true;
                    m_WorkerThread.Start();
                }
                //设置投影矩阵和视图矩阵
                CameraViewSetup();
                lock (matrixLock)
                {
                    matrix = ThisTransformation.get_Renamed();
                }
                Matrix world = m_Device3d.GetTransform(TransformState.World);
                m_Device3d.SetTransform(TransformState.World, matrix); //保证实现A状态-B状态-重置为A状态          
                //设置绘制模式，是线框模式？
                if (WorldSetting.RenderWireFrame)
                    m_Device3d.SetRenderState(RenderState.FillMode, FillMode.Wireframe);
                else
                    m_Device3d.SetRenderState(RenderState.FillMode, FillMode.Solid);
                //设置启用Z缓冲
                drawArgs.Device.SetRenderState(RenderState.ZEnable, true);
                //设置启用雾效
                if (WorldSetting.FogEnable == false)
                    m_Device3d.SetRenderState(RenderState.FogEnable, false);

                m_Device3d.BeginScene();

                // 渲染世界对象
                m_World.Render(this.drawArgs);

                m_Device3d.SetTransform(TransformState.World, world);
                m_Device3d.EndScene();
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
            }
            finally
            {
                this.drawArgs.EndRender();
            }
            drawArgs.UpdateMouseCursor(this);
        }
        /// <summary>
        /// Background worker thread loop (updates UI)
        /// </summary>
        private void WorkerThreadFunc()
        {
            const int refreshIntervalMs = 150; // Max 6 updates per seconds
            while (m_WorkerThreadRunning)
            {
                try
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    m_World.Update(this.drawArgs);
                    watch.Stop();
                    // 计算需要睡眠的时间  
                    float elapsedMilliSeconds = 1000 * (float)
        (watch.ElapsedTicks) / Stopwatch.Frequency;
                    float remaining = refreshIntervalMs - elapsedMilliSeconds;
                    if (remaining > 0)
                        Thread.Sleep((int)remaining);
                }
                catch (Exception caught)
                {
                    Log.Write(caught);
                }
            }
        }
        // 建立相机
        private void CameraViewSetup()
        {
            float fov = (float)Math.PI / 4;
            //float aspectRatio = (float)m_Device3d.Viewport.Width / m_Device3d.Viewport.Height;
            float aspectRatio = (float)Width /Height;
            Matrix projection = Matrix.PerspectiveFovLH(fov, aspectRatio, mapWidth == 0 ? 0.30f : (float)(mapWidth / 10), mapWidth == 0 ? 500f : (float)(mapWidth * 3));
            m_Device3d.SetTransform(TransformState.Projection, projection);
            m_Device3d.SetTransform(TransformState.View, Matrix.LookAtLH(new Vector3(0, 30, -30), new Vector3(0, 0, 0), new Vector3(0, 1.0f, 0)));

            BoundingBox bound = new BoundingBox(new Vector3(-100, -100, -100), new Vector3(100, 100, 100));
            Vector3 centre = (bound.Maximum + bound.Minimum) / 2;
            Vector3 position = centre + new Vector3(0, 130, -230);
            Matrix matrix = Matrix.LookAtLH(position, centre, new Vector3(0, 1.0f, 0));
            m_Device3d.SetTransform(TransformState.View, matrix);


            // this.drawArgs.WorldCamera = new Camera();
            // this.drawArgs.WorldCamera.Update(m_Device3d);
        }

        public void OnApplicationIdle(object sender, EventArgs e)
        {
            if (Parent.Focused && !Focused)
                Focus();
            while (IsAppStillIdle)
            {
                Render();
                drawArgs.Present();
            }
            Application.DoEvents();
        }

        /// <summary>
        ///清除对象
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                if (m_WorkerThread != null && m_WorkerThread.IsAlive)
                {
                    m_WorkerThreadRunning = false;
                    m_WorkerThread.Abort();
                }

                // m_FpsTimer.Stop();
                if (m_World != null)
                {
                    m_World.Dispose();
                    m_World = null;
                }
                if (this.drawArgs != null)
                {
                    this.drawArgs.Dispose();
                    this.drawArgs = null;
                }
                m_Device3d.Dispose();
            }

            base.Dispose(disposing);
            GC.SuppressFinalize(this);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isLeftDrag = true;
                mouseStartDrag = new Point(e.X, e.Y);
                this.startDrag(mouseStartDrag);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                Cursor.Current = Cursors.NoMove2D;
                isMiddleDrag = true;
                mouseStartDrag = new Point(e.X, e.Y);
                this.startDrag(mouseStartDrag);
            }
            else
            {
                Cursor.Current = Cursors.SizeAll;
                isRightDrag = true;
                mouseStartDrag = new Point(e.X, e.Y);
                this.startDrag(mouseStartDrag);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.drag(new Point(e.X, e.Y));
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isLeftDrag = false;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                Cursor.Current = Cursors.Default;
                isMiddleDrag = false;
            }
            else
            {
                Cursor.Current = Cursors.Default;
                isRightDrag = false;
            }
        }

        /// <summary>
        /// reset display.
        /// </summary>
        public void reset()
        {
            lock (matrixLock)
            {
                LastTransformation.SetIdentity();                                // Reset Rotation
                ThisTransformation.SetIdentity();                                // Reset Rotation
            }

            this.Render();
        }

        private void startDrag(Point MousePt)
        {
            lock (matrixLock)
            {
                LastTransformation.set_Renamed(ThisTransformation); // Set Last Static Rotation To Last Dynamic One
            }
            arcBall.click(MousePt); // Update Start Vector And Prepare For Dragging

            mouseStartDrag = MousePt;

        }

        private void drag(Point MousePt)
        {
            Quat4f ThisQuat = new Quat4f();

            arcBall.drag(MousePt, ref ThisQuat); // Update End Vector And Get Rotation As Quaternion

            lock (matrixLock)
            {
                if (isMiddleDrag) //zoom
                {
                    double len = Math.Sqrt(mouseStartDrag.X * mouseStartDrag.X + mouseStartDrag.Y * mouseStartDrag.Y)
                        / Math.Sqrt(MousePt.X * MousePt.X + MousePt.Y * MousePt.Y);

                    ThisTransformation.Scale = (float)len;
                    ThisTransformation.Pan = new Vector3(0, 0, 0);
                    ThisTransformation.Rotation = new Quat4f();
                    ThisTransformation.MatrixMultiply(ThisTransformation, LastTransformation);//累积上次旋转值到当前旋转
                }
                else if (isRightDrag) //pan
                {
                    float x = (float)(MousePt.X - mouseStartDrag.X) / (float)this.Width;
                    float y = (float)(MousePt.Y - mouseStartDrag.Y) / (float)this.Height;
                    float z = 0.0f;

                    ThisTransformation.Pan = new Vector3(x, y, z);
                    ThisTransformation.Scale = 1.0f;
                    ThisTransformation.Rotation = new Quat4f();
                    ThisTransformation.MatrixMultiply(ThisTransformation, LastTransformation);
                }
                else if (isLeftDrag) //rotate
                {
                    ThisTransformation.Pan = new Vector3(0, 0, 0);
                    ThisTransformation.Scale = 1.0f;
                    ThisTransformation.Rotation = ThisQuat;
                    ThisTransformation.MatrixMultiply(ThisTransformation, LastTransformation);
                }
            }
        }

    }
}
