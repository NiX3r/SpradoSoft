using Sprado.Forms.ListSubForms;
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
    public partial class ListForm : Form
    {

        private Button lastButton;

        public ListForm()
        {
            lastButton = null;
            InitializeComponent();
        }

        private void openForm(Form form)
        {
            LogUtils.Log($"Start open child form");
            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            panel2.Controls.Add(form);
            panel2.Tag = form;
            form.BringToFront();
            form.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (lastButton != null)
                lastButton.BackColor = Color.FromArgb(66, 66, 66);
            lastButton = (Button)sender;
            lastButton.BackColor = Color.FromArgb(88,88,88);
            openForm(new ContactSubForm());
        }
    }
}
