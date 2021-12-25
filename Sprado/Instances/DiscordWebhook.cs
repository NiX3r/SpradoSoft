using Microsoft.Win32;
using Sprado.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Sprado.Instances
{
    class DiscordWebhook
    {

        private readonly WebClient dWebClient;
        private static NameValueCollection discordValues = new NameValueCollection();
        private const string example = "__**ERROR REPORT**__\n" +
                                       "**Time**: `%time%`\n" +
                                       "**Message:** `%message%`\n" +
                                       "**Location:**\n" + 
                                       "  File: `%file%`\n" +
                                       "  Caller: `%caller%`\n" +
                                       "  Line: `%line%`\n" +
                                       "**Date:** `%create%`\n" +
                                       "**OS:** `%os%`\n" +
                                       "**MAC:** ||`%mac%`||\n" +
                                       "**IP:** ||`%ip%`||";

        public string UserName { get; set; }
        public string ProfilePicture { get; set; }

        public DiscordWebhook()
        {
            dWebClient = new WebClient();
            UserName = "Sprado error";
            ProfilePicture = "";
        }


        public void SendMessage(Exception ex, int line, string caller, string file, long time)
        {

            LogUtils.Log("Discord webhook send");

            var macAddr =
                (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();


            string msg = example;
            msg = msg.Replace("%message%", ex.Message)
                        .Replace("%time%", time.ToString())
                        .Replace("%file%", file)
                        .Replace("%caller%", caller)
                        .Replace("%line%", line.ToString())
                        .Replace("%create%", DateTime.Now.ToString("dd.MM.yyyy HH:mm"))
                        .Replace("%os%", FriendlyName())
                        .Replace("%mac%", macAddr)
                        .Replace("%ip%", externalIpString);

            discordValues.Add("username", UserName);
            discordValues.Add("avatar_url", ProfilePicture);
            discordValues.Add("content", msg);

            dWebClient.UploadValues(SecretUtils.GetWebhookURL(), discordValues);

        }

        public void Dispose()
        {
            dWebClient.Dispose();
        }

        public string HKLM_GetString(string path, string key)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
                if (rk == null) return "";
                return (string)rk.GetValue(key);
            }
            catch { return ""; }
        }

        public string FriendlyName()
        {
            string ProductName = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            string CSDVersion = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");
            if (ProductName != "")
            {
                return (ProductName.StartsWith("Microsoft") ? "" : "Microsoft ") + ProductName +
                            (CSDVersion != "" ? " " + CSDVersion : "");
            }
            return "";
        }

    }
}
