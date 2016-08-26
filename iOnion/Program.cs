using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;

namespace iOnion
{
    class Program
    {
        static EventWaitHandle exitWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, "iOnionExitWaitHandle");
        static NotifyIcon trayIcon = new NotifyIcon();
        static EXECUTION_STATE oldExecState;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }

        static void Main(string[] args)
        {
            ThreadPool.QueueUserWorkItem((o) => 
            {
                trayIcon.Text = "iOnion" + Environment.NewLine + "Click to turn off";
                Icon theIcon = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("iOnion.onion-logo.ico"));
                trayIcon.Icon = new Icon(theIcon, 40, 40);
                trayIcon.Click += new EventHandler(trayIcon_Click);
                trayIcon.Visible = true;

                Application.Run();
            });

            oldExecState = SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);            

            exitWaitHandle.WaitOne();

            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }

        static void trayIcon_Click(object sender, EventArgs e)
        {
            trayIcon.Dispose();
            exitWaitHandle.Set();
        }
    }
}
