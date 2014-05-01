using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RGeos.FrameWork.MVC
{
    //控制类
    public interface IController
    {
        string Name { get; }
        IExecutor Executor { get; }
        IObserver Observer { get; }
    }
    public abstract class AbstractController : IController
    {
        protected string mName;
        protected IExecutor mExecutor;
        protected IObserver mObserver;
        //从外部传入被观察者
        //控制器内部构造观察者，并向被观察者注册，观察者和控制器总是成对出现
        protected AbstractController(string name, IExecutor executor)
        {
            this.mName = name;
            this.mExecutor = executor;
        }

        public virtual IExecutor Executor
        {
            get
            {
                return this.mExecutor;
            }
        }

        public virtual string Name
        {
            get
            {
                return this.mName;
            }
        }

        public virtual IObserver Observer
        {
            get
            {
                return this.mObserver;
            }
        }
        //业务逻辑
        public virtual void HandExecuter()
        {
        }
        public virtual bool OnRemove(object o, params object[] assistant)
        {
            return this.mExecutor.OnRemove(o, assistant);
        }
        public virtual bool OnAdd(object o, params object[] assistant)
        {
            return this.mExecutor.OnAdd(o, assistant);
        }
        public virtual bool OnReplace(object o, params object[] assistant)
        {
            return this.mExecutor.OnReplace(o, assistant);
        }

    }
}
