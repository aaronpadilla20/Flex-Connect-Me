using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Connect_Me_FCT
{
    partial class Form2
    {
        string originalMacAddress;
        int countConnected;
        string fallaConexion;
        bool notConnection;
        string programada;
        bool programmed;
        //PRIMERA PARTE -------------------------------------------------------------------------------------------------------------------------
        private async Task<bool> GetMacPartNumber(RichTextBox console, TextBox macTextBox, TextBox pnTextBox, TextBox passwordTextBox, Label connectedLabel, Label progressLabel, ProgressBar progressBar, PictureBox led, PictureBox status, string imageButton)
        {
            serialPort.DiscardInBuffer();
            ClearSlot(macTextBox, pnTextBox, passwordTextBox, connectedLabel, progressLabel, progressBar, console, led, status);
            if (InvokeRequired)
                Invoke(new Action(() => console.AppendText("Hora inicio: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt" + "\r\n"))));
                Invoke(new Action(() => console.ScrollToCaret()));
            if (InvokeRequired)
            {
                Invoke(new Action(() => status.Image = System.Drawing.Image.FromFile("..\\Images\\In progress.png")));
                Invoke(new Action(() => progressBar.Value = 10));
                Invoke(new Action(() => progressLabel.Text = "Connecting unit..."));
                Invoke(new Action(() => macTextBox.Enabled = true));
                Invoke(new Action(() => macTextBox.Focus()));
            }
            await obtenMac(macTextBox);
            if (this.flexFlow.Contains("ON"))
            {
                if (!this.flexFlowConsult.Contains("Ok"))
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => macTextBox.Enabled = false));
                    }
                    MessageBox.Show(this.flexFlowConsult, "Unidad fuera de flujo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => macTextBox.Enabled = false));
                    }
                }
            }
            this.presionaBoton = new Thread(new ThreadStart(() => instWin("Presione el boton de seleccion", imageButton, "Connect Me FCT - Presiona boton")));
            Thread.Sleep(500);
            this.presionaBoton.Start();
            while (true)
            {
                DialogResult res = MessageBox.Show("El slot esta seleccionado?", "Seleccion slot", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(res==DialogResult.Yes)
                {
                    break;
                }
                else
                {
                    MessageBox.Show("Es necesario seleccionar el slot para proceder con la prueba","Seleccion slot",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
                Thread.Sleep(1000);
            }
            Thread.Sleep(2000);
            do
            {
                serialPort.DiscardInBuffer();
                Thread.Sleep(500);
                serialPort.Write("\r\n");
                Thread.Sleep(500);
                serialResponse = serialPort.ReadExisting();
                if (serialResponse.Contains("---------------------------- Diagnostic Tests ----------------------------)") || serialResponse.Contains("Serial CLI Started") || serialResponse.Contains("IAM") || serialResponse.Contains("Press any key in 5 seconds to change these settings."))
                {
                    this.programmed = true;
                    this.programada = "La unidad ya se encuentra programada imposible probar en FCT";
                    return false;
                }
                countConnected++;
                if (countConnected >= 4)
                {
                    this.presionaBoton.Abort();
                    this.notConnection = true;
                    this.countConnected = 0;
                    this.fallaConexion = "No_connected";
                    return false;
                }
                Thread.Sleep(1000);
            } while (!serialResponse.Contains("Scan in 6 or 12 digit MAC address  followed by [Enter] :"));
            this.presionaBoton.Abort();
            this.conectado = true;
            if (conectado)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => led.Image = System.Drawing.Image.FromFile("..\\Images\\Led_on.png")));
                    Invoke(new Action(() => connectedLabel.Text = "Unit Connected"));
                    Invoke(new Action(() => progressBar.Value = 20));
                    Invoke(new Action(() => progressLabel.Text = "Reading Mac Address"));
                }
            }
            if (InvokeRequired)
            {
                Invoke(new Action(() => console.AppendText("Scan in 6 or 12 digit MAC address  followed by[Enter] :")));
                Invoke(new Action(() => console.AppendText(macAddress + "\r\n")));
                Invoke(new Action(() => console.ScrollToCaret()));
            }
            serialPort.WriteLine(macAddress);
            Thread.Sleep(1000);
            if (InvokeRequired)
            {
                Invoke(new Action(() => progressBar.Value = 30));
                Invoke(new Action(() => progressLabel.Text = "Reading Part Number"));
            }
            do
            {
                serialPort.Write("\r\n");
                serialResponse = serialPort.ReadExisting();
                Thread.Sleep(1000);
            } while (!serialResponse.Contains("Enter 10 digit part number as NNNNNNNNNN followed by [Enter]: "));
            if (InvokeRequired)
            {
                Invoke(new Action(() => console.AppendText("Enter 10 digit part number as NNNNNNNNNN followed by [Enter]:")));
                Invoke(new Action(() => console.ScrollToCaret()));
                Invoke(new Action(() => pnTextBox.Enabled = true));
                Invoke(new Action(() => pnTextBox.Focus()));
            }
            MessageBox.Show("Ingresa Part Number");
            await obtenPN(pnTextBox,macTextBox);
            if (InvokeRequired)
            {
                Invoke(new Action(() => console.AppendText(partNumber + "\r\n")));
                Invoke(new Action(() => console.ScrollToCaret()));
                Invoke(new Action(() => pnTextBox.Enabled = false));
            }
            serialPort.WriteLine(partNumber);
            Thread.Sleep(8500);
            this.countConnected = 0;
            do
            {
                serialResponse += serialPort.ReadExisting();
                if (countConnected >=2)
                {
                    this.presionaBoton.Abort();
                    this.notConnection = true;
                    this.countConnected = 0;
                    this.fallaConexion = "VPD_Not_loaded";
                    return false;
                }
                countConnected++;
                Thread.Sleep(1000);
            } while (!serialResponse.Contains("Parsing VPD"));
            if (InvokeRequired)
            {
                Invoke(new Action(() => progressBar.Value = 40));
                Invoke(new Action(() => progressLabel.Text = "Self Test"));
                Invoke(new Action(() => console.AppendText("Starting Load of VPD" + "\r\n")));
                Invoke(new Action(() => console.AppendText("Parsing VPD" + "\r\n")));
                Invoke(new Action(() => console.ScrollToCaret()));
            }
            return true;
        }

        private void ClearSlot(TextBox macTextBox, TextBox pnTextBox, TextBox passwordTextBox, Label connectedLabel, Label progressLabel, ProgressBar progressBar, RichTextBox console, PictureBox led, PictureBox status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => macTextBox.Text = ""));
                Invoke(new Action(() => macTextBox.Enabled = false));
                Invoke(new Action(() => pnTextBox.Text = ""));
                Invoke(new Action(() => pnTextBox.Enabled = false));
                Invoke(new Action(() => passwordTextBox.Text = ""));
                Invoke(new Action(() => passwordTextBox.Enabled = false));
                Invoke(new Action(() => connectedLabel.Text = "Unit disconnected"));
                Invoke(new Action(() => progressLabel.Text = "Waiting..."));
                Invoke(new Action(() => progressBar.Value = 0));
                Invoke(new Action(() => console.Text = ""));
                Invoke(new Action(() => led.Image = System.Drawing.Image.FromFile("..\\Images\\Led_off.png")));
                Invoke(new Action(() => status.Image = Properties.Resources.Waiting));
            }
        }

        private async Task obtenPN(TextBox pnTextbox,TextBox macTextBox)
        {
            await Task.Run(() => cajaMensajePN(pnTextbox,macTextBox));
            this.partNumber = pnTextbox.Text;
        }

        private void cajaMensajePN(TextBox pnTextbox, TextBox macTextBox)
        {
            while (pnTextbox.Text.Length <= 10)
            {
                Thread.Sleep(1000);
            }

            if (pnTextbox.Text == macTextBox.Text)
            {
                MessageBox.Show("Ha ingresado la macAddress como numero de parte");
                if (InvokeRequired)
                {
                    Invoke(new Action(() => pnTextbox.Text = ""));
                }
                cajaMensajePN(pnTextbox, macTextBox);
            }
        }

        private async Task obtenMac(TextBox textbox)
        {
            
            switch (textbox.Name)
            {
                case "macBox1":
                    this.macAddress = await Task.Run(() => cajaMensajeMac(textbox));
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => textbox.Text = this.macAddress));
                    }
                    break;
                case "macBox2":
                    while(true)
                    {
                        this.macAddress = await Task.Run(() => cajaMensajeMac(textbox));
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => textbox.Text = this.macAddress));
                        }
                        if (textbox.Text == macBox1.Text)
                        {
                            MessageBox.Show("Ha ingresado una Mac Address duplicada", "Mac Address duplicada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textbox.Text = "";
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case "macBox3":
                    while (true)
                    {
                        this.macAddress = await Task.Run(() => cajaMensajeMac(textbox));
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => textbox.Text = this.macAddress));
                        }
                        if (textbox.Text == macBox1.Text || textbox.Text == macBox2.Text)
                        {
                            MessageBox.Show("Ha ingresado una Mac Address duplicada", "Mac Address duplicada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textbox.Text = "";
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case "macBox4":
                    while (true)
                    {
                        this.macAddress = await Task.Run(() => cajaMensajeMac(textbox));
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => textbox.Text = this.macAddress));
                        }
                        if (textbox.Text == macBox1.Text || textbox.Text == macBox2.Text || textbox.Text == macBox3.Text)
                        {
                            MessageBox.Show("Ha ingresado una Mac Address duplicada", "Mac Address duplicada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textbox.Text = "";
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
            }
        }

        private string cajaMensajeMac(TextBox textbox)
        {
            do
            {
                Thread.Sleep(1000);
                if (textbox.Text.Length == 12)
                {
                    this.originalMacAddress = textbox.Text;
                    Thread.Sleep(1000);
                    if (this.flexFlow.Contains("ON"))
                    {
                        Utilities ff = new Utilities();
                        Thread.Sleep(1500);
                        this.flexFlowConsult = ff.FFConsult("-VERIFY " + this.originalMacAddress);
                    }
                    Thread.Sleep(1000);
                    return textbox.Text;
                }
                Thread.Sleep(1000);
            } while (textbox.Text.Length < 6);
            if (this.flexFlow.Contains("ON"))
            {
                this.macAddress = textbox.Text;
                Utilities ff = new Utilities();
                Thread.Sleep(1500);
                this.flexFlowConsult = ff.FFConsult("-VERIFY " + this.macAddress);
            }
            return textbox.Text;
        }
        //----------------------------------------------------------------------------------------------------------------------------

        //SEGUNDA PARTE ----------------------------------------------------------------------------------------------------------------
        private async Task GetPassword(RichTextBox console, TextBox passwordTextBox, Label progressLabel, ProgressBar progressBar,string imageButton)
        {
            Thread.Sleep(500);
            if (Form1.Cantidad != "1")
            {
                switch (Form1.Cantidad)
                {
                    case "2":
                        if (!this.skip1)
                        {
                            this.presionaBoton = new Thread(new ThreadStart(() => instWin("Presione el boton de seleccion", imageButton, "Connect Me FCT - Presiona boton")));
                            this.presionaBoton.Start();
                            Thread.Sleep(100);
                        }
                        break;
                    case "3":
                        if (!this.skip1 || !this.skip2)
                        {
                            this.presionaBoton = new Thread(new ThreadStart(() => instWin("Presione el boton de seleccion", imageButton, "Connect Me FCT - Presiona boton")));
                            this.presionaBoton.Start();
                            Thread.Sleep(100);
                        }
                        break;
                    case "4":
                        if (!this.skip1 || !this.skip2 || !this.skip3)
                        {
                            this.presionaBoton = new Thread(new ThreadStart(() => instWin("Presione el boton de seleccion", imageButton, "Connect Me FCT - Presiona boton")));
                            this.presionaBoton.Start();
                            Thread.Sleep(100);
                        }
                        break;
                }
            }
            if (InvokeRequired)
            {
                Invoke(new Action(() => progressBar.Value = 50));
                Invoke(new Action(() => progressLabel.Text = "Obtaining Password"));
            }
            do
            {
                serialPort.Write("\r\n");
                serialResponse = serialPort.ReadExisting();
                Thread.Sleep(1000);
            } while (!serialResponse.Contains("Scan in 12 character unique password followed by [Enter] :"));
            this.presionaBoton.Abort();
            this.conectado = true;
            MessageBox.Show("Ingrese el numero de password");
            if (InvokeRequired)
            {
                Invoke(new Action(() => console.AppendText("Scan in 12 character unique password followed by [Enter] :")));
                Invoke(new Action(() => console.ScrollToCaret()));
                Invoke(new Action(() => passwordTextBox.Enabled = true));
                Invoke(new Action(() => passwordTextBox.Focus()));
            }
            await obtenPassword(passwordTextBox);
            if (InvokeRequired)
                Invoke(new Action(() => console.AppendText(password)));
                Invoke(new Action(() => console.ScrollToCaret()));
            foreach (char c in this.password.ToCharArray())
            {
                serialPort.Write(c.ToString());
                Thread.Sleep(100);
            }
            serialPort.Write("\r\n");
            if (InvokeRequired)
            {
                Invoke(new Action(() => progressBar.Value = 60));
                Invoke(new Action(() => progressLabel.Text = "Self Testing..."));
            }
            if (Form1.Cantidad != "1")
            {
                do
                {
                    serialResponse = serialPort.ReadExisting();
                    if (InvokeRequired)
                        Invoke(new Action(() => console.AppendText(serialResponse)));
                        Invoke(new Action(() => console.ScrollToCaret()));
                    Thread.Sleep(1000);
                } while (!serialResponse.Contains("Manufacturing Diagnostics"));
            }
        }

        private async Task obtenPassword(TextBox textbox)
        {
            await Task.Run(() => cajaMensajePassword(textbox));
            this.password = textbox.Text;
        }

        private void cajaMensajePassword(TextBox textbox)
        {
            while (textbox.Text.Length != 12)
            {
                Thread.Sleep(1000);
            }
        }

        //Tercera parte --------------------------------------------------------------------------------------------------------------------
        private async Task BootingUnit(RichTextBox console1, RichTextBox console2, RichTextBox console3, RichTextBox console4, Label progressLabel1, Label progressLabel2, Label progressLabel3, Label progressLabel4, ProgressBar progresBar1, ProgressBar progressBar2, ProgressBar progressBar3, ProgressBar progressBar4)
        {
            Thread.Sleep(2000);
            await Task.Run(() => finalizando(console1, console2, console3, console4,progressLabel1, progressLabel2, progressLabel3, progressLabel4, progresBar1,progressBar2,progressBar3,progressBar4));
        }

        private void finalizando(RichTextBox console1, RichTextBox console2, RichTextBox console3, RichTextBox console4, Label progressLabel1, Label progressLabel2, Label progressLabel3, Label progressLabel4, ProgressBar progressBar1, ProgressBar progressBar2, ProgressBar progressBar3, ProgressBar progressBar4)
        {
            this.serialPort.DiscardInBuffer();
            do
            {
                serialResponse = serialPort.ReadExisting();
                if (serialResponse.Contains("Booting..."))
                {
                    break;
                }
                Thread.Sleep(2000);
                if (InvokeRequired)
                    switch (Form1.Cantidad)
                    {
                        case "1":
                            Invoke(new Action(() => console1.AppendText(serialResponse)));
                            Invoke(new Action(() => console1.ScrollToCaret()));
                            break;
                        case "2":
                            if (this.flexFlow.Contains("ON"))
                            {
                                if (this.skip2)
                                {
                                    Invoke(new Action(() => console1.AppendText(serialResponse)));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                }
                                else
                                {
                                    Invoke(new Action(() => console2.AppendText(serialResponse)));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                }
                            }
                            else
                            {
                                Invoke(new Action(() => console2.AppendText(serialResponse)));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                            }
                            break;
                        case "3":
                            if (this.flexFlow.Contains("ON"))
                            {
                                #region ---------------------- CONSOLE 3 -------------------------
                                if (!this.skip1 && !this.skip2 && !this.skip3)
                                {
                                    Invoke(new Action(() => console3.AppendText(serialResponse)));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                }
                                else if (!this.skip1 && this.skip2 && !this.skip3)
                                {
                                    Invoke(new Action(() => console3.AppendText(serialResponse)));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                }
                                else if (this.skip1 && !this.skip2 && !this.skip3)
                                {
                                    Invoke(new Action(() => console3.AppendText(serialResponse)));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                }
                                else if (this.skip1 && this.skip2 && !this.skip3)
                                {
                                    Invoke(new Action(() => console3.AppendText(serialResponse)));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                }
                                #endregion ----------------------------------------------------------------

                                #region -------------------- CONSOLE 2 ----------------------------------------
                                else if (!this.skip1 && !this.skip2 && this.skip3)
                                {
                                    Invoke(new Action(() => console2.AppendText(serialResponse)));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                }
                                else if (this.skip1 && !this.skip2 && this.skip3)
                                {
                                    Invoke(new Action(() => console2.AppendText(serialResponse)));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                }
                                #endregion --------------------------------------------------------------------------

                                #region ------------------- CONSOLE 1 -----------------------------------------------
                                else if (!this.skip1 && this.skip2 && this.skip3)
                                {
                                    Invoke(new Action(() => console1.AppendText(serialResponse)));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                }
                                #endregion ----------------------------------------------------------------------

                            }
                            else
                            {
                                Invoke(new Action(() => console3.AppendText(serialResponse)));
                                Invoke(new Action(() => console3.ScrollToCaret()));
                            }
                            break;

                        case "4":
                            if (this.flexFlow.Contains("ON"))
                            {
                                #region --------------- Consola 4-----------------------------------
                                if (!this.skip1 && !this.skip2 && !this.skip3 && !this.skip4)
                                {
                                    Invoke(new Action(() => console4.AppendText(serialResponse)));
                                    Invoke(new Action(() => console4.ScrollToCaret()));
                                }
                                else if (!this.skip1 && !this.skip2 && this.skip3 && !this.skip4)
                                {
                                    Invoke(new Action(() => console4.AppendText(serialResponse)));
                                    Invoke(new Action(() => console4.ScrollToCaret()));
                                }
                                else if (!this.skip1 && this.skip2 && !this.skip3 && !this.skip4)
                                {
                                    Invoke(new Action(() => console4.AppendText(serialResponse)));
                                    Invoke(new Action(() => console4.ScrollToCaret()));
                                }
                                else if (this.skip1 && !this.skip2 && !this.skip3 && !this.skip4)
                                {
                                    Invoke(new Action(() => console4.AppendText(serialResponse)));
                                    Invoke(new Action(() => console4.ScrollToCaret()));
                                }
                                else if (!this.skip1 && this.skip2 && this.skip3 && !this.skip4)
                                {
                                    Invoke(new Action(() => console4.AppendText(serialResponse)));
                                    Invoke(new Action(() => console4.ScrollToCaret()));
                                }
                                else if (this.skip1 && this.skip2 && !this.skip3 && this.skip4)
                                {
                                    Invoke(new Action(() => console4.AppendText(serialResponse)));
                                    Invoke(new Action(() => console4.ScrollToCaret()));
                                }
                                else if (this.skip1 && !this.skip2 && this.skip3 && !this.skip4)
                                {
                                    Invoke(new Action(() => console4.AppendText(serialResponse)));
                                    Invoke(new Action(() => console4.ScrollToCaret()));
                                }
                                else if (this.skip1 && this.skip2 && this.skip3 && !this.skip4)
                                {
                                    Invoke(new Action(() => console4.AppendText(serialResponse)));
                                    Invoke(new Action(() => console4.ScrollToCaret()));
                                }
                                #endregion---------------------------------------------------------------------

                                #region---------------------- CONSOLA 3 --------------------------------------
                                else if (!this.skip1 && !this.skip2 && !this.skip3 && this.skip4)
                                {
                                    Invoke(new Action(() => console3.AppendText(serialResponse)));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                }
                                else if (this.skip1 && !this.skip2 && !this.skip3 && this.skip4)
                                {
                                    Invoke(new Action(() => console3.AppendText(serialResponse)));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                }
                                else if (!this.skip1 && this.skip2 && !this.skip3 && this.skip4)
                                {
                                    Invoke(new Action(() => console3.AppendText(serialResponse)));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                }
                                else if (this.skip1 && this.skip2 && !this.skip3 && this.skip4)
                                {
                                    Invoke(new Action(() => console3.AppendText(serialResponse)));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                }
                                #endregion

                                #region -----------------CONSOLA 2----------------------------------------------
                                else if (!this.skip1 && !this.skip2 && this.skip3 && this.skip4)
                                {
                                    Invoke(new Action(() => console2.AppendText(serialResponse)));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                }
                                else if (this.skip1 && !this.skip2 && this.skip3 && this.skip4)
                                {
                                    Invoke(new Action(() => console2.AppendText(serialResponse)));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                }
                                #endregion

                                #region -----------------CONSOLA 1 -------------------------------------------------
                                else if (!this.skip1 && this.skip2 && this.skip3 && this.skip4)
                                {
                                    Invoke(new Action(() => console1.AppendText(serialResponse)));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                }
                                #endregion --------------------------------------------------------------------------
                                else
                                {
                                    Invoke(new Action(() => console4.AppendText(serialResponse)));
                                    Invoke(new Action(() => console4.ScrollToCaret()));
                                }
                            }
                            else
                            {
                                Invoke(new Action(() => console4.AppendText(serialResponse)));
                                Invoke(new Action(() => console4.ScrollToCaret()));
                            }
                            break;
                    }
                Thread.Sleep(1000);
            } while (!serialResponse.Contains("Push and release the pushbutton on the Mfg jig..."));
            Thread.Sleep(1000);
            this.presionaMFGI = new Thread(new ThreadStart(() => instWin("Apague y encienda el switch MFGI", "..\\Images\\MFGI.png", "Connect Me FCT - Switch MFGI")));
            this.presionaMFGI.Start();
            Thread.Sleep(1000);
            do
            {
                if (serialResponse.Contains("Booting..."))
                {
                    break;
                }
                serialResponse = serialPort.ReadExisting();
                Thread.Sleep(1000);
            } while (!serialResponse.Contains("Booting..."));
            this.presionaMFGI.Abort();
            this.booting = true;
            if (InvokeRequired)
                switch (Form1.Cantidad)
                {
                    case "1":
                        Invoke(new Action(() => console1.AppendText("Booting...")));
                        Invoke(new Action(() => console1.ScrollToCaret()));
                        Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                        Invoke(new Action(() => progressBar1.Value = 70));
                        break;
                    case "2":
                        if (this.flexFlow.Contains("ON"))
                        {
                            if (!this.skip1 && !this.skip2)
                            {
                                Invoke(new Action(() => console1.AppendText("Booting...")));
                                Invoke(new Action(() => console1.ScrollToCaret()));
                                Invoke(new Action(() => console2.AppendText("Booting...")));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                                Invoke(new Action(() => progressBar1.Value = 70));
                                Invoke(new Action(() => progressBar2.Value = 70));
                            }
                            else if (this.skip1)
                            {
                                Invoke(new Action(() => console2.AppendText("Booting...")));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                                Invoke(new Action(() => progressBar2.Value = 70));
                            }
                            else if (this.skip2)
                            {
                                Invoke(new Action(() => console1.AppendText("Booting...")));
                                Invoke(new Action(() => console1.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                                Invoke(new Action(() => progressBar1.Value = 70));
                            }
                        }
                        else
                        {
                            Invoke(new Action(() => console1.AppendText("Booting...")));
                            Invoke(new Action(() => console1.ScrollToCaret()));
                            Invoke(new Action(() => console2.AppendText("Booting...")));
                            Invoke(new Action(() => console2.ScrollToCaret()));
                            Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                            Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                            Invoke(new Action(() => progressBar1.Value = 70));
                            Invoke(new Action(() => progressBar2.Value = 70));
                        }
                        break;
                    case "3":
                        if (this.flexFlow.Contains("ON"))
                        {
                            if (!this.skip1 && !this.skip2 && !this.skip3)
                            {
                                Invoke(new Action(() => console1.AppendText("Booting...")));
                                Invoke(new Action(() => console1.ScrollToCaret()));
                                Invoke(new Action(() => console2.AppendText("Booting...")));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                                Invoke(new Action(() => console3.AppendText("Booting...")));
                                Invoke(new Action(() => console3.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel3.Text = "Booting..."));
                                Invoke(new Action(() => progressBar1.Value = 70));
                                Invoke(new Action(() => progressBar2.Value = 70));
                                Invoke(new Action(() => progressBar3.Value = 70));
                            }
                            else if (!this.skip1 && !this.skip2 && this.skip3)
                            {
                                Invoke(new Action(() => console1.AppendText("Booting...")));
                                Invoke(new Action(() => console1.ScrollToCaret()));
                                Invoke(new Action(() => console2.AppendText("Booting...")));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                                Invoke(new Action(() => progressBar1.Value = 70));
                                Invoke(new Action(() => progressBar2.Value = 70));
                            }
                            else if (this.skip1 && !this.skip2 && !this.skip3)
                            {
                                Invoke(new Action(() => console2.AppendText("Booting...")));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                                Invoke(new Action(() => console3.AppendText("Booting...")));
                                Invoke(new Action(() => console3.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel3.Text = "Booting..."));
                                Invoke(new Action(() => progressBar2.Value = 70));
                                Invoke(new Action(() => progressBar3.Value = 70));
                            }
                            else if (!this.skip1 && this.skip2 && !this.skip3)
                            {
                                Invoke(new Action(() => console1.AppendText("Booting...")));
                                Invoke(new Action(() => console1.ScrollToCaret()));
                                Invoke(new Action(() => console3.AppendText("Booting...")));
                                Invoke(new Action(() => console3.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel3.Text = "Booting..."));
                                Invoke(new Action(() => progressBar1.Value = 70));
                                Invoke(new Action(() => progressBar3.Value = 70));
                            }
                            else if (this.skip1 && this.skip2 && !this.skip3)
                            {
                                Invoke(new Action(() => console3.AppendText("Booting...")));
                                Invoke(new Action(() => console3.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel3.Text = "Booting..."));
                                Invoke(new Action(() => progressBar3.Value = 70));
                            }
                            else if (this.skip1 && !this.skip2 && this.skip3)
                            {
                                Invoke(new Action(() => console2.AppendText("Booting...")));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                                Invoke(new Action(() => progressBar2.Value = 70));
                            }
                            else if (!this.skip1 && this.skip2 && this.skip3)
                            {
                                Invoke(new Action(() => console1.AppendText("Booting...")));
                                Invoke(new Action(() => console1.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                                Invoke(new Action(() => progressBar1.Value = 70));
                            }
                        }
                        else
                        {
                            Invoke(new Action(() => console1.AppendText("Booting...")));
                            Invoke(new Action(() => console1.ScrollToCaret()));
                            Invoke(new Action(() => console2.AppendText("Booting...")));
                            Invoke(new Action(() => console2.ScrollToCaret()));
                            Invoke(new Action(() => console3.AppendText("Booting...")));
                            Invoke(new Action(() => console3.ScrollToCaret()));
                            Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                            Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                            Invoke(new Action(() => progressLabel3.Text = "Booting..."));
                            Invoke(new Action(() => progressBar1.Value = 70));
                            Invoke(new Action(() => progressBar2.Value = 70));
                            Invoke(new Action(() => progressBar3.Value = 70));
                        }
                        break;
                    case "4":

                        if (this.flexFlow.Contains("ON"))
                        {
                            #region --------------- Consola 4-----------------------------------
                            if (!this.skip1 && !this.skip2 && !this.skip3 && !this.skip4)
                            {
                                Invoke(new Action(() => console1.AppendText("Booting...")));
                                Invoke(new Action(() => console1.ScrollToCaret()));
                                Invoke(new Action(() => console2.AppendText("Booting...")));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                                Invoke(new Action(() => console3.AppendText("Booting...")));
                                Invoke(new Action(() => console3.ScrollToCaret()));
                                Invoke(new Action(() => console4.AppendText("Booting...")));
                                Invoke(new Action(() => console4.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel3.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel4.Text = "Booting..."));
                                Invoke(new Action(() => progressBar1.Value = 70));
                                Invoke(new Action(() => progressBar2.Value = 70));
                                Invoke(new Action(() => progressBar3.Value = 70));
                                Invoke(new Action(() => progressBar4.Value = 70));
                            }
                            else if (!this.skip1 && !this.skip2 && this.skip3 && !this.skip4)
                            {
                                Invoke(new Action(() => console1.AppendText("Booting...")));
                                Invoke(new Action(() => console1.ScrollToCaret()));
                                Invoke(new Action(() => console2.AppendText("Booting...")));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                                Invoke(new Action(() => console4.AppendText("Booting...")));
                                Invoke(new Action(() => console4.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel4.Text = "Booting..."));
                                Invoke(new Action(() => progressBar1.Value = 70));
                                Invoke(new Action(() => progressBar2.Value = 70));
                                Invoke(new Action(() => progressBar4.Value = 70));
                            }
                            else if (!this.skip1 && this.skip2 && !this.skip3 && !this.skip4)
                            {
                                Invoke(new Action(() => console1.AppendText("Booting...")));
                                Invoke(new Action(() => console1.ScrollToCaret()));
                                Invoke(new Action(() => console3.AppendText("Booting...")));
                                Invoke(new Action(() => console3.ScrollToCaret()));
                                Invoke(new Action(() => console4.AppendText("Booting...")));
                                Invoke(new Action(() => console4.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel3.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel4.Text = "Booting..."));
                                Invoke(new Action(() => progressBar1.Value = 70));
                                Invoke(new Action(() => progressBar3.Value = 70));
                                Invoke(new Action(() => progressBar4.Value = 70));
                            }
                            else if (this.skip1 && !this.skip2 && !this.skip3 && !this.skip4)
                            {
                                Invoke(new Action(() => console2.AppendText("Booting...")));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                                Invoke(new Action(() => console3.AppendText("Booting...")));
                                Invoke(new Action(() => console3.ScrollToCaret()));
                                Invoke(new Action(() => console4.AppendText("Booting...")));
                                Invoke(new Action(() => console4.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel3.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel4.Text = "Booting..."));
                                Invoke(new Action(() => progressBar2.Value = 70));
                                Invoke(new Action(() => progressBar3.Value = 70));
                                Invoke(new Action(() => progressBar4.Value = 70));
                            }
                            else if (!this.skip1 && this.skip2 && this.skip3 && !this.skip4)
                            {
                                Invoke(new Action(() => console1.AppendText("Booting...")));
                                Invoke(new Action(() => console1.ScrollToCaret()));
                                Invoke(new Action(() => console4.AppendText("Booting...")));
                                Invoke(new Action(() => console4.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel4.Text = "Booting..."));
                                Invoke(new Action(() => progressBar1.Value = 70));
                                Invoke(new Action(() => progressBar4.Value = 70));
                            }
                            else if (this.skip1 && this.skip2 && !this.skip3 && this.skip4)
                            {
                                Invoke(new Action(() => console3.AppendText("Booting...")));
                                Invoke(new Action(() => console3.ScrollToCaret()));
                                Invoke(new Action(() => console4.AppendText("Booting...")));
                                Invoke(new Action(() => console4.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel3.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel4.Text = "Booting..."));
                                Invoke(new Action(() => progressBar3.Value = 70));
                                Invoke(new Action(() => progressBar4.Value = 70));
                            }
                            else if (this.skip1 && !this.skip2 && this.skip3 && !this.skip4)
                            {
                                Invoke(new Action(() => console2.AppendText("Booting...")));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                                Invoke(new Action(() => console4.AppendText("Booting...")));
                                Invoke(new Action(() => console4.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel4.Text = "Booting..."));
                                Invoke(new Action(() => progressBar2.Value = 70));
                                Invoke(new Action(() => progressBar4.Value = 70));
                            }
                            else if (this.skip1 && this.skip2 && this.skip3 && !this.skip4)
                            {
                                Invoke(new Action(() => console4.AppendText("Booting...")));
                                Invoke(new Action(() => console4.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel4.Text = "Booting..."));
                                Invoke(new Action(() => progressBar4.Value = 70));
                            }
                            #endregion---------------------------------------------------------------------

                            #region---------------------- CONSOLA 3 --------------------------------------
                            else if (!this.skip1 && !this.skip2 && !this.skip3 && this.skip4)
                            {
                                Invoke(new Action(() => console1.AppendText("Booting...")));
                                Invoke(new Action(() => console1.ScrollToCaret()));
                                Invoke(new Action(() => console2.AppendText("Booting...")));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                                Invoke(new Action(() => console3.AppendText("Booting...")));
                                Invoke(new Action(() => console3.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel3.Text = "Booting..."));
                                Invoke(new Action(() => progressBar1.Value = 70));
                                Invoke(new Action(() => progressBar2.Value = 70));
                                Invoke(new Action(() => progressBar3.Value = 70));
                            }
                            else if (this.skip1 && !this.skip2 && !this.skip3 && this.skip4)
                            {
                                Invoke(new Action(() => console2.AppendText("Booting...")));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                                Invoke(new Action(() => console3.AppendText("Booting...")));
                                Invoke(new Action(() => console3.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel3.Text = "Booting..."));
                                Invoke(new Action(() => progressBar2.Value = 70));
                                Invoke(new Action(() => progressBar3.Value = 70));
                            }
                            else if (!this.skip1 && this.skip2 && !this.skip3 && this.skip4)
                            {
                                Invoke(new Action(() => console1.AppendText("Booting...")));
                                Invoke(new Action(() => console1.ScrollToCaret()));
                                Invoke(new Action(() => console3.AppendText("Booting...")));
                                Invoke(new Action(() => console3.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel3.Text = "Booting..."));
                                Invoke(new Action(() => progressBar1.Value = 70));
                                Invoke(new Action(() => progressBar3.Value = 70));
                            }
                            else if (this.skip1 && this.skip2 && !this.skip3 && this.skip4)
                            {
                                Invoke(new Action(() => console3.AppendText("Booting...")));
                                Invoke(new Action(() => console3.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel3.Text = "Booting..."));
                                Invoke(new Action(() => progressBar3.Value = 70));
                            }
                            #endregion

                            #region -----------------CONSOLA 2----------------------------------------------
                            else if (!this.skip1 && !this.skip2 && this.skip3 && this.skip4)
                            {
                                Invoke(new Action(() => console1.AppendText("Booting...")));
                                Invoke(new Action(() => console1.ScrollToCaret()));
                                Invoke(new Action(() => console2.AppendText("Booting...")));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                                Invoke(new Action(() => progressBar1.Value = 70));
                                Invoke(new Action(() => progressBar2.Value = 70));
                            }
                            else if (this.skip1 && !this.skip2 && this.skip3 && this.skip4)
                            {
                                Invoke(new Action(() => console2.AppendText("Booting...")));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                                Invoke(new Action(() => progressBar2.Value = 70));
                            }
                            #endregion

                            #region -----------------CONSOLA 1 -------------------------------------------------
                            else if (!this.skip1 && this.skip2 && this.skip3 && this.skip4)
                            {
                                Invoke(new Action(() => console1.AppendText("Booting...")));
                                Invoke(new Action(() => console1.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                                Invoke(new Action(() => progressBar1.Value = 70));
                            }
                            #endregion --------------------------------------------------------------------------
                            else
                            {
                                Invoke(new Action(() => console1.AppendText("Booting...")));
                                Invoke(new Action(() => console1.ScrollToCaret()));
                                Invoke(new Action(() => console2.AppendText("Booting...")));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                                Invoke(new Action(() => console3.AppendText("Booting...")));
                                Invoke(new Action(() => console3.ScrollToCaret()));
                                Invoke(new Action(() => console4.AppendText("Booting...")));
                                Invoke(new Action(() => console4.ScrollToCaret()));
                                Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel3.Text = "Booting..."));
                                Invoke(new Action(() => progressLabel4.Text = "Booting..."));
                                Invoke(new Action(() => progressBar1.Value = 70));
                                Invoke(new Action(() => progressBar2.Value = 70));
                                Invoke(new Action(() => progressBar3.Value = 70));
                                Invoke(new Action(() => progressBar4.Value = 70));
                            }
                        }
                        else
                        {
                            Invoke(new Action(() => console1.AppendText("Booting...")));
                            Invoke(new Action(() => console1.ScrollToCaret()));
                            Invoke(new Action(() => console2.AppendText("Booting...")));
                            Invoke(new Action(() => console2.ScrollToCaret()));
                            Invoke(new Action(() => console3.AppendText("Booting...")));
                            Invoke(new Action(() => console3.ScrollToCaret()));
                            Invoke(new Action(() => console4.AppendText("Booting...")));
                            Invoke(new Action(() => console4.ScrollToCaret()));
                            Invoke(new Action(() => progressLabel1.Text = "Booting..."));
                            Invoke(new Action(() => progressLabel2.Text = "Booting..."));
                            Invoke(new Action(() => progressLabel3.Text = "Booting..."));
                            Invoke(new Action(() => progressLabel4.Text = "Booting..."));
                            Invoke(new Action(() => progressBar1.Value = 70));
                            Invoke(new Action(() => progressBar2.Value = 70));
                            Invoke(new Action(() => progressBar3.Value = 70));
                            Invoke(new Action(() => progressBar4.Value = 70));
                        }
                        break;
                }
            for (int count = 0; count <= 50; count++)
            {
                if (InvokeRequired)
                    switch (Form1.Cantidad)
                    {
                        case "1":
                            Invoke(new Action(() => console1.AppendText(count.ToString())));
                            Invoke(new Action(() => console1.ScrollToCaret()));
                            break;
                        case "2":
                            if (this.flexFlow.Contains("ON"))
                            {
                                if (!this.skip1 && !this.skip2)
                                {
                                    Invoke(new Action(() => console1.AppendText(count.ToString())));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                    Invoke(new Action(() => console2.AppendText(count.ToString())));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                }
                                else if (this.skip1)
                                {
                                    Invoke(new Action(() => console2.AppendText(count.ToString())));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                }
                                else if (this.skip2)
                                {
                                    Invoke(new Action(() => console1.AppendText(count.ToString())));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                }
                            }
                            else
                            {
                                Invoke(new Action(() => console1.AppendText(count.ToString())));
                                Invoke(new Action(() => console1.ScrollToCaret()));
                                Invoke(new Action(() => console2.AppendText(count.ToString())));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                            }
                            break;
                        case "3":
                            if (this.flexFlow.Contains("ON"))
                            {
                                if (!this.skip1 && !this.skip2 && !this.skip3)
                                {
                                    Invoke(new Action(() => console1.AppendText(count.ToString())));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                    Invoke(new Action(() => console2.AppendText(count.ToString())));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                    Invoke(new Action(() => console3.AppendText(count.ToString())));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                }
                                else if (!this.skip1 && !this.skip2 && this.skip3)
                                {
                                    Invoke(new Action(() => console1.AppendText(count.ToString())));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                    Invoke(new Action(() => console2.AppendText(count.ToString())));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                }
                                else if (this.skip1 && !this.skip2 && !this.skip3)
                                {
                                    Invoke(new Action(() => console2.AppendText(count.ToString())));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                    Invoke(new Action(() => console3.AppendText(count.ToString())));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                }
                                else if (!this.skip1 && this.skip2 && !this.skip3)
                                {
                                    Invoke(new Action(() => console1.AppendText(count.ToString())));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                    Invoke(new Action(() => console3.AppendText(count.ToString())));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                }
                                else if (this.skip1 && this.skip2 && !this.skip3)
                                {
                                    Invoke(new Action(() => console3.AppendText(count.ToString())));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                }
                                else if (this.skip1 && !this.skip2 && this.skip3)
                                {
                                    Invoke(new Action(() => console2.AppendText(count.ToString())));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                }
                                else if (!this.skip1 && this.skip2 && this.skip3)
                                {
                                    Invoke(new Action(() => console1.AppendText(count.ToString())));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                }
                            }
                            else
                            {
                                Invoke(new Action(() => console1.AppendText(count.ToString())));
                                Invoke(new Action(() => console1.ScrollToCaret()));
                                Invoke(new Action(() => console2.AppendText(count.ToString())));
                                Invoke(new Action(() => console2.ScrollToCaret()));
                                Invoke(new Action(() => console3.AppendText(count.ToString())));
                                Invoke(new Action(() => console3.ScrollToCaret()));
                            }
                            break;
                        case "4":
                            if (this.flexFlow.Contains("ON"))
                            {
                                #region --------------- Consola 4-----------------------------------
                                if (!this.skip1 && !this.skip2 && !this.skip3 && !this.skip4)
                                {
                                    Invoke(new Action(() => console1.AppendText(count.ToString())));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                    Invoke(new Action(() => console2.AppendText(count.ToString())));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                    Invoke(new Action(() => console3.AppendText(count.ToString())));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                    Invoke(new Action(() => console4.AppendText(count.ToString())));
                                    Invoke(new Action(() => console4.ScrollToCaret()));

                                }
                                else if (!this.skip1 && !this.skip2 && this.skip3 && !this.skip4)
                                {
                                    Invoke(new Action(() => console1.AppendText(count.ToString())));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                    Invoke(new Action(() => console2.AppendText(count.ToString())));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                    Invoke(new Action(() => console4.AppendText(count.ToString())));
                                    Invoke(new Action(() => console4.ScrollToCaret()));
                                }
                                else if (!this.skip1 && this.skip2 && !this.skip3 && !this.skip4)
                                {
                                    Invoke(new Action(() => console1.AppendText(count.ToString())));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                    Invoke(new Action(() => console3.AppendText(count.ToString())));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                    Invoke(new Action(() => console4.AppendText(count.ToString())));
                                    Invoke(new Action(() => console4.ScrollToCaret()));
                                }
                                else if (this.skip1 && !this.skip2 && !this.skip3 && !this.skip4)
                                {
                                    Invoke(new Action(() => console2.AppendText(count.ToString())));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                    Invoke(new Action(() => console3.AppendText(count.ToString())));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                    Invoke(new Action(() => console4.AppendText(count.ToString())));
                                    Invoke(new Action(() => console4.ScrollToCaret()));
                                }
                                else if (!this.skip1 && this.skip2 && this.skip3 && !this.skip4)
                                {
                                    Invoke(new Action(() => console1.AppendText(count.ToString())));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                    Invoke(new Action(() => console4.AppendText(count.ToString())));
                                    Invoke(new Action(() => console4.ScrollToCaret()));
                                }
                                else if (this.skip1 && this.skip2 && !this.skip3 && this.skip4)
                                {
                                    Invoke(new Action(() => console3.AppendText(count.ToString())));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                    Invoke(new Action(() => console4.AppendText(count.ToString())));
                                    Invoke(new Action(() => console4.ScrollToCaret()));
                                }
                                else if (this.skip1 && !this.skip2 && this.skip3 && !this.skip4)
                                {
                                    Invoke(new Action(() => console2.AppendText(count.ToString())));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                    Invoke(new Action(() => console4.AppendText(count.ToString())));
                                    Invoke(new Action(() => console4.ScrollToCaret()));
                                }
                                else if (this.skip1 && this.skip2 && this.skip3 && !this.skip4)
                                {
                                    Invoke(new Action(() => console4.AppendText(count.ToString())));
                                    Invoke(new Action(() => console4.ScrollToCaret()));
                                }
                                #endregion---------------------------------------------------------------------

                                #region---------------------- CONSOLA 3 --------------------------------------
                                else if (!this.skip1 && !this.skip2 && !this.skip3 && this.skip4)
                                {
                                    Invoke(new Action(() => console1.AppendText(count.ToString())));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                    Invoke(new Action(() => console2.AppendText(count.ToString())));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                    Invoke(new Action(() => console3.AppendText(count.ToString())));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                }
                                else if (this.skip1 && !this.skip2 && !this.skip3 && this.skip4)
                                {
                                    Invoke(new Action(() => console2.AppendText(count.ToString())));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                    Invoke(new Action(() => console3.AppendText(count.ToString())));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                }
                                else if (!this.skip1 && this.skip2 && !this.skip3 && this.skip4)
                                {
                                    Invoke(new Action(() => console1.AppendText(count.ToString())));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                    Invoke(new Action(() => console3.AppendText(count.ToString())));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                }
                                else if (this.skip1 && this.skip2 && !this.skip3 && this.skip4)
                                {
                                    Invoke(new Action(() => console3.AppendText(count.ToString())));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                }
                                #endregion

                                #region -----------------CONSOLA 2----------------------------------------------
                                else if (!this.skip1 && !this.skip2 && this.skip3 && this.skip4)
                                {
                                    Invoke(new Action(() => console1.AppendText(count.ToString())));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                    Invoke(new Action(() => console2.AppendText(count.ToString())));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                }
                                else if (this.skip1 && !this.skip2 && this.skip3 && this.skip4)
                                {
                                    Invoke(new Action(() => console2.AppendText(count.ToString())));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                }
                                #endregion

                                #region -----------------CONSOLA 1 -------------------------------------------------
                                else if (!this.skip1 && this.skip2 && this.skip3 && this.skip4)
                                {
                                    Invoke(new Action(() => console1.AppendText(count.ToString())));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                }
                                #endregion --------------------------------------------------------------------------
                                else
                                {
                                    Invoke(new Action(() => console1.AppendText(count.ToString())));
                                    Invoke(new Action(() => console1.ScrollToCaret()));
                                    Invoke(new Action(() => console2.AppendText(count.ToString())));
                                    Invoke(new Action(() => console2.ScrollToCaret()));
                                    Invoke(new Action(() => console3.AppendText(count.ToString())));
                                    Invoke(new Action(() => console3.ScrollToCaret()));
                                    Invoke(new Action(() => console4.AppendText(count.ToString())));
                                    Invoke(new Action(() => console4.ScrollToCaret()));
                                }
                            }
                            else
                            {
                                Invoke(new Action(() => console1.AppendText(count.ToString())));
                                Invoke(new Action(() => console2.AppendText(count.ToString())));
                                Invoke(new Action(() => console3.AppendText(count.ToString())));
                                Invoke(new Action(() => console4.AppendText(count.ToString())));
                            }
                            break;
                    }
                Thread.Sleep(1000);
            }
        }

        //Cuarta parte ----------------------------------------------------------------------------
        private async Task MFGOLedStatus(string mensaje, Label progressLabel, ProgressBar progressBar)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => progressBar.Value = 80));
                Invoke(new Action(() => progressLabel.Text = "Verifying MFGO led status.."));
            }
            estadoPrueba = await Task.Run(() => obtenResultadoFinal(mensaje));
        }

        private bool obtenResultadoFinal(string mensaje)
        {
            DialogResult res = MessageBox.Show(mensaje, "Estado de led", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (res == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------

        private async Task GetFFeti(bool resPrueba, string mac, Label progressLabel, ProgressBar progressBar, PictureBox status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => progressBar.Value = 90));
                Invoke(new Action(() => progressLabel.Text = "Flex Flow Data"));
            }
            await Task.Run(() => generaDato(resPrueba, mac,status));
            if (InvokeRequired)
            {
                Invoke(new Action(() => progressBar.Value = 99));
                Invoke(new Action(() => progressLabel.Text = "Test Finished"));
            }
        }

        private void generaDato(bool decision, string mac,PictureBox status)
        {
            if (decision)
            {
                if (this.flexFlow.Contains("ON"))
                {
                    Utilities passUnit = new Utilities();
                    if (mac.Length == 12)
                    {
                        this.flexFlowConsult = passUnit.FFConsult("-PASS " + mac);
                    }
                    else
                    {
                        this.flexFlowConsult = passUnit.FFConsult("-PASS " + mac);
                    }
                    if (this.flexFlowConsult.Contains("Ok"))
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => status.Image = System.Drawing.Image.FromFile("..\\Images\\PASS.jpg")));
                            Invoke(new Action(() => startButton.Enabled = true));
                        }
                        this.serialPort.Close();
                    }
                    else
                    {
                        MessageBox.Show("Se presento el siguiente problema al intentar dar el pase a la unidad: " + flexFlowConsult, "Flex Flow Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => startButton.Enabled = true));
                        }
                        this.serialPort.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Paso la unidad " + mac);
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => status.Image = System.Drawing.Image.FromFile("..\\Images\\PASS.jpg")));
                        Invoke(new Action(() => startButton.Enabled = true));
                    }
                    this.serialPort.Close();
                }

            }
            else
            {
                if (this.flexFlow.Contains("ON"))
                {
                    Utilities passUnit = new Utilities();
                    if (mac.Length == 12)
                    {
                        this.flexFlowConsult = passUnit.FFConsult("-FAIL " + mac + " -Code:" + this.codigoFalla);
                    }
                    else
                    {
                        this.flexFlowConsult = passUnit.FFConsult("-FAIL " + mac + " -Code:" + this.codigoFalla);
                    }
                    if (this.flexFlowConsult.Contains("Ok"))
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => status.Image = System.Drawing.Image.FromFile("..\\Images\\Failed.jpg")));
                            Invoke(new Action(() => startButton.Enabled = true));
                        }
                        this.serialPort.Close();
                    }
                    else
                    {
                        MessageBox.Show("Se presento el siguiente problema al intentar fallar la unidad " + flexFlowConsult, "Flex Flow Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => startButton.Enabled = true));
                        }
                        this.serialPort.Close();
                    }
                }
                else
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => status.Image = System.Drawing.Image.FromFile("..\\Images\\Failed.jpg")));
                        Invoke(new Action(() => startButton.Enabled = true));
                    }
                    this.serialPort.Close();
                }
            }
        }

        private void instWin(string mensaje, string ruta, string textoWin)
        {
            Form6 instructionWin = new Form6();
            instructionWin.textLabel(mensaje, ruta, textoWin);
            instructionWin.ShowDialog();
            if (mensaje == "Presione el boton de seleccion" || mensaje == "Apague y encienda el switch MFGI")
                while (true)
                {
                    if (this.conectado || this.booting || this.programmed || this.notConnection)
                    {
                        if (!this.notConnection)
                        {
                            if (instructionWin.IsDisposed)
                            {
                                break;
                            }
                            else
                            {
                                conectado = false;
                                instructionWin.Close();
                                continue;
                            }
                        }
                        conectado = false;
                        instructionWin.Close();
                        break;
                    }
                }
        }
    }
}
