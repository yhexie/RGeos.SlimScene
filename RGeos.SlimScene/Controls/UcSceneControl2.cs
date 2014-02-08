using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX.Direct3D9;
using System.Threading;
using RGeos.SlimScene.Core;
using SlimDX;
using System.Diagnostics;
using Utility;

namespace RGeos.SlimScene.Controls
{
    public partial class UcSceneControl2 : UserControl
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


        public UcSceneControl2()
        {
            base.SetStyle(ControlStyles.UserPaint | ControlStyles.Opaque, true);
            InitializeComponent();

        }

        private void UcSceneControl2_Load(object sender, EventArgs e)
        {
            InitializeGraphics();
            drawArgs = new DrawArgs(m_Device3d, this);

            m_World = new World("世界");
            this.drawArgs.WorldCamera = new Camera();

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
            if (base.Height <= 0 || base.Width <= 0)
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
            m_Device3d.SetRenderState(RenderState.Ambient, Color.FromArgb(0x40, 0x40, 0x40).ToArgb());

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
            float aspectRatio = (float)m_Device3d.Viewport.Width / m_Device3d.Viewport.Height;
            //float aspectRatio = (float)Width /Height;
            Matrix projection = Matrix.PerspectiveFovLH(fov, aspectRatio, mapWidth == 0 ? 0.30f : (float)(mapWidth / 10), mapWidth == 0 ? 500f : (float)(mapWidth * 3));
            m_Device3d.SetTransform(TransformState.Projection, projection);
            //m_Device3d.SetTransform(TransformState.View, Matrix.LookAtLH(new Vector3(0, 30, -30), new Vector3(0, 0, 0), new Vector3(0, 1.0f, 0)));

            //BoundingBox bound = new BoundingBox(new Vector3(-100, -100, -100), new Vector3(100, 100, 100));
            //Vector3 centre = (bound.Maximum + bound.Minimum) / 2;
            //Vector3 position = centre + new Vector3(0, 150, -250);
            //Matrix ViewMatrix = Matrix.LookAtLH(position, centre, new Vector3(0, 1.0f, 0));
            //m_Device3d.SetTransform(TransformState.View, ViewMatrix);

            //i += 2;
            //if (i == 360)
            //{
            //    i = 0;
            //}
            //this.drawArgs.WorldCamera = new Camera();
            //this.drawArgs.WorldCamera.RotateRay((float)(i * Math.PI / 180), new Vector3(0f, 0f, 0f), new Vector3(0.0f, 1.0f, 0.0f));
            this.drawArgs.WorldCamera.Update(m_Device3d);
        }
        int i = 0;
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
        private bool isMouseDragging;
        bool isDoubleClick = false;
        private Point mouseDownStartPosition = Point.Empty;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();  //fixes mousewheel not working problem

            DrawArgs.LastMousePosition.X = e.X;
            DrawArgs.LastMousePosition.Y = e.Y;

            mouseDownStartPosition.X = e.X;
            mouseDownStartPosition.Y = e.Y;


            if (e.Button == MouseButtons.Left)
                DrawArgs.IsLeftMouseButtonDown = true;

            if (e.Button == MouseButtons.Right)
                DrawArgs.IsRightMouseButtonDown = true;
            // Call the base class method so that registered delegates receive the event.
            base.OnMouseDown(e);

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Default to default cursor
            DrawArgs.MouseCursor = CursorType.Arrow;

            try
            {
                bool handled = false;

                if (!handled)
                {
                    int deltaX = e.X - DrawArgs.LastMousePosition.X;
                    int deltaY = e.Y - DrawArgs.LastMousePosition.Y;
                    float deltaXNormalized = (float)deltaX / drawArgs.screenWidth;
                    float deltaYNormalized = (float)deltaY / drawArgs.screenHeight;



                    if (mouseDownStartPosition == Point.Empty)
                        return;

                    bool isMouseLeftButtonDown = ((int)e.Button & (int)MouseButtons.Left) != 0;
                    bool isMouseRightButtonDown = ((int)e.Button & (int)MouseButtons.Right) != 0;
                    if (isMouseLeftButtonDown || isMouseRightButtonDown)
                    {
                        int dx = this.mouseDownStartPosition.X - e.X;
                        int dy = this.mouseDownStartPosition.Y - e.Y;
                        int distanceSquared = dx * dx + dy * dy;
                        if (distanceSquared > 3 * 3)
                            // Distance > 3 = drag
                            this.isMouseDragging = true;
                    }

                    if (isMouseLeftButtonDown && !isMouseRightButtonDown)
                    {

                        //锁定相机的扭动
                        //if (WorldSetting.CameraTwistLock)
                        //{
                        //    if (Angle.IsNaN(curLat) || Angle.IsNaN(prevLat))
                        //    {
                        //        // Old style pan
                        //        Angle deltaLat = Angle.FromRadians((double)deltaY * (this.drawArgs.WorldCamera.Altitude) / (800 * this.CurrentWorld.EquatorialRadius));
                        //        Angle deltaLon = Angle.FromRadians((double)-deltaX * (this.drawArgs.WorldCamera.Altitude) / (800 * this.CurrentWorld.EquatorialRadius));
                        //        this.drawArgs.WorldCamera.Pan(deltaLat, deltaLon);
                        //    }
                        //    else
                        //    {
                        //        //Picking ray pan
                        //        Angle lat = prevLat - curLat;
                        //        Angle lon = prevLon - curLon;
                        //        this.drawArgs.WorldCamera.Pan(lat, lon);
                        //    }
                        //}
                        //else
                        //{
                        //double factor = 100 / 250;

                        //Set adjustment factor for width/height
                        float adjustWidth = 1.0f / ((Width - 1.0f) * 0.5f);
                        float adjustHeight = 1.0f / ((Height - 1.0f) * 0.5f);

                        PointF tempPoint = new PointF(mouseDownStartPosition.X, mouseDownStartPosition.Y);

                        //Adjust point coords and scale down to range of [-1 ... 1]
                        tempPoint.X = (tempPoint.X * adjustWidth) - 1.0f;
                        tempPoint.Y = 1.0f - (tempPoint.Y * adjustHeight);

                        //Compute square of the length of the vector from this point to the center
                        float length = (tempPoint.X * tempPoint.X) + (tempPoint.Y * tempPoint.Y);
                        Vector3 vector = new Vector3(0.0f, 1.0f, 0.0f);
                        //If the point is mapped outside the sphere... (length > radius squared)
                        if (length > 1.0f)
                        {
                            //Compute a normalizing factor (radius / sqrt(length))
                            float norm = (float)(1.0 / Math.Sqrt(length));

                            //Return the "normalized" vector, a point on the sphere
                            vector.X = tempPoint.X * norm;
                            vector.Y = tempPoint.Y * norm;
                            vector.Z = 0.0f;
                        }
                        //Else it's inside
                        else
                        {
                            //Return a vector to a point mapped inside the sphere sqrt(radius squared - length)
                            vector.X = tempPoint.X;
                            vector.Y = tempPoint.Y;
                            vector.Z = (float)System.Math.Sqrt(1.0f - length);
                        }

                        PointF tempPointEnd = new PointF(e.X, e.Y);

                        //Adjust point coords and scale down to range of [-1 ... 1]
                        tempPointEnd.X = (tempPointEnd.X * adjustWidth) - 1.0f;
                        tempPointEnd.Y = 1.0f - (tempPointEnd.Y * adjustHeight);

                        //Compute square of the length of the vector from this point to the center
                        float lengthEnd = (tempPointEnd.X * tempPointEnd.X) + (tempPointEnd.Y * tempPointEnd.Y);
                        Vector3 vector2 = new Vector3(10.0f, 0.0f, 0.0f);
                        if (lengthEnd > 1.0f)
                        {
                            float norm = (float)(1.0 / Math.Sqrt(lengthEnd));
                            vector2.X = tempPointEnd.X * norm;
                            vector2.Y = tempPointEnd.Y * norm;
                            vector2.Z = 0.0f;
                        }
                        else
                        {
                            vector2.X = tempPointEnd.X;
                            vector2.Y = tempPointEnd.Y;
                            vector2.Z = (float)System.Math.Sqrt(1.0f - lengthEnd);
                        }
                        Vector3 Perp = new Vector3();
                        Matrix view = drawArgs.Device.GetTransform(TransformState.View);

                        //计算垂直向量
                        Perp = Vector3.Cross(vector, vector2);
                        //drawArgs.WorldCamera.RotateRay((float)(deltaX / 10), new Vector3(0f, 0f, 0f), new Vector3(view.M12,view.M22, view.M32));
                        //drawArgs.WorldCamera.RotateRay((float)((deltaY) / 5), new Vector3(0f, 0f, 0f), new Vector3(view.M11, view.M21, view.M31));
                        drawArgs.WorldCamera.RotateRay(0.01f * deltaX, new Vector3(0f, 0f, 0f), new Vector3(view.M12, view.M22, view.M32));
                        drawArgs.WorldCamera.RotateRay(0.01f * deltaY, new Vector3(0f, 0f, 0f), new Vector3(view.M11, view.M21, view.M31));
                        //}
                    }
                    else if (!isMouseLeftButtonDown && isMouseRightButtonDown)
                    {
                        //Right mouse button
                        float deltaEyeDirection = -deltaXNormalized;
                        this.drawArgs.WorldCamera.RotateRay((float)(Math.Atan(deltaEyeDirection)), new Vector3(0f, 0f, 0f), new Vector3(10.0f, 0.0f, 0.0f));


                    }
                    else if (isMouseLeftButtonDown && isMouseRightButtonDown)
                    {
                        // Both buttons (zoom)
                        if (Math.Abs(deltaYNormalized) > float.Epsilon)
                            this.drawArgs.WorldCamera.walk(-deltaYNormalized * 2);

                        //if (!World.Settings.CameraBankLock)
                        //    this.drawArgs.WorldCamera.Bank -= Angle.FromRadians(deltaXNormalized * World.Settings.CameraRotationSpeed);
                    }
                }
            }
            catch
            {
            }
            finally
            {

                DrawArgs.LastMousePosition.X = e.X;
                DrawArgs.LastMousePosition.Y = e.Y;
                base.OnMouseMove(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            DrawArgs.LastMousePosition.X = e.X;
            DrawArgs.LastMousePosition.Y = e.Y;

            try
            {
                bool handled = false;


                if (!handled)
                {
                    // Mouse must have been clicked outside our window and released on us, ignore
                    if (mouseDownStartPosition == Point.Empty)
                        return;

                    mouseDownStartPosition = Point.Empty;


                    if (m_World == null)
                        return;

                    if (isDoubleClick)
                    {
                        isDoubleClick = false;
                        if (e.Button == MouseButtons.Left)
                        {
                            drawArgs.WorldCamera.walk(10);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            drawArgs.WorldCamera.walk(-10);
                        }
                    }
                    else
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            if (this.isMouseDragging)
                            {
                                this.isMouseDragging = false;
                            }
                            else
                            {
                                if (!m_World.PerformSelectionAction(this.drawArgs))
                                {

                                    //Angle targetLatitude;
                                    //Angle targetLongitude;
                                    ////Quaternion targetOrientation = new Quaternion();
                                    //this.drawArgs.WorldCamera.PickingRayIntersection(
                                    //    DrawArgs.LastMousePosition.X,
                                    //    DrawArgs.LastMousePosition.Y,
                                    //    out targetLatitude,
                                    //    out targetLongitude);
                                    //if (!Angle.IsNaN(targetLatitude))
                                    //    this.drawArgs.WorldCamera.PointGoto(targetLatitude, targetLongitude);
                                }
                            }
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            if (this.isMouseDragging)
                                this.isMouseDragging = false;
                            else
                            {
                                if (!m_World.PerformSelectionAction(this.drawArgs))
                                {
                                    //nothing at the moment
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (e.Button == MouseButtons.Left)
                    DrawArgs.IsLeftMouseButtonDown = false;

                if (e.Button == MouseButtons.Right)
                    DrawArgs.IsRightMouseButtonDown = false;
                // Call the base class method so that registered delegates receive the event.
                base.OnMouseUp(e);
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            isDoubleClick = true;
            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                drawArgs.WorldCamera.walk(3);
            }
            else
            {
                drawArgs.WorldCamera.walk(-3);
            }
            base.OnMouseWheel(e);
        }
    }
}
