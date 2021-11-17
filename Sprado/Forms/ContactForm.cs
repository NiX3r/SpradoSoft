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
        private Dictionary<string, object> SELECTED_DATA;

        public ContactForm()
        {
            InitializeComponent();
        }

        private void ContactForm_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = ProgramUtils.Images["add"];
            pictureBox3.Image = ProgramUtils.Images["remove"];
            pictureBox4.Image = ProgramUtils.Images["edit"];
            pictureBox2.Image = ProgramUtils.Images["search"];
            pictureBox6.Image = ProgramUtils.Images["send"];

            SELECTED_ID = -1;
            SELECTED_DATA = new Dictionary<string, object>();

        }

        private void showData()
        {

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

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (tName.Text.Length > 0 || tFirstname.Text.Length > 0 || tLastname.Text.Length > 0 || tMail.Text.Length > 0 || tPhone.Text.Length > 0 || (lbHouse.SelectedItem != null))
            {
                Dictionary<int, Dictionary<string, object>> response = DatabaseUtils.GetContact(tName.Text,
                                                                                                tFirstname.Text,
                                                                                                tLastname.Text,
                                                                                                tMail.Text,
                                                                                                tPhone.Text.Equals("") ? -1 : Convert.ToInt32(tPhone.Text),
                                                                                                lbHouse.SelectedItem == null ? -1 : Convert.ToInt32(lbHouse.SelectedItem),
                                                                                                rtDescription.Text);
                if(response.Count == 1)
                {
                    SELECTED_ID = response.Keys.ElementAt(0);
                    SELECTED_DATA = response[SELECTED_ID];
                    showData();
                }
                if(response.Count == 0)
                {
                    MessageBox.Show("Je nám líto, ale žádný kontakt s těmito kritérii neexistuje.\n\nTIP: Zkuste si zkontrolovat správnost kritérií.");
                }
                else if(response.Count <= 10)
                {
                    string data = "";
                    foreach(Dictionary<string, object> item in response.Values)
                    {
                        data += "\nNázev jednotky: " + item["Name"] + " ; Jméno: " + item["Firstname"] + " ; Příjmení: " + item["Lastname"] + " ; Email: " + item["Email"];
                    }
                    MessageBox.Show("Podle vyhledaných kritériích existuje " + response.Count + " počet kontaktů. Prosím specifikujte kritéria, pro přesnější vyhledávání! Nalezlé kontakty: \n\n" + data);
                }
                else
                {
                    MessageBox.Show("Podle vyhledaných kritériích existuje " + response.Count + " počet kontaktů. Prosím specifikujte kritéria, pro přesnější vyhledávání!");
                }

            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:" + tMail.Text);
        }
    }
}
