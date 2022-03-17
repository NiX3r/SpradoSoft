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
    public partial class HomeForm : Form
    {

        private Dictionary<int, string> houses;

        public HomeForm()
        {
            houses = new Dictionary<int, string>();
            InitializeComponent();
        }

        private void HomeForm_Load(object sender, EventArgs e)
        {

            houses = DatabaseUtils.GetHouses();

            foreach(string house in houses.Values)
            {
                listBox1.Items.Add(house);
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            foreach(int key in houses.Keys)
            {
                if (houses[key].Equals(listBox1.SelectedItem.ToString()))
                {
                    ProgramUtils.MainUI.openForm(ProgramUtils.SubForms["Domy"]);
                    ProgramUtils.MainUI.SelectHouseButton();
                    ((HouseForm)ProgramUtils.SubForms["Domy"]).loadHouseById(key);
                }
            }

        }
    }
}
