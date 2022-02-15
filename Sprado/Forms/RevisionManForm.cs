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
using static Sprado.Enums.DatabaseMethodResponseEnum;

namespace Sprado.Forms
{
    public partial class RevisionManForm : Form
    {
        public RevisionManForm()
        {
            InitializeComponent();
        }

        private void RevisionManForm_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = ProgramUtils.Images["add"];
            pictureBox2.Image = ProgramUtils.Images["remove"];
            pictureBox3.Image = ProgramUtils.Images["edit"];
            pictureBox4.Image = ProgramUtils.Images["search"];
            pictureBox5.Image = ProgramUtils.Images["send"];
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

            if(textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" &&
               textBox4.Text != "" && textBox6.Text != "")
            {

                DatabaseResponse response = DatabaseUtils.AddRevisionMan(textBox1.Text, 
                                                                         textBox2.Text, 
                                                                         textBox3.Text, 
                                                                         Convert.ToInt32(textBox4.Text), 
                                                                         textBox6.Text, 
                                                                         richTextBox1.Text);
                if(response == DatabaseResponse.CREATED)
                {
                    MessageBox.Show("Revizák úspěšně vytvořen");
                }

            }
            else
            {
                MessageBox.Show("Prosím vyplň potřebné údaje označené *");
            }

        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
