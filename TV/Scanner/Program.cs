using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FutureConcepts.Media.TV.Scanner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0].ToLowerInvariant().Equals("reset"))
            {
                Config.AppUser.Current.SetDefaults();
                Config.AppUser.Current.SaveSettings();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if ((args.Length == 1) && args[0].ToLowerInvariant().Equals("config"))
            {
                Application.Run(new AdvancedSettings());
            }
            else
            {
                Application.Run(new MainForm(args));
            }
        }
    }
}