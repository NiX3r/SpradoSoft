using Sprado.Forms;
using Sprado.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sprado
{
    public partial class MainFrame : Form
    {

        private Button SELECTED_BUTTON;
        private Form CURRENT_FORM;

        public MainFrame()
        {
            
            InitializeComponent();

            openForm(new LoginForm());
            label2.ForeColor = ProgramUtils.Colors["main"];

        }

        private void controlButton_Click(object sender, EventArgs e)
        {
            if((bool)ProgramUtils.LoggedUser["logged"])
            {
                selectButton((Button)sender);
                openForm(ProgramUtils.SubForms[((Button)sender).Text]);
            }
        }

        public void openForm(Form form)
        {
            LogUtils.Log($"Start open child form");
            // Setup child form
            if (CURRENT_FORM != null)
            {
                CURRENT_FORM.Hide();
            }
            CURRENT_FORM = form;
            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            subFormPanel.Controls.Add(form);
            subFormPanel.Tag = form;
            form.BringToFront();
            form.Show();
        }

        private void selectButton(Button button)
        {

            SELECTED_BUTTON.ForeColor = ProgramUtils.Colors["font"];
            SELECTED_BUTTON = button;
            SELECTED_BUTTON.ForeColor = ProgramUtils.Colors["main"];

        }

        private void controlButton_MouseEnter(object sender, EventArgs e)
        {

            ((Button)sender).Size = new Size(((Button)sender).Size.Width, 45);
            ((Button)sender).BackColor = ProgramUtils.Colors["secondary"];

        }

        private void controlButton_MouseLeave(object sender, EventArgs e)
        {
            ((Button)sender).Size = new Size(((Button)sender).Size.Width, 40);
            ((Button)sender).BackColor = ProgramUtils.Colors["menu"];
        }

        private void MainFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            LogUtils.Log("Sprado program end");
            LogUtils.Save();
            Application.Exit();
        }

        public void RefreshColors()
        {
            LogUtils.Log("Refreshing form colors");
            panel1.BackColor = ProgramUtils.Colors["menu"];
            subFormPanel.BackColor = ProgramUtils.Colors["background"];
        }

        public void UpdateUser()
        {
            label2.Text = ProgramUtils.LoggedUser["firstname"] + " " + ProgramUtils.LoggedUser["lastname"];
            pictureBox1.ImageLocation = (string)ProgramUtils.LoggedUser["profile"];
        }

        public void Login()
        {
            SELECTED_BUTTON = buttonHome;
            CURRENT_FORM = ProgramUtils.SubForms[SELECTED_BUTTON.Text];
            selectButton(SELECTED_BUTTON);
            openForm(ProgramUtils.SubForms[SELECTED_BUTTON.Text]);
            UpdateUser();
        }

    }
}