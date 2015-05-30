using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using _11thLauncher.LogViewer;

namespace _11thLauncher
{
    static class Program
    {
        internal static Form1 form;
        internal static LogViewerForm viewer;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Unhandled exceptions handlers
            AppDomain.CurrentDomain.UnhandledException += Util.CurrentDomainUnhandledException;
            Application.ThreadException += new ThreadExceptionEventHandler(Util.ThreadUnhandledException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
