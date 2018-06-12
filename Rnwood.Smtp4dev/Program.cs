#region

using System;
using System.Windows.Forms;
using Rnwood.Smtp4dev.Properties;

#endregion

namespace Rnwood.Smtp4dev
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);

            if (Settings.Default.SettingsUpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.SettingsUpgradeRequired = false;
                Settings.Default.Save();
            }

            var form = new MainForm();
            Application.Run(form);
        }
    }
}