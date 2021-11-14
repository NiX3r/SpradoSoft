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
    public partial class HouseForm : Form
    {
        public HouseForm()
        {
            InitializeComponent();
        }

        private void HouseForm_Load(object sender, EventArgs e)
        {

            pictureBox1.Image = ProgramUtils.RecolorImage((Bitmap)pictureBox1.Image, "main");
            pictureBox2.Image = ProgramUtils.RecolorImage((Bitmap)pictureBox2.Image, "main");
            pictureBox3.Image = ProgramUtils.RecolorImage((Bitmap)pictureBox3.Image, "main");
            pictureBox4.Image = ProgramUtils.RecolorImage((Bitmap)pictureBox4.Image, "main");

        }
    }
}
