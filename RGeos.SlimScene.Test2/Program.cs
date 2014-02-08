using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RGeos.SlimScene.Test2
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
          //  Form1 frm = new Form1();
            FrmPerspectiveCameraTest frm = new FrmPerspectiveCameraTest();
            Application.Idle += new EventHandler(frm.mSceneControl.OnApplicationIdle);
            Application.Run(frm);
        }
    }
}
