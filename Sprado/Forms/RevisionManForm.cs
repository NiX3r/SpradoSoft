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

        private int selectedId = -1;
        private Dictionary<string, object> selectedData = new Dictionary<string, object>();

        public RevisionManForm()
        {
            InitializeComponent();
        }

        private void RevisionManForm_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = ProgramUtils.Images["add"];
            pictureBox3.Image = ProgramUtils.Images["remove"];
            pictureBox4.Image = ProgramUtils.Images["edit"];
            pictureBox2.Image = ProgramUtils.Images["search"];
        }

        private void showData()
        {
            textBox1.Text = selectedData["Company"].ToString();
            textBox2.Text = selectedData["Firstname"].ToString();
            textBox3.Text = selectedData["Lastname"].ToString();
            textBox6.Text = selectedData["Email"].ToString();
            textBox4.Text = selectedData["Phone"].ToString();
            richTextBox1.Text = selectedData["Description"].ToString();
            textBox7.Text = ((DateTime)selectedData["CreateDate"]).ToString("yyyy-MM-dd HH:mm:ss");
            textBox10.Text = selectedData["CreateAuthor"].ToString();
            textBox9.Text = ((DateTime)selectedData["LastEditDate"]).ToString("yyyy-MM-dd HH:mm:ss");
            textBox8.Text = selectedData["LastEditAuthor"].ToString();
        }

        private void clearData()
        {
            textBox1.Text = textBox2.Text = textBox3.Text = 
                textBox6.Text = textBox4.Text = textBox7.Text = 
                textBox10.Text = textBox9.Text = textBox8.Text = richTextBox1.Text = "";
            selectedId = -1;
            selectedData = new Dictionary<string, object>();
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

        private void pictureBox2_Click(object sender, EventArgs e)
        {

            if(textBox1.Text.Length > 0 || textBox2.Text.Length > 0 || textBox3.Text.Length > 0 ||
                textBox4.Text.Length > 0 || textBox6.Text.Length > 0)
            {

                Dictionary<int, Dictionary<string, object>> response = DatabaseUtils.GetRevisionMan(textBox1.Text,
                                                                                                    textBox2.Text,
                                                                                                    textBox3.Text,
                                                                                                    textBox6.Text,
                                                                                                    textBox4.Text == "" ? -1 : Convert.ToInt32(textBox4.Text));

                if(response.Count == 1)
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
                        data += $"\n{item["Firstname"]} {item["Lastname"]}, Tel.: {item["Phone"]}, Email: {item["Email"]}, {item["Company"]}";
                    }
                    MessageBox.Show("Podle vyhledaných kritériích existuje " + response.Count + " počet domů. Prosím specifikujte kritéria, pro přesnější vyhledávání! Nalezlé domy: \n\n" + data);
                }
                else
                {
                    MessageBox.Show("Podle vyhledaných kritériích existuje " + response.Count + " počet domů. Prosím specifikujte kritéria, pro přesnější vyhledávání!");
                }

            }

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (selectedId >= 0)
            {

                if (MessageBox.Show("Opravdu si přejete odstranit tohoto revizáka?", "Odstranění dat", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DatabaseResponse response = DatabaseUtils.RemoveRevisionMan(selectedId);
                    switch (response)
                    {
                        case DatabaseResponse.REMOVED:
                            MessageBox.Show("Úspěšně jsi smazal revizáka!");
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

            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" &&
               textBox4.Text != "" && textBox6.Text != "")
            {
                
                string company = textBox1.Text, firstname = textBox2.Text, lastname = textBox3.Text, email = textBox6.Text, description = richTextBox1.Text;
                int phone = Convert.ToInt32(textBox4.Text);

                if (company == selectedData["Company"].ToString()) company = "";
                if (firstname == selectedData["Firstname"].ToString()) firstname = "";
                if (lastname == selectedData["Lastname"].ToString()) lastname = "";
                if (email == selectedData["Email"].ToString()) email = "";
                if (description == selectedData["Description"].ToString()) description = "";
                if (phone == (int)selectedData["Phone"]) phone = -1;

                DatabaseResponse response = DatabaseUtils.EditRevisionMan(selectedId, 
                                                                          company, 
                                                                          firstname, 
                                                                          lastname, 
                                                                          phone, 
                                                                          email, 
                                                                          description);

                if(response == DatabaseResponse.EDITED)
                {
                    MessageBox.Show("Úspěšně jsi upravil revizáka.");
                }

            }

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:" + textBox6.Text);
        }
    }
}
