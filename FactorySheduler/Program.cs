using FactorySheduler.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FactorySheduler
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //try
            //{
                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                MainWindow mainWindow = MainWindow.getInstance();
                Controller controller = new Controller(mainWindow);
                Application.Run(mainWindow);
            //}
            //catch (Exception e)
            //{
            //    logException(e.Message, e.StackTrace);
            //    throw e;
            //}
        }

        public static void logException(string content, string stackTrace)
        {
            StreamWriter writer = new StreamWriter("errorlog.txt", true);
            writer.Write(content);
            writer.Write(stackTrace);
            writer.Close();
        }
    }
}
