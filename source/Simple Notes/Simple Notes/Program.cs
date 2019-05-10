using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using IWshRuntimeLibrary;

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

            object shDesktop = (object)"Desktop";
            WshShell shell = new WshShell();
            string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\simplenotes.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "Simple Notes program by Joshua Wolfsohn";
            shortcut.TargetPath = @"C:\simplenotes\simplenotes.exe";
            shortcut.IconLocation = @"C:\simplenotes\favicon.ico";
            shortcut.WorkingDirectory = @"C:\simplenotes";
            shortcut.Save();

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
