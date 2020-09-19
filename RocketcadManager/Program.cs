using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketcadManager
{
    static class Program
    {
        static Mutex mutex = new Mutex(true, "2P9W7OERTASQQO291PJZFF9QE0GZUK29");
        public const int HWND_PTR = 0xffff;
        public static readonly int WINDOW_MSG = RegisterWindowMessage("SHOW_WINDOW");
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);

        [STAThread]
        static void Main()
        {
            // Allow only one window to be open
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm(WINDOW_MSG));
                mutex.ReleaseMutex();
            }
            else
            {
                PostMessage((IntPtr)HWND_PTR, WINDOW_MSG, IntPtr.Zero, IntPtr.Zero);
            }
        }
    }
}
