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

            listBox2.Items.Add("");

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
    }
}
