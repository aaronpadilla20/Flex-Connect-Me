using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Connect_Me_FCT
{
    public partial class Form4 : Form
    {
        private static string image;
        private static bool topMostWin;
        private static string button;
        private static string password;
        public static string accion;
        private string[] portComs;
        public Form4()
        {
            
            InitializeComponent();
        }

        public static void Carga(string imagePath, string buttonPath, bool topMost)
        {
            topMostWin = topMost;
            image = imagePath;
            button = buttonPath;
            if (File.Exists("..\\Settings.xml"))
            {
                Utilities.loadDocument("desencripta");
                Utilities reader = new Utilities();
                var datos = reader.LeeXML("..\\Settings.xml");
                Utilities.loadDocument("encripta");
                password = datos.Item6;
            }
            else
            {
                password = "default";
            }
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            imageBox.Image = System.Drawing.Image.FromFile(image);
            botonBox.Image = System.Drawing.Image.FromFile(button);
            this.TopMost = topMostWin;
        }

        private void botonBox_Click(object sender, EventArgs e)
        {
            if (image == "..\\Images\\exit.png" && textBox1.Text == password)
            {
                accion = "close";
                Environment.Exit(0);
            }
            else if(image == "..\\Images\\settings_password_Window.png" && textBox1.Text == password)
            {
                Form3 settingWindow = new Form3();
                if (File.Exists("..\\Settings.xml"))
                {
                    settingWindow.ShowDialog();
                    textBox1.Text = "";
                    this.Close();
                }
                else
                {
                    portComs = Utilities.ListaportCOM();
                    Utilities.CreaXML("ON",portComs[0],"9600","..\\Log_File\\Pass","..\\Log_File\\Fail","default");
                    Utilities.loadDocument("encripta");
                    settingWindow.ShowDialog();
                    textBox1.Text = "";
                    this.Close();
                }
            }
            else if (textBox1.Text != password)
            {
                MessageBox.Show("The entered password is incorrect, please try again");
            }
        }
    }
}
