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
    public partial class Form1 : Form
    {
        public static string PartNumber;
        public static string Cantidad;
        public Form1()
        {
            Form1.PartNumber = "";
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CantidadBox.SelectedIndex = 0;
            ModelCombo.SelectedIndex = 0;
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            string ruta = "..\\Images\\Play_Animated.GIF";
            pictureBox1.Image = System.Drawing.Image.FromFile(ruta);
            sonidobutton();
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            string ruta = "..\\Images\\Play.JPG";
            pictureBox1.Image = System.Drawing.Image.FromFile(ruta);
        }

        private void sonidobutton()
        {
            String ruta = "..\\Sounds\\Button.WAV";
            System.Media.SoundPlayer sonido;
            sonido = new System.Media.SoundPlayer(ruta);
            sonido.Play();
        }

   
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Form2 sequence = new Form2();
            entry();
            this.Hide();
            PartNumber = ModelCombo.Text;
            Cantidad = CantidadBox.Text;
            sequence.Show();
        }

        private void entry()
        {
            String ruta = "..\\Sounds\\Button_Click.WAV";
            System.Media.SoundPlayer sonido;
            sonido = new System.Media.SoundPlayer(ruta);
            sonido.Play();
        }

        private void error()
        {
            String ruta = "..\\Sounds\\windows-exclamacion.WAV";
            System.Media.SoundPlayer sonido;
            sonido = new System.Media.SoundPlayer(ruta);
            sonido.Play();
        }

        private void CantidadBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (cbx != null)
            {
                e.DrawBackground();
                if (e.Index>= 0)
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

        private void ModelCombo_DrawItem(object sender, DrawItemEventArgs e)
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
