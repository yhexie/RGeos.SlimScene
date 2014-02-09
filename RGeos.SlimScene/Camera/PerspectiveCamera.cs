using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;
using System.ComponentModel;
using SlimDX;

namespace RGeos.SlimScene
{
    [Serializable()]
    public class PerspectiveCamera : RCamera
    {
        /// <summary>
        /// Gets or sets the field of view.
        /// </summary>
        /// <value>
        /// The field of view.
        /// </value>
        [Description("The angle of the lense of the camera (60 degrees = human eye)."), Category("Camera (Perspective")]
        public float FieldOfView
        {
            get { return fieldOfView; }
            set { fieldOfView = value; }
        }

        /// <summary>
        /// Gets or sets the near.
        /// </summary>
        /// <value>
        /// The near.
        /// </value>
        [Description("The near clipping distance."), Category("Camera (Perspective")]
        public float Near
        {
            get { return near; }
            set { near = value; }
        }

        /// <summary>
        /// Gets or sets the far.
        /// </summary>
        /// <value>
        /// The far.
        /// </value>
        [Description("The far clipping distance."), Category("Camera (Perspective")]
        public float Far
        {
            get { return far; }
            set { far = value; }
        }

        Vector3 target = new Vector3(0.0f, 0.0f, 0.0f);

        public Vector3 Target
        {
            get { return target; }
            set { target = value; }
        }
        Vector3 upVector = new Vector3(0.0f, 1.0f, 0.0f);

        /// <summary>
        /// Initializes a new instance of the <see cref="PerspectiveCamera"/> class.
        /// </summary>
        public PerspectiveCamera()
        {
            Name = "Camera (Perspective)";
        }

        /// <summary>
        /// This is the class' main function, to override this function and perform a 
        /// perspective transformation.
        /// </summary>
        public override void TransformProjectionMatrix(Device device)
        {
            //  Perform the perspective transformation.
            //gl.Translate(Position.X, Position.Y, Position.Z);
            //gl.Perspective(fieldOfView, AspectRatio, near, far);
            Matrix projection = Matrix.PerspectiveFovLH(fieldOfView, AspectRatio, near, far);
            device.SetTransform(TransformState.Projection, projection);

            //  Perform the look at transformation.
            Matrix ViewMatrix = Matrix.LookAtLH(Position, target, upVector);
            device.SetTransform(TransformState.View, ViewMatrix);
        }

        public void RotateRay(float angle, Vector3 vOrigin, Vector3 vAxis)
        {
            // 计算新的焦点
            Vector3 vView = target - vOrigin;
            Matrix temp = Matrix.RotationAxis(vAxis, angle);
            vView = Vector3.TransformCoordinate(vView, temp);
            target = vOrigin + vView;

            // 计算新的视点
            vView = Position - vOrigin;
            vView = Vector3.TransformCoordinate(vView, temp);
            Position = vOrigin + vView;

            upVector = Vector3.TransformCoordinate(upVector, temp);
        }
        public void Zoom(int Delta)
        {
            float scaleFactor = -(float)Delta * 1.0f;
            // CamPosition.Subtract(CamTarget);
            Vector3 look = Vector3.Subtract(Position, target);
            look.Normalize();
            Position += look * scaleFactor;
        }
        /// <summary>
        /// The field of view. 
        /// </summary>
        private float fieldOfView = (float)Math.PI / 4;

        /// <summary>
        /// The near clip.
        /// </summary>
        private float near = 0.3f;

        /// <summary>
        /// The far flip.
        /// </summary>
        private float far = 500.0f;


    }
}
