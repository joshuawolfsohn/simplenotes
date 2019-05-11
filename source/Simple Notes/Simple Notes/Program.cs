using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Simple_Notes
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                if (System.IO.File.Exists("C:\\simplenotes\\simplenotes.db"))
                {
                    Application.Run(new Main());
                }
                else
                {
                    Application.Run(new CreateDB());
                }
            } catch
            {
                MessageBox.Show("Missing DLLs!");
            }

        }
    }
}
