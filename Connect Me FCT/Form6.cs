using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Connect_Me_FCT
{
    public partial class Form6 : Form
    {
        protected string texto;
        protected string imagePath;
        public static bool winClosed;
        public Form6()
        {
            InitializeComponent();
            this.Size = new Size(width: 421, height: 313);
            button1.Visible = true;
        }

        public void textLabel (string instruccion, string imagePath,string textoWin)
        {
            this.Text = textoWin;
            this.imagePath = imagePath;
            this.texto = instruccion;
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            winClosed = false;
            imagen.Image = System.Drawing.Image.FromFile(this.imagePath);
            label.Text = texto;
            if (this.texto != "Coloque el switch de power en la posicion indicada" && !this.texto.Contains("Coloque") )
            {
                this.Size = new Size(Width = 421, Height = 254);
                button1.Visible = false;
            }
            else if (this.texto == "Coloque el switch de power en la posicion indicada")
            {
                label.Font = new Font(label.Font.Name, 9);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form6_FormClosed(object sender, FormClosedEventArgs e)
        {
            winClosed = true;
        }
    }
}
