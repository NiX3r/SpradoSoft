using Sprado.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sprado.Forms
{
    public partial class LoginForm : Form
    {

        private bool IS_LOGIN;

        public LoginForm()
        {
            InitializeComponent();

            changeMode(true);

        }

        private void changeMode(bool isLogin)
        {

            IS_LOGIN = isLogin;

            if (isLogin)
            {
                button1.ForeColor = ProgramUtils.Colors["main"];
                button2.ForeColor = ProgramUtils.Colors["font"];
                label3.Hide();
                label4.Hide();
                label6.Hide();
                textBox3.Hide();
                textBox4.Hide();
                textBox6.Hide();
            }
            else
            {
                button2.ForeColor = ProgramUtils.Colors["main"];
                button1.ForeColor = ProgramUtils.Colors["font"];
                label3.Show();
                label4.Show();
                label6.Show();
                textBox3.Show();
                textBox4.Show();
                textBox6.Show();
            }

        }

        private void login()
        {
            LogUtils.Log("Start login user");
            Dictionary<string, object> result = DatabaseUtils.GetUser(textBox1.Text, PasswordEncryption(textBox2.Text));
            if (result != null)
            {
                ProgramUtils.LoggedUser.Add("id", result["id"]);
                ProgramUtils.LoggedUser.Add("firstname", result["firstname"]);
                ProgramUtils.LoggedUser.Add("lastname", result["lastname"]);
                ProgramUtils.LoggedUser.Add("admin", result["admin"]);
                ProgramUtils.LoggedUser.Add("profile", result["profile"]);
                ProgramUtils.LoggedUser.Add("email", textBox1.Text);
                ProgramUtils.LoggedUser["logged"] = true;
                ProgramUtils.MainUI.Login();
                return;
            }
        }

        private void register()
        {
            LogUtils.Log("Start register user");
            if (!DatabaseUtils.ExistsUser(textBox1.Text))
            {
                if(DatabaseUtils.CreateUser(textBox4.Text, textBox3.Text, textBox1.Text, Convert.ToInt32(textBox6.Text), PasswordEncryption(textBox2.Text)))
                {
                    MessageBox.Show("Uživatel úspěšně vytvořen!");
                    textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox6.Text = "";
                }
                return;
            }
            MessageBox.Show("Bohužel uživatel se stejným emailem již existuje.\n\nTIP: Napište jiný email nebo upravte uživatele");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (IS_LOGIN)
                login();
            else
                changeMode(true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!IS_LOGIN)
                register();
            else
                changeMode(false);
        }

        private string PasswordEncryption(string password, bool second = false)
        {
            string salt1 = "6&eL#YwFJFqD";
            string salt2 = "zyQ@^cVX9H67";
            string saltedPassword = salt1 + password + salt2;

            var sha1 = new System.Security.Cryptography.SHA256Managed();
            var plaintextBytes = Encoding.UTF8.GetBytes(saltedPassword);
            var hashBytes = sha1.ComputeHash(plaintextBytes);

            var sb = new StringBuilder();
            foreach (var hashByte in hashBytes)
            {
                sb.AppendFormat("{0:x2}", hashByte);
            }
            var hashString = sb.ToString();
            if (second)
                return hashString;
            else
                return PasswordEncryption(hashString, true);
        }

    }
}
