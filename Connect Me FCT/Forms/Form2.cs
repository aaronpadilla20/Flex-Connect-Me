using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Threading;
using System.IO.Ports;

namespace Connect_Me_FCT
{
    public partial class Form2 : Form
    {
        System.Media.SoundPlayer EngranajeSound;
        System.Media.SoundPlayer PaginaSound;
        System.Media.SoundPlayer NeonSound;

        Form4 passwordWindow = new Form4();

        protected string log;
        protected int slots;
        public static string portCom;
        public static string baudeRate;
        private string flexFlow;
        private string[] portComs;

        DialogResult res;

        public static string ruta_notAvailable = "..\\Images\\Not Available.png";
        public Form2()
        {
            string ruta_engrane = "..\\Sounds\\Engrane.WAV";
            EngranajeSound = new System.Media.SoundPlayer(ruta_engrane);
            string ruta_pagina = "..\\Sounds\\Pagina.WAV";
            PaginaSound = new System.Media.SoundPlayer(ruta_pagina);
            string ruta_neon = "..\\Sounds\\halogen-light.WAV";
            NeonSound = new System.Media.SoundPlayer(ruta_neon);
            InitializeComponent();
         }

        private void cargaConfiguracion()
        {
            Utilities.loadDocument("desencripta");
            Utilities reader = new Utilities();
            var data = reader.LeeXML("..\\Settings.xml");
            this.flexFlow = data.Item1;
            portCom = data.Item2;
            baudeRate = data.Item3;
            Utilities.loadDocument("encripta");
            flexFlowLabel.Text = "Flex Flow: " + this.flexFlow;
            if (this.flexFlow.Contains("ON"))
            {
                flexFlowLabel.BackColor = Color.Green;
                flexFlowLabel.ForeColor = Color.White;
            }
            else
            {
                flexFlowLabel.BackColor = Color.Red;
                flexFlowLabel.ForeColor = Color.White;
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (!File.Exists("..\\Settings.xml"))
            {
                portComs = Utilities.ListaportCOM();
                Utilities.CreaXML("ON", portComs[0], "9600", "..\\Log_File\\Pass", "..\\Log_File\\Fail", "default");
                Utilities.loadDocument("encripta");
            }
            cargaConfiguracion();
            res = MessageBox.Show("El puerto serial configurado actualmente es: " + portCom + " y el baude rate configurado es: " + baudeRate + " ¿Desea realizar modificaciones?", "Configuracion inicial", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                Passwordwindow("..\\Images\\settings_password_Window.png", "..\\Images\\adelante_png.png", true);
                cargaConfiguracion();
            }
            this.Text = Form1.PartNumber;
            switch(Form1.Cantidad)
            {
                case "1":
                    slots = 1;
                    panelSlot2.BackColor = Color.Gray;
                    macBox2.Enabled = false;
                    pnBox2.Enabled = false;
                    passwordBox2.Enabled = false;
                    statusTest2.Visible = false;
                    console2.Enabled = false;
                    groupBox2.Text = "Slot Disabled";

                    panelSlot3.BackColor = Color.Gray;
                    macBox3.Enabled = false;
                    pnBox3.Enabled = false;
                    passwordBox3.Enabled = false;
                    statusTest3.Visible = false;
                    console3.Enabled = false;
                    groupBox3.Text = "Slot Disabled";

                    panelSlot4.BackColor = Color.Gray;
                    macBox4.Enabled = false;
                    pnBox4.Enabled = false;
                    passwordBox4.Enabled = false;
                    statusTest4.Visible = false;
                    console4.Enabled = false;
                    groupBox5.Text = "Slot Disabled ";
                    break;

                case "2":
                    slots = 2;
                    panelSlot3.BackColor = Color.Gray;
                    macBox3.Enabled = false;
                    pnBox3.Enabled = false;
                    passwordBox3.Enabled = false;
                    statusTest3.Visible = false;
                    console3.Enabled = false;
                    groupBox3.Text = "Slot Disabled";

                    panelSlot4.BackColor = Color.Gray;
                    macBox4.Enabled = false;
                    pnBox4.Enabled = false;
                    passwordBox4.Enabled = false;
                    statusTest4.Visible = false;
                    console4.Enabled = false;
                    groupBox5.Text = "Slot Disabled ";
                    break;

                case "3":
                    slots = 3;
                    panelSlot4.BackColor = Color.Gray;
                    macBox4.Enabled = false;
                    pnBox4.Enabled = false;
                    passwordBox4.Enabled = false;
                    statusTest4.Visible = false;
                    console4.Enabled = false;
                    groupBox5.Text = "Slot Disabled ";
                    break;
                case "default":
                    slots = 4;
                    break;
            }
            panel2.BackColor = ColorTranslator.FromHtml("#4f9300"); 
        }

        private void Settings_MouseHover(object sender, EventArgs e)
        {
            EngranajeSound.PlayLooping();
            string ruta = "..\\Images\\Settings_Animated.GIF";
            Settings.Image = System.Drawing.Image.FromFile(ruta);
        }

        private void Settings_MouseLeave(object sender, EventArgs e)
        {
            EngranajeSound.Stop();
            string ruta = "..\\Images\\Settings.PNG";
            Settings.Image = System.Drawing.Image.FromFile(ruta);
        }

        private void Logs_MouseHover(object sender, EventArgs e)
        {
            PaginaSound.PlayLooping();
            string ruta = "..\\Images\\Book_Animated.GIF";
            Logs.Image = System.Drawing.Image.FromFile(ruta);
        }

        private void Logs_MouseLeave(object sender, EventArgs e)
        {
            PaginaSound.Stop();
            string ruta = "..\\Images\\Book.PNG";
            Logs.Image = System.Drawing.Image.FromFile(ruta);
        }

        private void DrawGroupBox(GroupBox box, Graphics g, Color textcolor, Color bordercolor)
        {
            if (box!= null)
            {
                Brush textbrush = new SolidBrush(textcolor);
                Brush borderbrush = new SolidBrush(bordercolor);
                Pen borderPen = new Pen(borderbrush,10);
                SizeF strSize = g.MeasureString(box.Text, box.Font);
                Rectangle rect = new Rectangle(box.ClientRectangle.X,
                                               box.ClientRectangle.Y + (int)(strSize.Height / 2),
                                               box.ClientRectangle.Width - 1,
                                               box.ClientRectangle.Height - (int)(strSize.Height / 2) - 1);
                g.Clear(this.BackColor);
                g.DrawString(box.Text, box.Font, textbrush, box.Padding.Left, 0);

                g.DrawLine(borderPen, rect.Location, new Point(rect.X, rect.Y + rect.Height));
                g.DrawLine(borderPen, new Point(rect.X + rect.Width, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                g.DrawLine(borderPen, new Point(rect.X, rect.Y + rect.Height), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                g.DrawLine(borderPen, new Point(rect.X, rect.Y), new Point(rect.X + box.Padding.Left, rect.Y));
                g.DrawLine(borderPen, new Point(rect.X + box.Padding.Left + (int)(strSize.Width), rect.Y), new Point(rect.X + rect.Width, rect.Y));
            }
        }

        public void Settings_Click(object sender, EventArgs e)
        {
            Passwordwindow("..\\Images\\settings_password_Window.png","..\\Images\\adelante_png.png",false);
        }

        private void Passwordwindow (string image, string buton, bool topMost)
        {
            Form4.Carga(image, buton,topMost);
            if (passwordWindow.IsDisposed==true)
            {
                Form4 passwordWindow = new Form4();
                passwordWindow.ShowDialog();
                cargaConfiguracion();
            }
            else
            {
                passwordWindow.ShowDialog();
                cargaConfiguracion();
            }
        }


        private void groupBox3_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            DrawGroupBox(box, e.Graphics, Color.White, Color.FromArgb(37, 150, 190));
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            DrawGroupBox(box, e.Graphics, Color.White, Color.FromArgb(37, 150, 190));
        }

        private void groupBox2_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            DrawGroupBox(box, e.Graphics, Color.White, Color.FromArgb(37, 150, 190));
        }

        private void groupBox5_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            DrawGroupBox(box, e.Graphics, Color.White, Color.FromArgb(37, 150, 190));
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Passwordwindow("..\\Images\\exit.png", "..\\Images\\exitButton.png",true);
            if (Form4.accion == "close")
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            startButton.Enabled = false;
            Thread secuencia = new Thread(new ThreadStart(iniciaPrueba));
            secuencia.Start();
        }

        private void iniciaPrueba()
        {
            switch (Form1.PartNumber)
            {
                case "DIT-DC-ME-Y402-S-UPW(DIT-55001129-54)":
                    TestUPW();
                    break;
                case "DIT-DC-ME4-01T-S-UPW(DIT-55001129-03)":
                    TestUPW();
                    break;
                default:
                    TestGeneral();
                    break;
            }
           // TestUPW();
        }

        private void Logs_Click(object sender, EventArgs e)
        {
            Form5 pathWin = new Form5();
            pathWin.Show();
        }
    }
}

