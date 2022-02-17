using Sprado.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        private int selectedId = -1;
        private Dictionary<string, object> selectedData = new Dictionary<string, object>();

        public HouseForm()
        {
            InitializeComponent();
        }

        private void HouseForm_Load(object sender, EventArgs e)
        {

            pictureBox1.Image = ProgramUtils.Images["add"];
            pictureBox3.Image = ProgramUtils.Images["remove"];
            pictureBox4.Image = ProgramUtils.Images["edit"];
            pictureBox2.Image = ProgramUtils.Images["search"];
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

        private void showData()
        {
            textBox6.Text = "";
            textBox1.Text = selectedData["Street"].ToString();
            textBox2.Text = selectedData["StreetNo"].ToString();
            textBox3.Text = selectedData["City"].ToString();
            textBox4.Text = selectedData["ZipCode"].ToString();
            textBox5.Text = selectedData["Flats"].ToString();
            textBox10.Text = selectedData["CreateAuthor"].ToString();
            textBox7.Text = ((DateTime)selectedData["CreateDate"]).ToString("yyyy-MM-dd HH:mm:ss");
            textBox8.Text = selectedData["LastEditAuthor"].ToString();
            textBox9.Text = ((DateTime)selectedData["LastEditDate"]).ToString("yyyy-MM-dd HH:mm:ss");
            listBox2.SelectedIndex = (int)selectedData["Type"];
            foreach(string item in contacts.Keys)
            {
                if(contacts[item] == (int)selectedData["Owner"])
                    listBox1.SelectedItem = item;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

            if(listBox1.SelectedItem != null && listBox2.SelectedItem != null && textBox1.Text != "" &&
               textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && textBox5.Text != "")
            {
                string address = textBox1.Text, city = textBox3.Text, description = richTextBox1.Text;
                int addressNo = Convert.ToInt32(textBox2.Text), zipCode = Convert.ToInt32(textBox4.Text), flatsCount = Convert.ToInt32(textBox5.Text), ownerId = contacts[listBox1.SelectedItem.ToString()], type = listBox2.SelectedIndex;

                DatabaseResponse databaseResponse = DatabaseUtils.AddHouse(city, zipCode, address, zipCode, flatsCount, type, ownerId, description);

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

                Dictionary<int, Dictionary<string, object>> response = DatabaseUtils.GetHouse(textBox1.Text,
                                                                                              textBox2.Text == "" ? -1 : Convert.ToInt32(textBox2.Text),
                                                                                              textBox3.Text,
                                                                                              textBox4.Text == "" ? -1 : Convert.ToInt32(textBox4.Text),
                                                                                              listBox1.SelectedItem == null ? -1 : contacts[listBox1.SelectedItem.ToString()]);

                if (response.Count == 1)
                {
                    selectedId = response.Keys.ElementAt(0);
                    selectedData = response[selectedId];
                    showData();
                }
                else if (response.Count == 0)
                {
                    MessageBox.Show("Je nám líto, ale žádný kontakt s těmito kritérii neexistuje.\n\nTIP: Zkuste si zkontrolovat správnost kritérií.");
                }
                else if (response.Count <= 10)
                {
                    string data = "";
                    foreach (Dictionary<string, object> item in response.Values)
                    {
                        data += $"\nAdresa: {item["Street"]} {item["StreetNo"]}, {item["City"]} {item["ZipCode"]}";
                    }
                    MessageBox.Show("Podle vyhledaných kritériích existuje " + response.Count + " počet domů. Prosím specifikujte kritéria, pro přesnější vyhledávání! Nalezlé domy: \n\n" + data);
                }
                else
                {
                    MessageBox.Show("Podle vyhledaných kritériích existuje " + response.Count + " počet domů. Prosím specifikujte kritéria, pro přesnější vyhledávání!");
                }

            }

        }

        private void clearData()
        {
            textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = textBox6.Text = textBox7.Text = textBox8.Text = textBox9.Text = textBox10.Text = "";
            selectedId = -1;
            selectedData = new Dictionary<string, object>();
            listBox1.SelectedItem = listBox2.SelectedItem = null;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

            if(selectedId >= 0)
            {

                if (MessageBox.Show("Opravdu si přejete odstranit tento dům?", "Odstranění dat", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DatabaseResponse response = DatabaseUtils.RemoveHouse(selectedId);
                    switch (response)
                    {
                        case DatabaseResponse.REMOVED:
                            MessageBox.Show("Úspěšně jsi smazal kontakt!");
                            clearData();
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
            if (listBox1.SelectedItem != null && listBox2.SelectedItem != null && textBox1.Text != "" &&
               textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && textBox5.Text != "")
            {
                string address = textBox1.Text, city = textBox3.Text, description = richTextBox1.Text;
                int addressNo = Convert.ToInt32(textBox2.Text), zipCode = Convert.ToInt32(textBox4.Text), flatsCount = Convert.ToInt32(textBox5.Text), ownerId = contacts[listBox1.SelectedItem.ToString()], type = listBox2.SelectedIndex;

                if (address.Equals(selectedData["Street"])) address = null;
                if (city.Equals(selectedData["City"])) city = null;
                if (description.Equals("Description")) description = null;
                if (addressNo == (int)selectedData["StreetNo"]) addressNo = -1;
                if (zipCode == (int)selectedData["ZipCode"]) zipCode = -1;
                if (flatsCount == (int)selectedData["Flats"]) flatsCount = -1;
                if (ownerId == (int)selectedData["Owner"]) ownerId = -1;
                if (type == (int)selectedData["Type"]) type = -1;

                DatabaseResponse databaseResponse = DatabaseUtils.EditHouse(selectedId, city, zipCode, address, zipCode, flatsCount, type, ownerId, description);

                if (databaseResponse == DatabaseResponse.EDITED)
                {
                    MessageBox.Show("Dům úspěšně upraven.");
                }

            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

            if(selectedId != -1)
            {
                
                List<string> contacts = DatabaseUtils.GetContactEmailsByHouseID(selectedId);
                string cmd = "mailto:?bcc=";

                foreach(string mail in contacts)
                {
                    cmd += mail + ",";
                }
                cmd = cmd.Substring(0, cmd.Length - 1);

                Process.Start(cmd);

            }

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            if (selectedId != -1)
            {

                List<string> contacts = DatabaseUtils.GetOwnerContactEmailsByHouseID(selectedId);
                string cmd = "mailto:?bcc=";

                foreach (string mail in contacts)
                {
                    cmd += mail + ",";
                }
                cmd = cmd.Substring(0, cmd.Length - 1);

                Process.Start(cmd);

            }
        }
    }
}
