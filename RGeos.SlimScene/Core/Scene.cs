using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RGeos.SlimScene.Core
{
    public delegate void ItemAdded_Event(ILayer layer);
    public delegate void ItemRemoved_Event(ILayer layer);
    public class Scene : RenderableObjectList
    {
        public ItemAdded_Event ItemAdded;
        public ItemRemoved_Event ItemRemoved;
        public Scene(string name)
            : base(name)
        {
        }
        public void AddLayer(ILayer layer)
        {
            RenderableObject obj = layer as RenderableObject;
            m_children.Add(obj);
            if (ItemAdded != null)
            {
                ItemAdded(layer);
            }
        }
        public void RemoveLayer(ILayer layer)
        {
            RenderableObject obj = layer as RenderableObject;
            m_children.Remove(obj);
            if (ItemRemoved != null)
            {
                ItemRemoved(layer);
            }
        }
    }
}
