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

        private void showData()
        {
            textBox1.Text = selectedData["Name"].ToString();
            textBox6.Text = selectedData["YearLoop"].ToString();
            textBox7.Text = ((DateTime)selectedData["CreateDate"]).ToString("yyyy-MM-dd HH:mm:ss");
            textBox10.Text = selectedData["CreateAuthor"].ToString();
            textBox9.Text = ((DateTime)selectedData["LastEditDate"]).ToString("yyyy-MM-dd HH:mm:ss");
            textBox8.Text = selectedData["LastEditAuthor"].ToString();
            richTextBox1.Text = selectedData["Description"].ToString();
        }

        private void clearData()
        {
            textBox1.Text = textBox10.Text = textBox6.Text = textBox7.Text = textBox8.Text = textBox9.Text = richTextBox1.Text = "";
            selectedId = -1;
            selectedData = new Dictionary<string, object>();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

            if(textBox1.Text.Length > 0 || textBox6.Text.Length > 0)
            {

                Dictionary<int, Dictionary<string, object>> response = DatabaseUtils.GetRevisionType(textBox1.Text,
                                                                                                     textBox6.Text == "" ? -1 : Convert.ToInt32(textBox6.Text));

                if (response.Count == 1)
                {
                    selectedId = response.Keys.ElementAt(0);
                    selectedData = response[selectedId];
                    showData();
                }
                else if (response.Count == 0)
                {
                    MessageBox.Show("Je nám líto, ale žádný typ s těmito kritérii neexistuje.\n\nTIP: Zkuste si zkontrolovat správnost kritérií.");
                }
                else if (response.Count <= 10)
                {
                    string data = "";
                    foreach (Dictionary<string, object> item in response.Values)
                    {
                        data += $"\n{item["Name"]}, jednou za {item["YearLoop"]} rok/y";
                    }
                    MessageBox.Show("Podle vyhledaných kritériích existuje " + response.Count + " počet typů. Prosím specifikujte kritéria, pro přesnější vyhledávání! Nalezlé domy: \n\n" + data);
                }
                else
                {
                    MessageBox.Show("Podle vyhledaných kritériích existuje " + response.Count + " počet typů. Prosím specifikujte kritéria, pro přesnější vyhledávání!");
                }

            }

        }
    }
}
