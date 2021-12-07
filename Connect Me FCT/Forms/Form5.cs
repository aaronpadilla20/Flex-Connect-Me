using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Connect_Me_FCT
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            openFolder("pass");
        }

        private void openFolder(string status)
        {
            Utilities.loadDocument("desencripta");
            Utilities reader = new Utilities();
            var data = reader.LeeXML("..\\Settings.xml");
            Utilities.loadDocument("encripta");
            if (status == "pass")
            {
                System.Diagnostics.Process.Start(data.Item4);
            }
            else
            {
                System.Diagnostics.Process.Start(data.Item5);
            }
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            openFolder("fail");
        }
    }
}
