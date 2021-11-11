using Sprado.Instances;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        public static void ExceptionThrowned(Exception ex)
        {

            // Save error in log
            LogUtils.Log("ERROR >> " + ex.Message);
            // Send error to discord webhook
            WEBHOOK.SendMessage(ex);

        }

    }
}
