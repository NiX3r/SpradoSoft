using Sprado.Forms;
using Sprado.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sprado
{
    static class Program
    {

        [STAThread]
        static void Main()
        {

            // Initialize variables
            LogUtils.LogClassInitialize();
            LogUtils.Log("Sprado program start");

            LogUtils.Log("Start initialize program");
            ProgramUtils.Colors = new Dictionary<string, Color>();
            ProgramUtils.Colors.Add("MAIN", Color.FromArgb(0, 153, 255));
            ProgramUtils.Colors.Add("SECONDARY", Color.FromArgb(0, 49, 81));
            ProgramUtils.Colors.Add("FONT", Color.FromArgb(240, 240, 240));
            ProgramUtils.Colors.Add("MENU", Color.FromArgb(55, 55, 55));
            ProgramUtils.Colors.Add("BACKGROUND", Color.FromArgb(44, 44, 44));

            ProgramUtils.SubForms = new Dictionary<string, Form>();
            ProgramUtils.SubForms.Add("Úvodní strana", new HomeForm());
            ProgramUtils.SubForms.Add("Domy", new HouseForm());
            ProgramUtils.SubForms.Add("Kontakty", new ContactForm());
            ProgramUtils.SubForms.Add("Revizáci", new RevisionManForm());
            ProgramUtils.SubForms.Add("Typy revizí", new RevisionTypeForm());
            ProgramUtils.SubForms.Add("Revize", new RevisionForm());
            ProgramUtils.SubForms.Add("Seznam dle ...", new ListForm());
            ProgramUtils.SubForms.Add("Nastavení", new SettingsForm());

            ProgramUtils.LoggedUser = new Dictionary<string, object>();
            ProgramUtils.LoggedUser.Add("LOGGED", false);

            ProgramUtils.MainUI = new MainFrame();

            // Check database connection
            if (DatabaseUtils.OpenConnection())
            {
                LogUtils.Log("Database successfully connected");
            }
            else
            {
                LogUtils.Log("Database unsuccessfully connected. Close program");
                Application.Exit();
            }

            // Load main ui
            LogUtils.Log("Main frame start");
            Application.EnableVisualStyles();
            Application.Run(ProgramUtils.MainUI);

        }

    }
}
