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
    public partial class RevisionForm : Form
    {


        private int selectedId = -1;
        private Dictionary<string, object> selectedData = new Dictionary<string, object>();
        private Dictionary<string, int> houses = new Dictionary<string, int>(), types = new Dictionary<string, int>(), revisionMan = new Dictionary<string, int>();

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
            revisionMan = DatabaseUtils.GetRevisionMen();

            foreach(string item in houses.Keys)
            {
                listBox1.Items.Add(item);
            }

            foreach (string item in types.Keys)
            {
                listBox2.Items.Add(item);
            }

            foreach (string item in revisionMan.Keys)
            {
                listBox3.Items.Add(item);
            }

        }
    }
}
