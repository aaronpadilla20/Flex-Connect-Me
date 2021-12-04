using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.IO;

namespace Connect_Me_FCT
{
    public partial class Form3 : Form
    {
        protected string flexFlow;
        protected string portCom;
        protected string baudeRate;
        protected string passLogFile;
        protected string failLogFile;
        protected string password;
        protected string desirePath;
        protected string newPath;
        protected string[] portsCom;
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            this.portsCom = Utilities.ListaportCOM();
            Utilities.loadDocument("desencripta");
            Utilities reader = new Utilities();
            var datos = reader.LeeXML("..\\Settings.xml");
            this.flexFlow = datos.Item1;
            this.portCom = datos.Item2;
            this.baudeRate = datos.Item3;
            this.passLogFile = datos.Item4;
            this.failLogFile = datos.Item5;
            this.password = datos.Item6;

            foreach(string port in this.portsCom)
            {
                portsComBox.Items.Add(port);
            }

            portsComBox.SelectedItem = portCom;
            passwordText.UseSystemPasswordChar = true;
            passLogFileText.Text = passLogFile;
            failLogPathText.Text = failLogFile;
            passwordText.Text = password;
            

            if (this.flexFlow == "ON") 
            {
                flexFlowBox.SelectedIndex = 0;
            }
            else
            {
                flexFlowBox.SelectedIndex = 1;
            }

            if (this.baudeRate == "9600")
            {
                baudRateBox.SelectedIndex = 0;
            }
            else
            {
                baudRateBox.SelectedIndex = 1;
            }

            Utilities.loadDocument("encripta");
        }

        private void flexFlowBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (cbx != null)
            {
                e.DrawBackground();
                if (e.Index >= 0)
                {
                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;

                    Brush brush = new SolidBrush(cbx.ForeColor);
                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                        brush = SystemBrushes.HighlightText;

                    e.Graphics.DrawString(cbx.Items[e.Index].ToString(), cbx.Font, brush, e.Bounds, sf);
                }
            }
        }

        private void baudRateBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (cbx != null)
            {
                e.DrawBackground();
                if (e.Index >= 0)
                {
                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;

                    Brush brush = new SolidBrush(cbx.ForeColor);
                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                        brush = SystemBrushes.HighlightText;

                    e.Graphics.DrawString(cbx.Items[e.Index].ToString(), cbx.Font, brush, e.Bounds, sf);
                }
            }
        }

        private void searchPassPath_Click(object sender, EventArgs e)
        {
            this.newPath=Pathselect("passPath");
            passLogFileText.Text = newPath;
        }

        private void searchFailPath_Click(object sender, EventArgs e)
        {
            this.newPath=Pathselect("failPath");
            failLogPathText.Text = newPath;
        }

        private string Pathselect(string sender)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();


            if(path.ShowDialog() == DialogResult.OK)
            {
                this.desirePath = path.SelectedPath;
                return this.desirePath;
            }
            else if(sender == "passPath")
            {
                return this.passLogFile;
            }
            else
            {
                return this.failLogFile;
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            flexFlowBox.Enabled = true;
            portsComBox.Enabled = true;
            baudRateBox.Enabled = true;
            searchPassPath.Enabled = true;
            searchFailPath.Enabled = true;
            passwordText.ReadOnly = false;
            saveButton.Enabled = true;
            visionPassword.Enabled = true;
            editButton.Enabled = false;
        }

        private void visionPassword_Click(object sender, EventArgs e)
        {
            string pathVisible = "..\\Images\\ojo_visible.png";
            string pathOculto = "..\\Images\\ojo_cerrado.png";

            if (passwordText.UseSystemPasswordChar == true)
            {
                visionPassword.Image = System.Drawing.Image.FromFile(pathVisible);
                passwordText.UseSystemPasswordChar = false;
            }
            else
            {
                visionPassword.Image = System.Drawing.Image.FromFile(pathOculto);
                passwordText.UseSystemPasswordChar = true;
            }
            
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            File.Delete("..\\Settings.xml");
            Utilities.CreaXML(flexFlowBox.Text, portsComBox.Text, baudRateBox.Text, passLogFileText.Text, failLogPathText.Text, passwordText.Text);
            Utilities.loadDocument("encripta");
            MessageBox.Show("The new setting have been saved");
            this.Close();
        }

        private void portsComBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (cbx != null)
            {
                e.DrawBackground();
                if (e.Index >= 0)
                {
                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;

                    Brush brush = new SolidBrush(cbx.ForeColor);
                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                        brush = SystemBrushes.HighlightText;

                    e.Graphics.DrawString(cbx.Items[e.Index].ToString(), cbx.Font, brush, e.Bounds, sf);
                }
            }
        }
    }
}
