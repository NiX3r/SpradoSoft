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
        static void Main(string[] args)
        {

            // Initialize variables
            LogUtils.LogClassInitialize();
            LogUtils.Log("Sprado program start");

            LogUtils.Log("Start initialize program");

            if(args.Length == 1)
            {
                if (args[0].Equals("-test"))
                {
                    ProgramUtils.IsTest = true;
                }
            }
            else
                ProgramUtils.IsTest = false;

            ProgramUtils.Colors = new Dictionary<string, Color>();
            ProgramUtils.Colors.Add("main", Color.FromArgb(0, 153, 255));
            ProgramUtils.Colors.Add("secondary", Color.FromArgb(0, 49, 81));
            ProgramUtils.Colors.Add("font", Color.FromArgb(240, 240, 240));
            ProgramUtils.Colors.Add("menu", Color.FromArgb(55, 55, 55));
            ProgramUtils.Colors.Add("background", Color.FromArgb(44, 44, 44));

            ProgramUtils.SubForms = new Dictionary<string, Form>();
            ProgramUtils.SubForms.Add("Úvodní strana", new HomeForm());
            ProgramUtils.SubForms.Add("Domy", new HouseForm());
            ProgramUtils.SubForms.Add("Kontakty", new ContactForm());
            ProgramUtils.SubForms.Add("Revizáci", new RevisionManForm());
            ProgramUtils.SubForms.Add("Typy revizí", new RevisionTypeForm());
            ProgramUtils.SubForms.Add("Revize", new RevisionForm());
            ProgramUtils.SubForms.Add("Seznam dle ...", new ListForm());
            ProgramUtils.SubForms.Add("Nastavení", new SettingsForm());

            ProgramUtils.Images = new Dictionary<string, Bitmap>();
            ProgramUtils.Images.Add("add", Sprado.Properties.Resources.add);
            ProgramUtils.Images.Add("remove", Sprado.Properties.Resources.remove);
            ProgramUtils.Images.Add("edit", Sprado.Properties.Resources.edit);
            ProgramUtils.Images.Add("search", Sprado.Properties.Resources.magnifier);
            ProgramUtils.Images.Add("send", Sprado.Properties.Resources.send);
            ProgramUtils.RecolorAllImages("main");

            ProgramUtils.LoggedUser = new Dictionary<string, object>();
            ProgramUtils.LoggedUser.Add("logged", false);

            ProgramUtils.MainUI = new MainFrame();

            LogUtils.Log("Initialize program done");

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
