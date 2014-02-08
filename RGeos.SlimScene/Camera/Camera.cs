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
        /// Initializes a new instance of the <see cref="Camera"/> class.
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
            projectionMatrix = device.GetTransform(TransformState.Projection);
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
        /// The camera position.
        /// </summary>
        private Vector3 position = new Vector3(0, 0, 0);

        /// <summary>
        /// Every time a camera is used to project, the projection matrix calculated 
        /// and stored here.
        /// </summary>
        private Matrix projectionMatrix = new Matrix();

        /// <summary>
        /// The screen aspect ratio.
        /// </summary>
        private float aspectRatio = 1.0f;

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
    }
}
