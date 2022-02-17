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
    /// <summary>
    /// TODO - Add house selection
    /// </summary>
    public partial class ContactForm : Form
    {

        private int SELECTED_ID;
        private Dictionary<string, object> SELECTED_DATA;
        private Dictionary<int, string> houses = new Dictionary<int, string>();

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

            houses = DatabaseUtils.GetHouses();

            foreach(string item in houses.Values)
            {
                lbHouse.Items.Add(item);
            }

        }

        private void showData()
        {
            MessageBox.Show(SELECTED_DATA["House_ID"].GetType().Name);
            tName.Text = SELECTED_DATA["Name"].ToString();
            tFirstname.Text = SELECTED_DATA["Firstname"].ToString();
            tLastname.Text = SELECTED_DATA["Lastname"].ToString();
            tMail.Text = SELECTED_DATA["Email"].ToString();
            tPhone.Text = SELECTED_DATA["Phone"].ToString();
            rtDescription.Text = SELECTED_DATA["Description"].ToString();
            cbOwner.Checked = (bool)SELECTED_DATA["IsOwner"];
            tCreateAuthor.Text = SELECTED_DATA["CreateAuthor"].ToString();
            tCreateDate.Text = ((DateTime)SELECTED_DATA["CreateDate"]).ToString("yyyy-MM-dd HH:mm:ss");
            tLastEditAuthor.Text = SELECTED_DATA["LastEditAuthor"].ToString();
            tLastEditDate.Text = ((DateTime)SELECTED_DATA["LastEditDate"]).ToString("yyyy-MM-dd HH:mm:ss");
            lbHouse.SelectedItem = SELECTED_DATA["House_ID"].GetType().Name.Equals("DBNull") ? null : houses[(int)SELECTED_DATA["House_ID"]];
        }

        private void clearData()
        {
            tName.Text = tFirstname.Text = tLastname.Text = tMail.Text = tPhone.Text = rtDescription.Text = tCreateAuthor.Text = tCreateDate.Text = tLastEditDate.Text = tLastEditAuthor.Text = "";
            lbHouse.SelectedItem = null;
            SELECTED_ID = -1;
            SELECTED_DATA = new Dictionary<string, object>();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if(!tName.Text.Equals("") && !tFirstname.Text.Equals("") && !tLastname.Text.Equals("") && !tMail.Text.Equals(""))
            {

                int houseId = -1;
                foreach(int item in houses.Keys)
                {
                    if (houses[item].Equals(lbHouse.SelectedItem))
                        houseId = item;
                }

                DatabaseResponse response = DatabaseUtils.AddContact(tName.Text, 
                                                                    tFirstname.Text, 
                                                                    tLastname.Text,
                                                                    tMail.Text, 
                                                                    tPhone.Text.Equals("") ? -1 : Convert.ToInt32(tPhone.Text), 
                                                                    houseId,
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
                    case DatabaseResponse.ERROR:
                        MessageBox.Show("Nastala chyba programu. Chyba jiz odeslana, prosim vyckejte na spravce aplikace!");
                        break;
                }

            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (tName.Text.Length > 0 || tFirstname.Text.Length > 0 || tLastname.Text.Length > 0 || tMail.Text.Length > 0 || tPhone.Text.Length > 0 || (lbHouse.SelectedItem != null))
            {

                int houseId = -1;
                foreach (int item in houses.Keys)
                {
                    if (houses[item].Equals(lbHouse.SelectedItem))
                        houseId = item;
                }

                Dictionary<int, Dictionary<string, object>> response = DatabaseUtils.GetContact(tName.Text,
                                                                                                tFirstname.Text,
                                                                                                tLastname.Text,
                                                                                                tMail.Text,
                                                                                                tPhone.Text.Equals("") ? -1 : Convert.ToInt32(tPhone.Text),
                                                                                                houseId);
                if(response.Count == 1)
                {
                    SELECTED_ID = response.Keys.ElementAt(0);
                    SELECTED_DATA = response[SELECTED_ID];
                    showData();
                }
                else if(response.Count == 0)
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

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if(SELECTED_ID >= 0)
            {
                if (MessageBox.Show("Opravdu si přejete odstranit tento kontakt?", "Odstranění dat", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DatabaseResponse response = DatabaseUtils.RemoveContact(SELECTED_ID);
                    switch (response)
                    {
                        case DatabaseResponse.REMOVED:
                            MessageBox.Show("Úspěšně jsi smazal kontakt!");
                            clearData();
                            break;
                        case DatabaseResponse.BAD_INPUT:
                            MessageBox.Show("Bohužel zadal jsi špatná vstupní data!");
                            break;
                        case DatabaseResponse.BAD_VERIFICATION:
                            MessageBox.Show("Špatné ověření. Prosím kontaktujte správce aplikace!");
                            break;
                        case DatabaseResponse.ERROR:
                            MessageBox.Show("Nastala chyba programu. Chyba jiz odeslana, prosim vyckejte na spravce aplikace!");
                            break;
                    }
                }
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if(SELECTED_ID >= 0)
            {
                if (!tName.Text.Equals("") && !tFirstname.Text.Equals("") && !tLastname.Text.Equals("") && !tMail.Text.Equals(""))
                {

                    int houseId = -1;
                    foreach (int item in houses.Keys)
                    {
                        if (houses[item].Equals(lbHouse.SelectedItem))
                            houseId = item;
                    }

                    DatabaseResponse response = DatabaseUtils.EditContact(SELECTED_ID,
                                                                    tName.Text,
                                                                    tFirstname.Text,
                                                                    tLastname.Text,
                                                                    tMail.Text,
                                                                    tPhone.Text.Equals("") ? -1 : Convert.ToInt32(tPhone.Text),
                                                                    houseId,
                                                                    cbOwner.Checked,
                                                                    rtDescription.Text.Equals("") ? null : rtDescription.Text);

                    switch (response)
                    {
                        case DatabaseResponse.EDITED:
                            MessageBox.Show("Úspěšně jsi upravil kontakt!");
                            break;
                        case DatabaseResponse.BAD_INPUT:
                            MessageBox.Show("Bohužel zadal jsi špatná vstupní data!");
                            break;
                        case DatabaseResponse.BAD_VERIFICATION:
                            MessageBox.Show("Špatné ověření. Prosím kontaktujte správce aplikace!");
                            break;
                        case DatabaseResponse.ERROR:
                            MessageBox.Show("Nastala chyba programu. Chyba jiz odeslana, prosim vyckejte na spravce aplikace!");
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Povinné údaje bohužel nelze smazat.");
                }
            }
        }
    }
}
