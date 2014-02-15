using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.ComponentModel;
using SlimDX.Direct3D9;

namespace RGeos.SlimScene
{
    [Serializable()]
    public abstract class RCamera
    {
        public string Name { get; set; }


        /// <summary>
        /// The screen aspect ratio.
        /// </summary>
        private float aspectRatio = 1.0f;
        /// <summary>
        /// Gets or sets the aspect.
        /// </summary>
        /// <value>
        /// The aspect.
        /// </value>
        [Description("Screen Aspect Ratio"), Category("Camera")]
        public float AspectRatio
        {
            get { return aspectRatio; }
            set { aspectRatio = value; }
        }

        /// <summary>
        /// The camera position.
        /// </summary>
        private Vector3 position = new Vector3(0, 0, 0);

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        [Description("The position of the camera"), Category("Camera")]
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="RCamera"/> class.
        /// </summary>
        public RCamera()
        {
            Name = "Camera";
        }

        /// <summary>
        ///	This function projects through the camera, to OpenGL, ie. it 
        ///	creates a projection matrix.
        /// </summary>
        public virtual void Project(Device device)
        {
            //	Perform the projection.
            TransformProjectionMatrix(device);

            //	Get the matrix.
            m_ProjectionMatrix = device.GetTransform(TransformState.Projection);
        }

        /// <summary>
        /// This function is for when you simply want to call only the functions that
        /// would transform the projection matrix. Warning, it won't load the identity
        /// first, and it won't set the current matrix to projection, it's really for
        /// people who need to use it for their own projection functions (e.g Picking
        /// uses it to create a composite 'Pick' projection).
        /// </summary>
        public abstract void TransformProjectionMatrix(Device gl);

        /// <summary>
        /// UnProject和Project之前需要调用该方法
        /// </summary>
        /// <param name="m_Device3d"></param>
        public void ComputeMatrix(Device m_Device3d)
        {
            m_WorldMatrix = m_Device3d.GetTransform(TransformState.World);
            m_ProjectionMatrix = m_Device3d.GetTransform(TransformState.Projection);
            m_ViewMatrix = m_Device3d.GetTransform(TransformState.View);
            mViewPort = m_Device3d.Viewport;
        }

        /// <summary>
        /// Projects a point from world to screen coordinates.
        /// 计算指定世界坐标的屏幕坐标
        /// </summary>
        /// <param name="point">Point in world space</param>
        /// <returns>Point in screen space</returns>
        public Vector3 Project(Vector3 point)
        {
            point = Vector3.Project(point, mViewPort.X, mViewPort.Y, mViewPort.Width, mViewPort.Height, mViewPort.MinZ, mViewPort.MaxZ, m_WorldMatrix * m_ViewMatrix * m_ProjectionMatrix);//m_ProjectionMatrix * m_ViewMatrix * m_WorldMatrix
            return point;
        }

        public Vector3 UnProject(Vector3 v1)
        {
            Vector3 v2 = Vector3.Unproject(v1, mViewPort.X, mViewPort.Y, mViewPort.Width, mViewPort.Height, mViewPort.MinZ, mViewPort.MaxZ, m_WorldMatrix * m_ViewMatrix * m_ProjectionMatrix);
            return v2;
        }

        public virtual double Distance
        {
            get
            {
                return 0;
            }
        }

        protected Viewport mViewPort;//视口大小
        /// <summary>
        /// Every time a camera is used to project, the projection matrix calculated 
        /// and stored here.
        /// </summary>
        protected Matrix m_ProjectionMatrix = new Matrix();
        protected Matrix m_ViewMatrix; //上一次渲染采用的观察矩阵
        protected Matrix m_WorldMatrix = Matrix.Identity;//世界变换矩阵

        //视口大小
        public Viewport Viewport
        {
            get
            {
                return mViewPort;
            }
        }
        //观察变换矩阵
        public Matrix ViewMatrix
        {
            get
            {
                return m_ViewMatrix;
            }
        }
        //投影变换矩阵
        public Matrix ProjectionMatrix
        {
            get
            {
                return m_ProjectionMatrix;
            }
        }
        //世界变换矩阵
        public Matrix WorldMatrix
        {
            get
            {
                return m_WorldMatrix;
            }
        }

    }
}
