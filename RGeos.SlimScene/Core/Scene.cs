using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RGeos.SlimScene.Core
{
    public class Scene : RenderableObject
    {
        public Scene(string name)
            : base(name)
        {
        }
        public override void Initialize(DrawArgs drawArgs)
        {
            throw new NotImplementedException();
        }

        public override void Update(DrawArgs drawArgs)
        {
            throw new NotImplementedException();
        }

        public override void Render(DrawArgs drawArgs)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            throw new NotImplementedException();
        }
    }
}
