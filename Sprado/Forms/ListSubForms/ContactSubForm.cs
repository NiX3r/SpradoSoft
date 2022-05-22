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

namespace Sprado.Forms.ListSubForms
{
    public partial class ContactSubForm : Form
    {



        public ContactSubForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            dataGridView1.Rows.Clear();

            Dictionary<int, List<object>> response = DatabaseUtils.GetContactList(textBox7.Text);

            foreach(List<object> contact in response.Values)
            {

                object[] vs = new object[8];

                vs[0] = contact[0];
                vs[1] = contact[1];
                vs[2] = contact[2];
                vs[3] = contact[3] + " " + contact[4];
                vs[4] = ((bool)contact[5] ? "ANO" : "NE");
                vs[5] = contact[6];
                vs[6] = contact[7];
                vs[7] = contact[8];

                dataGridView1.Rows.Add(vs);

            }

        }
    }
}
