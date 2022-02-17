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
    public partial class RevisionForm : Form
    {


        private int selectedId = -1;
        private Dictionary<string, object> selectedData = new Dictionary<string, object>();
        private Dictionary<int, string> houses = new Dictionary<int, string>(), types = new Dictionary<int, string>(), revisionMen = new Dictionary<int, string>();

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach(string item in houses.Values)
            {
                if (item.Contains(textBox6.Text))
                    listBox1.Items.Add(item);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            foreach (string item in types.Values)
            {
                if (item.Contains(textBox1.Text))
                    listBox2.Items.Add(item);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            listBox3.Items.Clear();
            foreach (string item in revisionMen.Values)
            {
                if (item.Contains(textBox2.Text))
                    listBox3.Items.Add(item);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

            if(listBox1.SelectedItem != null && 
               listBox2.SelectedItem != null &&
               listBox3.SelectedItem != null)
            {

                int houseId = -1, typeId = -1, manId = -1;

                foreach(int item in houses.Keys)
                {
                    if (listBox1.SelectedItem.ToString().Equals(houses[item]))
                        houseId = item;

                }
                foreach (int item in types.Keys)
                {
                    if (listBox2.SelectedItem.ToString().Equals(types[item]))
                        typeId = item;

                }
                foreach (int item in revisionMen.Keys)
                {
                    if (listBox3.SelectedItem.ToString().Equals(revisionMen[item]))
                        manId = item;

                }

                DatabaseResponse response = DatabaseUtils.AddRevision(houseId,
                                                                      typeId, 
                                                                      manId,     
                                                                      dateTimePicker1.Value,
                                                                      richTextBox1.Text);

                if(response == DatabaseResponse.CREATED)
                {
                    MessageBox.Show("Úspěšně jsi zapsal revizi.");
                }

            }

        }

        private void showData()
        {
            textBox1.Text = textBox2.Text = textBox6.Text = "";
            textBox10.Text = selectedData["CreateAuthor"].ToString();
            textBox7.Text = ((DateTime)selectedData["CreateDate"]).ToString("yyyy-MM-dd HH:mm:ss");
            textBox8.Text = selectedData["LastEditAuthor"].ToString();
            textBox9.Text = ((DateTime)selectedData["LastEditDate"]).ToString("yyyy-MM-dd HH:mm:ss");
            listBox1.SelectedItem = houses[(int)selectedData["House_ID"]];
            listBox2.SelectedItem = types[(int)selectedData["RevisionType_ID"]];
            listBox3.SelectedItem = revisionMen[(int)selectedData["RevisionMan_ID"]];
            dateTimePicker1.Value = (DateTime)selectedData["LastDate"];
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

            int houseId = -1, typeId = -1, manId = -1;

            if(listBox1.SelectedItem != null)
            {
                foreach (int item in houses.Keys)
                {
                    if (listBox1.SelectedItem.ToString().Equals(houses[item]))
                        houseId = item;

                }
            }
            if(listBox2.SelectedItem != null)
            {
                foreach (int item in types.Keys)
                {
                    if (listBox2.SelectedItem.ToString().Equals(types[item]))
                        typeId = item;

                }
            }
            if(listBox3.SelectedItem != null)
            {
                foreach (int item in revisionMen.Keys)
                {
                    if (listBox3.SelectedItem.ToString().Equals(revisionMen[item]))
                        manId = item;

                }
            }

            Dictionary<int, Dictionary<string, object>> response = DatabaseUtils.GetRevision(listBox1.SelectedItem == null ? -1 : houseId,
                                                                                             listBox2.SelectedItem == null ? -1 : typeId,
                                                                                             listBox3.SelectedItem == null ? -1 : manId,
                                                                                             dateTimePicker1.Value, checkBox1.Checked);

            if (response.Count == 1)
            {
                selectedId = response.Keys.ElementAt(0);
                selectedData = response[selectedId];
                showData();
            }
            else if (response.Count == 0)
            {
                MessageBox.Show("Je nám líto, ale žádná revize s těmito kritérii neexistuje.\n\nTIP: Zkuste si zkontrolovat správnost kritérií.");
            }
            else if (response.Count <= 10)
            {
                string data = "";
                foreach (Dictionary<string, object> item in response.Values)
                {
                    data += $"\n{houses[(int)item["House_ID"]]}, {types[(int)item["RevisionType_ID"]]}, {((DateTime)item["LastDate"]).ToString("D")}";
                }
                MessageBox.Show("Podle vyhledaných kritériích existuje " + response.Count + " počet domů. Prosím specifikujte kritéria, pro přesnější vyhledávání! Nalezlé domy: \n\n" + data);
            }
            else
            {
                MessageBox.Show("Podle vyhledaných kritériích existuje " + response.Count + " počet domů. Prosím specifikujte kritéria, pro přesnější vyhledávání!");
            }

        }

        private void clearData()
        {
            listBox1.SelectedItem = listBox2.SelectedItem = listBox3.SelectedItem = null;
            dateTimePicker1.Value = DateTime.Now;
            richTextBox1.Text = "";
            selectedId = -1;
            selectedData = new Dictionary<string, object>();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

            if(selectedId != -1)
            {

                if (MessageBox.Show("Opravdu si přejete odstranit tuto revizi?", "Odstranění dat", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DatabaseResponse response = DatabaseUtils.RemoveRevision(selectedId);
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

            int houseId = -1, typeId = -1, manId = -1;

            if (listBox1.SelectedItem != null)
            {
                foreach (int item in houses.Keys)
                {
                    if (listBox1.SelectedItem.ToString().Equals(houses[item]))
                        houseId = item;

                }
            }
            if (listBox2.SelectedItem != null)
            {
                foreach (int item in types.Keys)
                {
                    if (listBox2.SelectedItem.ToString().Equals(types[item]))
                        typeId = item;

                }
            }
            if (listBox3.SelectedItem != null)
            {
                foreach (int item in revisionMen.Keys)
                {
                    if (listBox3.SelectedItem.ToString().Equals(revisionMen[item]))
                        manId = item;

                }
            }

            DatabaseResponse databaseResponse = DatabaseUtils.EditRevision(selectedId, houseId, typeId, manId, dateTimePicker1.Value, checkBox1.Checked, richTextBox1.Text);

            if (databaseResponse == DatabaseResponse.EDITED)
            {
                MessageBox.Show("Dům úspěšně upraven.");
            }

        }

        public RevisionForm()
        {
            InitializeComponent();
        }

        private void RevisionForm_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = ProgramUtils.Images["add"];
            pictureBox3.Image = ProgramUtils.Images["remove"];
            pictureBox4.Image = ProgramUtils.Images["edit"];
            pictureBox2.Image = ProgramUtils.Images["search"];

            houses = DatabaseUtils.GetHouses();
            types = DatabaseUtils.GetRevisionTypes();
            revisionMen = DatabaseUtils.GetRevisionMen();

            foreach(string item in houses.Values)
            {
                listBox1.Items.Add(item);
            }

            foreach (string item in types.Values)
            {
                listBox2.Items.Add(item);
            }

            foreach (string item in revisionMen.Values)
            {
                listBox3.Items.Add(item);
            }

        }
    }
}
