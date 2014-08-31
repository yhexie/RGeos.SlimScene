using System;
using RGeos.SlimScene.Core;

namespace RGeos.SlimScene.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRenderable : IDisposable
    {
        void Initialize(DrawArgs drawArgs);
        void Update(DrawArgs drawArgs);
        void Render(DrawArgs drawArgs);
    }
}
