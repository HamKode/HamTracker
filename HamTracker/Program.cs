using System;
using System.Drawing;
using System.Windows.Forms;
using HamTracker.Database;
using HamTracker.Forms;

namespace HamTracker
{
    static class Program
    {
        public static Icon AppIcon { get; private set; }

        [STAThread]
        static void Main()
        {
            DatabaseManager.InitializeDatabase();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Icon load karo
            try
            {
                AppIcon = new Icon("hamtracker_favicon.ico");
            }
            catch
            {
                AppIcon = SystemIcons.Shield;
            }

            // Step 1 — Login
            using (var login = new LoginForm())
            {
                if (login.ShowDialog() != DialogResult.OK)
                    return;
            }

            // Step 2 — Splash
            using (var splash = new SplashScreen())
            {
                splash.ShowDialog();
            }

            // Step 3 — Main app
            Application.Run(new MainForm());
        }
    }
}