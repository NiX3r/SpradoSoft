﻿using Sprado.Instances;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sprado.Utils
{
    class ProgramUtils
    {

        // Main frame variable
        public static MainFrame MainUI { get; set; }
        public static Color MainColor { get; set; }
        public static Dictionary<string, Color> Colors { get; set; }
        public static Dictionary<string, Form> SubForms { get; set; }
        public static Dictionary<string, object> LoggedUser { get; set; }

        private static DiscordWebhook WEBHOOK = new DiscordWebhook();

        /// <summary>
        /// Method for save error log and send it through discord webhook
        /// </summary>
        /// <param name="ex"> specific error </param>
        public static void ExceptionThrowned(Exception ex, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = "null", [CallerFilePath] string file = "null")
        {

            long time = DateTime.Now.Ticks;
            LogUtils.Log("ERROR >> " + ex.Message);
            WEBHOOK.SendMessage(ex, lineNumber, caller, file, time);
            MessageBox.Show($"Chyba!\nNeboj, kód chyby byl odeslán vývojáři. Vyřešíme ji co nejdříve budeme moct!\nČas chyby: {time}\n\nDěkujeme za strpení.");

        }

        public static Bitmap RecolorImage(Bitmap image, string colorKey)
        {
            Bitmap bm = image;
            for (int y = 0; y < bm.Height; y++)
            {
                for (int x = 0; x < bm.Width; x++)
                {
                    if (bm.GetPixel(x, y).A > 0)
                        bm.SetPixel(x, y, ProgramUtils.Colors[colorKey]);
                }
            }
            return bm;
        }

    }
}
