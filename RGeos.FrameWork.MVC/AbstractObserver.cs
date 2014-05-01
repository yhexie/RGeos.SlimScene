using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RGeos.FrameWork.MVC
{
    //观察者
    public interface IObserver
    {
        IController Controller { get; }
        IExecutor Executor { get; }
        string Name { get; }

        void InitializeObserver();
        //更新观察者
        void OnUpdate(IExecutor currentExecutor);
    }
    public abstract class AbstractObserver : IObserver
    {
        protected IController mController;
        protected IExecutor mExecutor;
        protected string mName;

        public abstract void InitializeObserver();
        public abstract void OnUpdate(IExecutor currentExecutor);

        public virtual IController Controller
        {
            get
            {
                return this.mController;
            }
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
        //观察者通过控制器引用，调用被观察者的方法，更新其他的观察者。
        //此时观察者和被观察者不通信
        protected AbstractObserver(string name, IController controller)
        {
            this.mName = name;
            this.mController = controller;
            this.mExecutor = controller.Executor;
        }
        //在控制器中构造观察者时，将被观察者传入观察者，这样观察者可以调用被观察者中的数据。
        //此时观察者和被观察者通信
        protected AbstractObserver(string name, IController controller, IExecutor executor)
        {
            this.mName = name;
            this.mController = controller;
            this.mExecutor = executor;
        }
    }

}
