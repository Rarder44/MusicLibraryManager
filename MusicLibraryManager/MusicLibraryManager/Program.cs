using ExtendCSharp.Services;
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

            ServicesManager.RegistService(new SystemService());
            ServicesManager.RegistService(new RegisteredFileTypeService());
            ServicesManager.RegistService(new JsonService());
            ServicesManager.RegistService(new ZipService());
            ServicesManager.RegistService(new MD5Service());
            ServicesManager.RegistService(new FFmpeg());
            ServicesManager.RegistService(new FormService());
            #if TEST
                Application.Run(new TEST());
            #else
                Application.Run(new MainForm( args));
            #endif
            

        }
    }
}
