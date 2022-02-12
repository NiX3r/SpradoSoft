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
    public partial class HouseForm : Form
    {

        private Dictionary<string, int> contacts = new Dictionary<string, int>();

        public HouseForm()
        {
            InitializeComponent();
        }

        private void HouseForm_Load(object sender, EventArgs e)
        {

            pictureBox1.Image = ProgramUtils.Images["add"];
            pictureBox2.Image = ProgramUtils.Images["remove"];
            pictureBox3.Image = ProgramUtils.Images["edit"];
            pictureBox4.Image = ProgramUtils.Images["search"];
            pictureBox5.Image = pictureBox6.Image = ProgramUtils.Images["send"];

            contacts = DatabaseUtils.GetContacts();

            foreach(string s in contacts.Keys)
            {
                listBox1.Items.Add(s);
            }

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach(string s in contacts.Keys)
            {
                if (s.Contains(textBox6.Text))
                {
                    listBox1.Items.Add(s);
                }
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

            if(listBox1.SelectedItem != null && listBox2.SelectedItem != null)
            {
                string address = textBox1.Text, city = textBox3.Text, description = richTextBox1.Text;
                int addressNo = Convert.ToInt32(textBox2.Text), zipCode = Convert.ToInt32(textBox4.Text), flatsCount = Convert.ToInt32(textBox5.Text), ownerId = contacts[listBox1.SelectedItem.ToString()], type = listBox2.SelectedIndex;

                DatabaseResponse databaseResponse = DatabaseUtils.AddHouse(address, zipCode, description, zipCode, flatsCount, type, ownerId, description);

                if(databaseResponse == DatabaseResponse.CREATED)
                {
                    MessageBox.Show("Dům úspěšně přidán.");
                }

            }

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

            if (textBox1.Text.Length > 0 || textBox2.Text.Length > 0 || textBox3.Text.Length > 0 || 
                textBox4.Text.Length > 0 || (listBox1.SelectedItem != null))
            {



            }

        }
    }
}
