using MusicLibraryManager.GUI;
using MusicLibraryManager.GUI.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicLibraryManager
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            #if TEST
                Application.Run(new TEST());
            #else
                Application.Run(new MainForm( args));
            #endif
            

        }
    }
}
