using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RGeos.SlimScene.Test
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
            MainFrm frm = new MainFrm();

            Application.Idle += new EventHandler(frm.mSceneControl.OnApplicationIdle);
            Application.Run(frm);
        }
    }
}
