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
                button1.ForeColor = ProgramUtils.Colors["MAIN"];
                button2.ForeColor = ProgramUtils.Colors["FONT"];
                label3.Hide();
                label4.Hide();
                label6.Hide();
                textBox3.Hide();
                textBox4.Hide();
                textBox6.Hide();
            }
            else
            {
                button2.ForeColor = ProgramUtils.Colors["MAIN"];
                button1.ForeColor = ProgramUtils.Colors["FONT"];
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

        }

        private void register()
        {

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
    }
}
