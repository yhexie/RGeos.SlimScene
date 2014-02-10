using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RGeos.AppScene
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
            //Application.Run(new MainFrm());
            MainFrm frm = new MainFrm();
            Application.Idle += new EventHandler(frm.mSceneControl.OnApplicationIdle);
            Application.Run(frm);
        }
    }
}
