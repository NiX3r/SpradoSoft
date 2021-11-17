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
    public partial class ContactForm : Form
    {

        private int SELECTED_ID;

        public ContactForm()
        {
            InitializeComponent();
        }

        private void ContactForm_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = ProgramUtils.Images["add"];
            pictureBox2.Image = ProgramUtils.Images["remove"];
            pictureBox3.Image = ProgramUtils.Images["edit"];
            pictureBox4.Image = ProgramUtils.Images["search"];
            pictureBox6.Image = ProgramUtils.Images["send"];

            SELECTED_ID = -1;

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if(!tName.Text.Equals("") && !tFirstname.Text.Equals("") && !tLastname.Text.Equals("") && !tMail.Text.Equals(""))
            {
                DatabaseResponse response = DatabaseUtils.AddContact(tName.Text, 
                                                                    tFirstname.Text, 
                                                                    tLastname.Text, 
                                                                    tMail.Text, 
                                                                    tPhone.Text.Equals("") ? -1 : Convert.ToInt32(tPhone.Text), 
                                                                    lbHouse.SelectedItem == null ? -1 : Convert.ToInt32(lbHouse.SelectedItem),
                                                                    cbOwner.Checked,
                                                                    rtDescription.Text.Equals("") ? null : rtDescription.Text);

                switch (response)
                {
                    case DatabaseResponse.CREATED:
                        MessageBox.Show("Úspěšně jsi vytvořil kontakt!");
                        break;
                    case DatabaseResponse.BAD_INPUT:
                        MessageBox.Show("Bohužel zadal jsi špatná vstupní data!");
                        break;
                    case DatabaseResponse.BAD_VERIFICATION:
                        MessageBox.Show("Špatné ověření. Prosím kontaktujte správce aplikace!");
                        break;
                }

            }
        }
    }
}
