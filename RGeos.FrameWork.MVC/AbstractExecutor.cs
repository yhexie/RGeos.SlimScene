using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RGeos.FrameWork.MVC
{
   //被观察者，执行者
    public interface IExecutor
    {
        string Name { get; }

        bool IsStateChanged { get; }

        bool NeedRefresh { get; }
        //观察者集合
        List<IObserver> Observers { get; }

        bool OnAdd(object o, params object[] assistant);
        bool OnRemove(object o, params object[] assistant);
        bool OnReplace(object o, params object[] assistant);

        void NotifyObservers();
        //添加观察者
        bool RegisterObserver(IObserver view);
        //注销观察者
        bool UnRegisterObserver(IObserver view);
        //注销所有观察者
        bool UnRegisterObservers();
    }
    public abstract class AbstractExecutor : IExecutor, ICloneable
    {
        protected bool mIsStateChanged;
        protected string mName;
        protected bool mNeedRefresh;
        protected List<IObserver> m_observers;
        public bool IsStateChanged
        {
            get
            {
                return this.mIsStateChanged;
            }
        }

        public string Name
        {
            get
            {
                return this.mName;
            }
        }
        public bool NeedRefresh
        {
            get
            {
                return this.mNeedRefresh;
            }
        }

        public List<IObserver> Observers
        {
            get
            {
                return this.m_observers;
            }
        }

        protected AbstractExecutor(string name)
        {
            this.mName = name;
            this.mNeedRefresh = true;
            this.mIsStateChanged = false;
            this.m_observers = new List<IObserver>();
        }
        protected AbstractExecutor()
        {
            this.mNeedRefresh = true;
            this.mIsStateChanged = false;
            this.m_observers = new List<IObserver>();
        }


        public virtual object Clone()
        {
            return null;
        }

        public virtual bool RegisterObserver(IObserver observer)
        {
            if (this.m_observers == null)
            {
                this.m_observers = new List<IObserver>();
            }
            if (!this.m_observers.Contains(observer))
            {
                this.m_observers.Add(observer);
                return true;
            }
            return false;
        }

        public virtual bool UnRegisterObserver(IObserver observer)
        {
            if (this.m_observers != null)
            {
                while (this.m_observers.Contains(observer))
                {
                    return this.m_observers.Remove(observer);
                }
            }
            return false;
        }

        public virtual bool UnRegisterObservers()
        {
            if (this.m_observers != null)
            {
                this.m_observers.Clear();
                this.m_observers = null;
                return true;
            }
            return false;
        }

        public virtual bool OnAdd(object o, params object[] assistant)
        {
            this.mIsStateChanged = true;
            this.NotifyObservers();
            return true;
        }
        public virtual bool OnRemove(object o, params object[] assistant)
        {
            this.mIsStateChanged = true;
            this.NotifyObservers();
            return true;
        }
        public virtual bool OnReplace(object o, params object[] assistant)
        {
            this.mIsStateChanged = true;
            this.NotifyObservers();
            return true;
        }
        public virtual void NotifyObservers()
        {
            if (this.mNeedRefresh)
            {
                if (this.mIsStateChanged)
                {
                    foreach (IObserver observer in this.m_observers)
                    {
                        observer.OnUpdate(this);
                    }
                }
            }
            this.mIsStateChanged = false;
        }
    }

}
