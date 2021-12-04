using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Connect_Me_FCT
{
    partial class Form2

    {
        Thread inicio;
        Thread enciende;
        Thread presionaBoton;
        Thread presionaMFGI;

        

        SerialPort serialPort;
        protected bool conectado;
        protected bool booting;
        protected bool estadoPrueba;

        public static string fecha_hora = "";
        public static string serialResponse = "";

        private string macAddress;
        private string partNumber;
        private string password;
        private string flexFlowConsult;
        private bool valido;
        private bool skip1;
        private bool skip2;
        private bool skip3;
        private bool skip4;
        private bool abortado;

        Form6 buttonWin = new Form6();

        private async void TestUPW()
        {
            this.serialPort = new SerialPort(Form2.portCom, Int32.Parse(Form2.baudeRate));
            try
            {
                this.serialPort.Open();
            }
            catch (Exception e)
            {
                DialogResult res = MessageBox.Show("El puerto COM se encuentra ocupado por otra aplicacion, cierrale e intente de nuevo");
                if (InvokeRequired)
                    Invoke(new Action(() => startButton.Enabled = true));
                return;
            }
            switch (Form1.Cantidad)
            {
                case "1":
                    this.inicio = new Thread(new ThreadStart(() => instWin("Coloque la unidad en el nido 1", "..\\Images\\coloca_unidad.jpg", "Connect Me FCT - Coloca unidad")));
                    break;
                case "2":
                    this.inicio = new Thread(new ThreadStart(() => instWin("Coloque las unidades en los nidos 1 y 3", "..\\Images\\dos_unidades.jpg", "Connect Me FCT - Coloca unidades")));
                    break;
                case "3":
                    this.inicio = new Thread(new ThreadStart(() => instWin("Coloque las unidades en los nidos 1, 3 y 5 ", "..\\Images\\tres_unidades.jpg", "Connect Me FCT - Coloca unidades")));
                    break;
                case "4":
                    this.inicio = new Thread(new ThreadStart(() => instWin("Coloque las unidades en los 4 nidos", "..\\Images\\cuatro_unidades.jpg", "Connect Me FCT - Coloca unidades")));
                    break;
            }
            this.inicio.Start();
            while (true)
            {
                if (Form6.winClosed == true)
                {
                    break;
                }
            }
            Form6.winClosed = false;
            this.inicio.Abort();
            this.enciende = new Thread(new ThreadStart(() => instWin("Coloque el switch de power en la posicion indicada", "..\\Images\\power_button_on.jpg", "Connect Me FCT - Enciende fixtura")));
            this.enciende.Start();
            while (true)
            {
                if (Form6.winClosed == true)
                {
                    break;
                }
            }
            this.enciende.Abort();
            Form6.winClosed = false;

            this.skip1 = false;
            this.skip2 = false;
            this.skip3 = false;
            this.skip4 = false;
            switch(Form1.Cantidad)
            {
                case "1":
                    if (this.flexFlow.Contains("ON"))
                    {
                        while (!valido)
                        {
                            this.valido = await GetMacPartNumber(console1, macBox1, pnBox1, passwordBox1, statusConnect1, progressingLabel1, progressBar1, Led1, statusTest1, "..\\Images\\button_1.jpg");
                            if (!valido)
                            {
                                DialogResult res = MessageBox.Show("Desea abortar la prueba?", "Continuar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (res == DialogResult.Yes)
                                {
                                    this.abortado = true;
                                    MessageBox.Show("La prueba se ha abortado", "Prueba Abortada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    if (InvokeRequired)
                                        Invoke(new Action(() => startButton.Enabled = true));
                                    return;
                                }
                            }
                        }
                        await GetPassword(console1, passwordBox1,progressingLabel1,progressBar1,"..\\Images\\button_1.jpg");
                        await BootingUnit(console1, console2, console3, console4,progressingLabel1,progressingLabel2,progressingLabel3,progressingLabel4,progressBar1,progressBar2,progressBar3,progressBar4);
                        await MFGOLedStatus("¿Esta parpadeando el led MFGO del slot 1?",progressingLabel1,progressBar1);
                        await GetFFeti(estadoPrueba, macBox1.Text,progressingLabel1,progressBar1,statusTest1);
                    }
                    else
                    {
                        await GetMacPartNumber(console1, macBox1, pnBox1, passwordBox1, statusConnect1, progressingLabel1, progressBar1, Led1, statusTest1, "..\\Images\\button_1.jpg");
                        await GetPassword(console1, passwordBox1, progressingLabel1, progressBar1, "..\\Images\\button_1.jpg");
                        await BootingUnit(console1, console2, console3, console4, progressingLabel1, progressingLabel2, progressingLabel3, progressingLabel4, progressBar1, progressBar2, progressBar3, progressBar4);
                        await MFGOLedStatus("¿Esta parpadeando el led MFGO del slot 1?", progressingLabel1, progressBar1);
                        await GetFFeti(estadoPrueba, macBox1.Text, progressingLabel1, progressBar1,statusTest1);
                    }
                    break;
                case "2":
                    if (this.flexFlow.Contains("ON"))
                    {
                        while (!valido)
                        {
                            this.valido = await GetMacPartNumber(console1, macBox1, pnBox1, passwordBox1, statusConnect1, progressingLabel1, progressBar1, Led1, statusTest1, "..\\Images\\button_1.jpg");
                            if (!valido)
                            {
                                DialogResult res = MessageBox.Show("Desea ignorar esta imagen?", "Decision", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (res == DialogResult.Yes)
                                {
                                    this.skip1 = true;
                                    MessageBox.Show("Imagen ignorada desconecte la unidad previo a proceder con la siguiente unidad", "Imagen ignorada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    if (InvokeRequired)
                                    {
                                        Invoke(new Action(() => macBox1.Text = ""));
                                        Invoke(new Action(() => macBox1.Enabled = false));
                                        Invoke(new Action(() => console1.Text = ""));
                                    }
                                    break;
                                }
                            }
                        }
                        this.valido = false;
                        while(!valido)
                        {
                            this.valido = await GetMacPartNumber(console2, macBox2, pnBox2, passwordBox2, statusConnect2, progressingLabel2, progressBar2, led2, statusTest2, "..\\Images\\button_2.png");
                            if (!valido)
                            {
                                if (this.skip1)
                                {
                                    DialogResult res = MessageBox.Show("Desea abortar la prueba?", "Continuar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (res == DialogResult.Yes)
                                    {
                                        this.abortado = true;
                                        MessageBox.Show("La prueba se ha abortado", "Prueba Abortada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        if (InvokeRequired)
                                            Invoke(new Action(() => startButton.Enabled = true));
                                        this.serialPort.Close();
                                        return;
                                    }
                                }
                                else
                                {
                                    DialogResult res = MessageBox.Show("Desea ignorar esta imagen?", "Decision", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (res == DialogResult.Yes)
                                    {
                                        this.skip2 = true;
                                        MessageBox.Show("Imagen ignorada desconecte la unidad previo a proceder con la siguiente unidad", "Imagen ignorada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        if (InvokeRequired)
                                        {
                                            Invoke(new Action(() => macBox2.Text = ""));
                                            Invoke(new Action(() => macBox2.Enabled = false));
                                            Invoke(new Action(() => console2.Text = ""));
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        if (!this.skip1)
                            await GetPassword(console1, passwordBox1,progressingLabel1,progressBar1,"..\\Images\\button_1.jpg");
                        if (!this.skip2)
                            await GetPassword(console2, passwordBox2,progressingLabel2,progressBar2,"..\\Images\\button_2.png");
                        if (!this.skip1 || !this.skip2)
                            await BootingUnit(console1, console2, console3, console4,progressingLabel1,progressingLabel2,progressingLabel3,progressingLabel4,progressBar1,progressBar2,progressBar3,progressBar4);
                        if (!this.skip1)
                        {
                            await MFGOLedStatus("¿Esta parpadeando el led MFGO1?",progressingLabel1,progressBar1);
                            await GetFFeti(estadoPrueba, macBox1.Text,progressingLabel1,progressBar1,statusTest1);
                        }
                        if (!this.skip2)
                        {
                            await MFGOLedStatus("¿Esta parpadeando el led MFGO3?",progressingLabel2,progressBar2);
                            await GetFFeti(estadoPrueba, macBox2.Text,progressingLabel2,progressBar2,statusTest2);
                        }
                    }
                    else
                    {
                        await GetMacPartNumber(console1, macBox1, pnBox1, passwordBox1, statusConnect1, progressingLabel1, progressBar1, Led1, statusTest1, "..\\Images\\button_1.jpg");
                        await GetMacPartNumber(console2, macBox2, pnBox2, passwordBox2, statusConnect2, progressingLabel2, progressBar2, led2, statusTest2, "..\\Images\\button_2.png");
                        await GetPassword(console1, passwordBox1,progressingLabel1,progressBar1,"..\\Images\\button_1.jpg");
                        await GetPassword(console2, passwordBox2,progressingLabel2,progressBar2,"..\\Images\\button_2.png");
                        await BootingUnit(console1, console2, console3, console4,progressingLabel1,progressingLabel2,progressingLabel3,progressingLabel4,progressBar1,progressBar2,progressBar3,progressBar4);
                        await MFGOLedStatus("¿Esta parpadeando el led MFGO1?",progressingLabel1,progressBar1);
                        await GetFFeti(estadoPrueba, macBox1.Text,progressingLabel1,progressBar1,statusTest1);
                        await MFGOLedStatus("¿Esta parpadeando el led MFGO3?",progressingLabel2,progressBar2);
                        await GetFFeti(estadoPrueba, macBox2.Text,progressingLabel2,progressBar2,statusTest2);
                    }
                    break;
                case "3":
                    if (this.flexFlow.Contains("ON"))
                    {
                        while (!valido)
                        {
                            this.valido = await GetMacPartNumber(console1, macBox1, pnBox1, passwordBox1, statusConnect1, progressingLabel1, progressBar1, Led1, statusTest1, "..\\Images\\button_1.jpg");
                            if (!valido)
                            {
                                DialogResult res = MessageBox.Show("Desea ignorar esta imagen?", "Decision", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (res == DialogResult.Yes)
                                {
                                    this.skip1 = true;
                                    MessageBox.Show("Imagen ignorada desconecte la unidad previo a proceder con la siguiente unidad", "Imagen ignorada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    if (InvokeRequired)
                                    {
                                        Invoke(new Action(() => macBox1.Text = ""));
                                        Invoke(new Action(() => macBox1.Enabled = false));
                                        Invoke(new Action(() => console1.Text = ""));
                                    }
                                    break;
                                }
                            }
                        }
                        this.valido = false;
                        while (!valido)
                        {
                            this.valido = await GetMacPartNumber(console2, macBox2, pnBox2, passwordBox2, statusConnect2, progressingLabel2, progressBar2, led2, statusTest2, "..\\Images\\button_2.png");
                            if (!valido)
                            {
                                DialogResult res = MessageBox.Show("Desea ignorar esta imagen?", "Decision", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (res == DialogResult.Yes)
                                {
                                    this.skip2 = true;
                                    MessageBox.Show("Imagen ignorada desconecte la unidad previo a proceder con la siguiente unidad", "Imagen ignorada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    if (InvokeRequired)
                                    {
                                        Invoke(new Action(() => macBox2.Text = ""));
                                        Invoke(new Action(() => macBox2.Enabled = false));
                                        Invoke(new Action(() => console2.Text = ""));
                                    }
                                    break;
                                }
                            }
                        }
                        this.valido = false;
                        while (!valido)
                        {
                            this.valido = await GetMacPartNumber(console3, macBox3, pnBox3, passwordBox3, statusConnect3, progressingLabel3, progressBar3, led3, statusTest3, "..\\Images\\button_3.jpg");
                            if (!valido)
                            {
                                if (this.skip1 && this.skip2)
                                {
                                    DialogResult res = MessageBox.Show("Desea abortar la prueba?", "Continuar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (res == DialogResult.Yes)
                                    {
                                        this.abortado = true;
                                        MessageBox.Show("La prueba se ha abortado", "Prueba Abortada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        if (InvokeRequired)
                                            Invoke(new Action(() => startButton.Enabled = true));
                                        this.serialPort.Close();
                                        return;
                                    }
                                }
                                else
                                {
                                    DialogResult res = MessageBox.Show("Desea ignorar esta imagen?", "Decision", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (res == DialogResult.Yes)
                                    {
                                        this.skip3 = true;
                                        MessageBox.Show("Imagen ignorada desconecte la unidad previo a proceder con la siguiente unidad", "Imagen ignorada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        if (InvokeRequired)
                                        {
                                            Invoke(new Action(() => macBox3.Text = ""));
                                            Invoke(new Action(() => macBox3.Enabled = false));
                                            Invoke(new Action(() => console3.Text = ""));
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        if (!this.skip1)
                        {
                            await GetPassword(console1, passwordBox1,progressingLabel1,progressBar1,"..\\Images\\button_1.jpg");
                        }
                        if (!this.skip2)
                        {
                            await GetPassword(console2, passwordBox2,progressingLabel2,progressBar2,"..\\Images\\button_2.png");
                        }
                            
                        if (!this.skip3)
                        {
                            await GetPassword(console3, passwordBox3,progressingLabel3,progressBar3,"..\\Images\\button_3.jpg");
                        }
                        if (!this.skip1 || !this.skip2 || !this.skip3)
                            await BootingUnit(console1, console2, console3, console4,progressingLabel1,progressingLabel2,progressingLabel3,progressingLabel4,progressBar1,progressBar2,progressBar3,progressBar4);
                        if (!this.skip1)
                        {
                            await MFGOLedStatus("¿Esta parpadeando el led MFGO1?",progressingLabel1,progressBar1);
                            await GetFFeti(estadoPrueba, macBox1.Text,progressingLabel1,progressBar1,statusTest1);
                        }
                        if (!this.skip2)
                        {
                            await MFGOLedStatus("¿Esta parpadeando el led MFGO3?",progressingLabel2,progressBar2);
                            await GetFFeti(estadoPrueba, macBox2.Text,progressingLabel2,progressBar2,statusTest2);
                        }
                        if (!this.skip3)
                        {
                            await MFGOLedStatus("¿Esta parpadeando el led MFGO5?",progressingLabel3,progressBar3);
                            await GetFFeti(estadoPrueba, macBox3.Text,progressingLabel3,progressBar3,statusTest3);
                        }
                    }
                    else
                    {
                        await GetMacPartNumber(console1, macBox1, pnBox1, passwordBox1, statusConnect1, progressingLabel1, progressBar1, Led1, statusTest1, "..\\Images\\button_1.jpg");
                        await GetMacPartNumber(console2, macBox2, pnBox2, passwordBox2, statusConnect2, progressingLabel2, progressBar2, led2, statusTest2, "..\\Images\\button_2.png");
                        await GetMacPartNumber(console3, macBox3, pnBox3, passwordBox3, statusConnect3, progressingLabel3, progressBar3, led3, statusTest3, "..\\Images\\button_3.jpg");
                        await GetPassword(console1, passwordBox1,progressingLabel1,progressBar1,"..\\Images\\button_1.jpg");
                        await GetPassword(console2, passwordBox2,progressingLabel2,progressBar2,"..\\Images\\button_2.png");
                        await GetPassword(console3, passwordBox3,progressingLabel3,progressBar3,"..\\Images\\button_3.jpg");
                        await BootingUnit(console1, console2, console3, console4,progressingLabel1,progressingLabel2,progressingLabel3,progressingLabel4,progressBar1,progressBar2,progressBar3,progressBar4);
                        await MFGOLedStatus("¿Esta parpadeando el led MFGO1?",progressingLabel1,progressBar1);
                        await GetFFeti(estadoPrueba, macBox1.Text,progressingLabel1,progressBar1,statusTest1);
                        await MFGOLedStatus("¿Esta parpadeando el led MFGO3?",progressingLabel2,progressBar2);
                        await GetFFeti(estadoPrueba, macBox2.Text,progressingLabel2,progressBar2,statusTest2);
                        await MFGOLedStatus("¿Esta parpadeando el led MFGO5?",progressingLabel3,progressBar3);
                        await GetFFeti(estadoPrueba, macBox3.Text, progressingLabel3, progressBar3, statusTest3);
                    }
                    
                    break;
                case "4":
                    if (this.flexFlow.Contains("ON"))
                    {
                        while (!valido)
                        {
                            this.valido = await GetMacPartNumber(console1, macBox1, pnBox1, passwordBox1, statusConnect1, progressingLabel1, progressBar1, Led1, statusTest1, "..\\Images\\button_1.jpg");
                            if (!valido)
                            {
                                DialogResult res = MessageBox.Show("Desea ignorar esta imagen?", "Decision", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (res == DialogResult.Yes)
                                {
                                    this.skip1 = true;
                                    MessageBox.Show("Imagen ignorada desconecte la unidad previo a proceder con la siguiente unidad", "Imagen ignorada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    if (InvokeRequired)
                                    {
                                        Invoke(new Action(() => macBox1.Text = ""));
                                        Invoke(new Action(() => macBox1.Enabled = false));
                                        Invoke(new Action(() => console1.Text = ""));
                                    }
                                    break;
                                }
                            }
                        }
                        this.valido = false;
                        while (!valido)
                        {
                            this.valido = await GetMacPartNumber(console2, macBox2, pnBox2, passwordBox2, statusConnect2, progressingLabel2, progressBar2, led2, statusTest2, "..\\Images\\button_2.png");
                            if (!valido)
                            {
                                DialogResult res = MessageBox.Show("Desea ignorar esta imagen?", "Decision", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (res == DialogResult.Yes)
                                {
                                    this.skip2 = true;
                                    MessageBox.Show("Imagen ignorada desconecte la unidad previo a proceder con la siguiente unidad", "Imagen ignorada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    if (InvokeRequired)
                                    {
                                        Invoke(new Action(() => macBox2.Text = ""));
                                        Invoke(new Action(() => macBox2.Enabled = false));
                                        Invoke(new Action(() => console2.Text = ""));
                                    }
                                    break;
                                }
                            }
                        }
                        this.valido = false;
                        while (!valido)
                        {
                            this.valido = await GetMacPartNumber(console3, macBox3, pnBox3, passwordBox3, statusConnect3, progressingLabel3, progressBar3, led3, statusTest3, "..\\Images\\button_3.jpg");
                            if (!valido)
                            {
                                DialogResult res = MessageBox.Show("Desea ignorar esta imagen?", "Decision", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (res == DialogResult.Yes)
                                {
                                    this.skip3 = true;
                                    MessageBox.Show("Imagen ignorada desconecte la unidad previo a proceder con la siguiente unidad", "Imagen ignorada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    if (InvokeRequired)
                                    {
                                        Invoke(new Action(() => macBox3.Text = ""));
                                        Invoke(new Action(() => macBox3.Enabled = false));
                                        Invoke(new Action(() => console3.Text = ""));
                                    }
                                    break;
                                }
                            }
                        }
                        this.valido = false;
                        while (!valido)
                        {
                            this.valido = await GetMacPartNumber(console4, macBox4, pnBox4, passwordBox4, statusConnect4, progressingLabel4, progressBar4, led4, statusTest4, "..\\Images\\button_4.jpg");
                            if (!valido)
                            {
                                if (this.skip1 && this.skip2 && this.skip3)
                                {
                                    DialogResult res = MessageBox.Show("Desea abortar la prueba?", "Continuar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (res == DialogResult.Yes)
                                    {
                                        this.abortado = true;
                                        MessageBox.Show("La prueba se ha abortado", "Prueba Abortada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        if (InvokeRequired)
                                            Invoke(new Action(() => startButton.Enabled = true));
                                        this.serialPort.Close();
                                        return;
                                    }
                                }
                                else
                                {
                                    DialogResult res = MessageBox.Show("Desea ignorar esta imagen?", "Decision", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (res == DialogResult.Yes)
                                    {
                                        this.skip4 = true;
                                        MessageBox.Show("Imagen ignorada desconecte la unidad previo a proceder con la siguiente unidad", "Imagen ignorada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        if (InvokeRequired)
                                        {
                                            Invoke(new Action(() => macBox4.Text = ""));
                                            Invoke(new Action(() => macBox4.Enabled = false));
                                            Invoke(new Action(() => console4.Text = ""));
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        if (!this.skip1)
                        {
                            await GetPassword(console1, passwordBox1,progressingLabel1,progressBar1,"..\\Images\\button_1.jpg");
                        }
                        if (!this.skip2)
                        {
                            await GetPassword(console2, passwordBox2,progressingLabel2,progressBar2,"..\\Images\\button_2.png");
                        }
                        if (!this.skip3)
                        {
                            await GetPassword(console3, passwordBox3,progressingLabel3,progressBar3,"..\\Images\\button_3.jpg");
                        }
                        if (!this.skip4)
                        {
                            await GetPassword(console4, passwordBox4,progressingLabel4,progressBar4,"..\\Images\\button_4.jpg");
                        }
                        if (!this.skip1 || !this.skip2 || !this.skip3 || this.skip4)
                        {
                            await BootingUnit(console1, console2, console3, console4,progressingLabel1,progressingLabel2,progressingLabel3,progressingLabel4,progressBar1,progressBar2,progressBar3,progressBar4);
                        }
                        if (!this.skip1)
                        {
                            await MFGOLedStatus("¿Esta parpadeando el led MFGO1?",progressingLabel1,progressBar1);
                            await GetFFeti(estadoPrueba, macBox1.Text,progressingLabel1,progressBar1,statusTest1);
                        }
                        if (!this.skip2)
                        {
                            await MFGOLedStatus("¿Esta parpadeando el led MFGO3?",progressingLabel2,progressBar2);
                            await GetFFeti(estadoPrueba, macBox2.Text,progressingLabel2,progressBar2,statusTest2);
                        }
                        if (!this.skip3)
                        {
                            await MFGOLedStatus("¿Esta parpadeando el led MFGO5?",progressingLabel3,progressBar3);
                            await GetFFeti(estadoPrueba, macBox3.Text,progressingLabel3,progressBar3,statusTest3);
                        }
                        if (!this.skip4)
                        {
                            await MFGOLedStatus("¿Esta parpadeando el led MFGO7?",progressingLabel4,progressBar4);
                            await GetFFeti(estadoPrueba, macBox4.Text,progressingLabel4,progressBar4,statusTest4);
                        }
                    }
                    else
                    {
                        await GetMacPartNumber(console1, macBox1, pnBox1, passwordBox1, statusConnect1, progressingLabel1, progressBar1, Led1, statusTest1, "..\\Images\\button_1.jpg");
                        await GetMacPartNumber(console2, macBox2, pnBox2, passwordBox2, statusConnect2, progressingLabel2, progressBar2, led2, statusTest2, "..\\Images\\button_2.png");
                        await GetMacPartNumber(console3, macBox3, pnBox3, passwordBox3, statusConnect3, progressingLabel3, progressBar3, led3, statusTest3, "..\\Images\\button_3.jpg");
                        await GetMacPartNumber(console4, macBox4, pnBox4, passwordBox4, statusConnect4, progressingLabel4, progressBar4, led4, statusTest4, "..\\Images\\button_4.jpg");
                        await GetPassword(console1, passwordBox1,progressingLabel1,progressBar1,"..\\Images\\button_1.jpg");
                        await GetPassword(console2, passwordBox2,progressingLabel2,progressBar2,"..\\Images\\button_2.png");
                        await GetPassword(console3, passwordBox3,progressingLabel3,progressBar3,"..\\Images\\button_3.jpg");
                        await GetPassword(console4, passwordBox4,progressingLabel4,progressBar4,"..\\Images\\button_4.jpg");
                        await BootingUnit(console1, console2, console3, console4,progressingLabel1,progressingLabel2,progressingLabel3,progressingLabel4,progressBar1,progressBar2,progressBar3,progressBar4);
                        await MFGOLedStatus("¿Esta parpadeando el led MFGO1?",progressingLabel1,progressBar1);
                        await GetFFeti(estadoPrueba, macBox1.Text,progressingLabel1,progressBar1,statusTest1);
                        await MFGOLedStatus("¿Esta parpadeando el led MFGO3?",progressingLabel2,progressBar2);
                        await GetFFeti(estadoPrueba, macBox2.Text,progressingLabel2,progressBar2,statusTest2);
                        await MFGOLedStatus("¿Esta parpadeando el led MFGO5?",progressingLabel3,progressBar3);
                        await GetFFeti(estadoPrueba, macBox3.Text,progressingLabel3,progressBar3,statusTest3);
                        await MFGOLedStatus("¿Esta parpadeando el led MFGO7?",progressingLabel4,progressBar4);
                        await GetFFeti(estadoPrueba, macBox4.Text,progressingLabel4,progressBar4,statusTest4);
                    }
                    break;
            }
        }

       
    }
}
