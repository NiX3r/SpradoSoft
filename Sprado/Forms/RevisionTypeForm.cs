﻿using Sprado.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Sprado.Enums.DatabaseMethodResponseEnum;

namespace Sprado.Forms
{
    public partial class RevisionTypeForm : Form
    {

        private int selectedId = -1;
        private Dictionary<string, object> selectedData = new Dictionary<string, object>();

        public RevisionTypeForm()
        {
            InitializeComponent();
        }

        private void RevisionTypeForm_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = ProgramUtils.Images["add"];
            pictureBox2.Image = ProgramUtils.Images["remove"];
            pictureBox3.Image = ProgramUtils.Images["edit"];
            pictureBox4.Image = ProgramUtils.Images["search"];
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

            if (textBox1.Text != "" && textBox6.Text != "")
            {

                DatabaseResponse response = DatabaseUtils.AddRevisionType(textBox1.Text, Convert.ToInt32(textBox6.Text), richTextBox1.Text);

                if(response == DatabaseResponse.CREATED)
                {
                    MessageBox.Show("Typ revize úšpěšně vytvořena.");
                }

            }
            else
            {
                MessageBox.Show("Prosím vyplň potřebné údaje označené *");
            }

        }
    }
}
