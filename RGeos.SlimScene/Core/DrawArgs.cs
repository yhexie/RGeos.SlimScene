using System;
using System.Diagnostics;
using System.Collections;
using SlimDX.Direct3D9;

namespace RGeos.SlimScene.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class DrawArgs : IDisposable
    {
        public static Device sDevice = null;
        public static System.Windows.Forms.Control ParentControl = null;
        protected static Hashtable m_textures = new Hashtable();
        public static Hashtable Textures
        {
            get { return m_textures; }
        }
        public static RCamera Camera = null;

        public System.Windows.Forms.Control parentControl;

        public int numBoundaryPointsTotal;
        public int numBoundaryPointsRendered;
        public int numBoundariesDrawn;

        public System.Drawing.Font defaultSubTitleFont;

        public int screenWidth;
        public int screenHeight;
        public static System.Drawing.Point LastMousePosition;
        public int numberTilesDrawn;
        public System.Drawing.Point CurrentMousePosition;
        public string UpperLeftCornerText = "";

        public static bool IsLeftMouseButtonDown = false;
        public static bool IsRightMouseButtonDown = false;
        static CursorType mouseCursor;
        static CursorType lastCursor;
        bool repaint = true;
        bool isPainting;
        Hashtable fontList = new Hashtable();

        System.Windows.Forms.Cursor measureCursor;
        public int TexturesLoadedThisFrame = 0;
        private static System.Drawing.Bitmap bitmap;
        public static System.Drawing.Graphics Graphics = null;
        // Ù–‘
        private Device m_device = null;
        public Device Device
        {
            get { return m_device; }
            set { m_device = value; }
        }
        private RCamera m_WorldCamera = null;
        public RCamera WorldCamera
        {
            get
            {
                return m_WorldCamera;
            }
            set
            {
                m_WorldCamera = value;
                Camera = value;
            }
        }
        public World m_CurrentWorld = null;
        public World CurrentWorld
        {
            get
            {
                return m_CurrentWorld;
            }
            set
            {
                m_CurrentWorld = value;
            }
        }

        /// <summary>
        /// Absolute time of current frame render start (ticks)
        /// </summary>
        public static long CurrentFrameStartTicks;

        /// <summary>
        /// Seconds elapsed between start of previous frame and start of current frame.
        /// </summary>
        public static float LastFrameSecondsElapsed;

        public DrawArgs(Device device, System.Windows.Forms.Control parentForm)
        {
            this.parentControl = parentForm;
            DrawArgs.ParentControl = parentForm;
            DrawArgs.sDevice = device;
            this.m_device = device;

            bitmap = new System.Drawing.Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            DrawArgs.Graphics = System.Drawing.Graphics.FromImage(bitmap);
            //	InitializeReference();
        }

        public void BeginRender()
        {
            // Development variable to see the number of tiles drawn - Added for frustum culling testing
            this.numberTilesDrawn = 0;

            this.TexturesLoadedThisFrame = 0;

            this.UpperLeftCornerText = "";
            this.numBoundaryPointsRendered = 0;
            this.numBoundaryPointsTotal = 0;
            this.numBoundariesDrawn = 0;

            this.isPainting = true;
        }

        public void EndRender()
        {
            Debug.Assert(isPainting);
            this.isPainting = false;
        }

        /// <summary>
        /// Displays the rendered image (call after EndRender)
        /// </summary>
        public void Present()
        {
            // Calculate frame time
            long previousFrameStartTicks = CurrentFrameStartTicks;
            //PerformanceTimer.QueryPerformanceCounter(ref CurrentFrameStartTicks);
            //LastFrameSecondsElapsed = (CurrentFrameStartTicks - previousFrameStartTicks) / 
            //    (float)PerformanceTimer.TicksPerSecond;

            // Display the render
            m_device.Present();
        }

        /// <summary>
        /// Active mouse cursor
        /// </summary>
        public static CursorType MouseCursor
        {
            get
            {
                return mouseCursor;
            }
            set
            {
                mouseCursor = value;
            }
        }

        public void UpdateMouseCursor(System.Windows.Forms.Control parent)
        {
            if (lastCursor == mouseCursor)
                return;

            switch (mouseCursor)
            {
                case CursorType.Hand:
                    parent.Cursor = System.Windows.Forms.Cursors.Hand;
                    break;
                case CursorType.Cross:
                    parent.Cursor = System.Windows.Forms.Cursors.Cross;
                    break;
                case CursorType.Measure:
                    if (measureCursor == null)
                        //measureCursor = ImageHelper.LoadCursor("measure.cur");
                        parent.Cursor = measureCursor;
                    break;
                case CursorType.SizeWE:
                    parent.Cursor = System.Windows.Forms.Cursors.SizeWE;
                    break;
                case CursorType.SizeNS:
                    parent.Cursor = System.Windows.Forms.Cursors.SizeNS;
                    break;
                case CursorType.SizeNESW:
                    parent.Cursor = System.Windows.Forms.Cursors.SizeNESW;
                    break;
                case CursorType.SizeNWSE:
                    parent.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
                    break;
                default:
                    parent.Cursor = System.Windows.Forms.Cursors.Arrow;
                    break;
            }
            lastCursor = mouseCursor;
        }

        /// <summary>
        /// Returns the time elapsed since last frame render operation started.
        /// </summary>
        public static float SecondsSinceLastFrame
        {
            get
            {
                return 0;
                //long curTicks = 0;
                //PerformanceTimer.QueryPerformanceCounter(ref curTicks);
                //float elapsedSeconds = (curTicks - CurrentFrameStartTicks) / (float)PerformanceTimer.TicksPerSecond;
                //return elapsedSeconds;
            }
        }

        public bool IsPainting
        {
            get
            {
                return this.isPainting;
            }
        }

        public bool Repaint
        {
            get
            {
                return this.repaint;
            }
            set
            {
                this.repaint = value;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (IDisposable font in fontList.Values)
            {
                if (font != null)
                {
                    font.Dispose();
                }
            }
            fontList.Clear();

            if (measureCursor != null)
            {
                measureCursor.Dispose();
                measureCursor = null;
            }



            GC.SuppressFinalize(this);
        }

        #endregion

    }

    /// <summary>
    /// Mouse cursor
    /// </summary>
    public enum CursorType
    {
        Arrow = 0,
        Hand,
        Cross,
        Measure,
        SizeWE,
        SizeNS,
        SizeNESW,
        SizeNWSE
    }
}
