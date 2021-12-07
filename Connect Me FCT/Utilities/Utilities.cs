using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace Connect_Me_FCT
{
    public class Utilities
    {
        protected string status;
        protected string portCom;
        protected string baudeRate;
        protected string passLogFile;
        protected string failLogFile;
        protected string password;

        public static string[] ListaportCOM()
        {
            return SerialPort.GetPortNames();
        }

        public string FFConsult(string param)
        {
            while (true)
            {
                Thread.Sleep(500);
                try
                {
                    Process process = new Process();
                    process.StartInfo.FileName = "..\\eti\\eti.exe";
                    process.StartInfo.WorkingDirectory = "..\\eti\\";
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.Arguments = param;
                    process.Start();
                }
                catch (Exception err)
                {
                    MessageBox.Show("Se presento el siguiente error al intenta establecer comunicacion con flex flow: " + err.Message);
                }
                Thread.Sleep(2500);
                if (File.Exists("..\\eti\\ffeticlient.out"))
                {
                    string str = "";
                    try
                    {
                        Thread.Sleep(1000);
                        str = File.ReadAllText("..\\eti\\ffeticlient.out");
                    }
                    catch
                    {
                        File.Delete("..\\eti\\ffeticlient.out");
                        continue;
                    }
                    if (str.Contains("OK"))
                    {
                        try
                        {
                            Thread.Sleep(1000);
                            File.Delete("..\\eti\\ffeticlient.out");
                            return "Ok";
                        }
                        catch(Exception err)
                        {
                            continue;
                        }
                        
                    }
                    else
                    {
                        File.Delete("..\\eti\\ffeticlient.out");
                        return "El flujo de la unidad no corresponde a la estacion verifiquelo e intentelo nuevamente";
                    }
                }
                else
                {
                    return "Ha ocurrido un problema con la Flex flow verifique que exista una conexion de red";
                }
            }

        }

        public static void CreaXML(string flexFlowStatus, string portCom, string baudeRate, string passLogFile, string failLogFile,string password)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlElement body = doc.CreateElement(string.Empty, "body", string.Empty);
            doc.AppendChild(body);

            //Flex flow flag
            XmlElement Flex_Flow = doc.CreateElement(string.Empty, "Flex_Flow", string.Empty);
            body.AppendChild(Flex_Flow);

            XmlElement status_Keyword = doc.CreateElement(string.Empty, "Status", string.Empty);
            XmlText status_Value = doc.CreateTextNode(flexFlowStatus);
            status_Keyword.AppendChild(status_Value);
            Flex_Flow.AppendChild(status_Keyword);

            //Station settings
            XmlElement station_settings = doc.CreateElement(string.Empty, "Settings", string.Empty);
            body.AppendChild(station_settings);

            XmlElement portComKeyword = doc.CreateElement(string.Empty, "Port_COM", string.Empty);
            XmlText portComValue = doc.CreateTextNode(portCom);
            portComKeyword.AppendChild(portComValue);
            station_settings.AppendChild(portComKeyword);

            XmlElement baudrate_keyword = doc.CreateElement(string.Empty, "Baude_Rate", string.Empty);
            XmlText baud_rate_value = doc.CreateTextNode(baudeRate);
            baudrate_keyword.AppendChild(baud_rate_value);
            station_settings.AppendChild(baudrate_keyword);

            XmlElement pass_log_file_Path = doc.CreateElement(string.Empty,"Pass_Log_File_Path", string.Empty);
            XmlText pass_path = doc.CreateTextNode(passLogFile);
            pass_log_file_Path.AppendChild(pass_path);
            station_settings.AppendChild(pass_log_file_Path);

            XmlElement fail_log_file_Path = doc.CreateElement(string.Empty, "Fail_Log_File_Path", string.Empty);
            XmlText fail_Path = doc.CreateTextNode(failLogFile);
            fail_log_file_Path.AppendChild(fail_Path);
            station_settings.AppendChild(fail_log_file_Path);

            XmlElement password_Application = doc.CreateElement(string.Empty, "Password_Application", string.Empty);
            XmlText password_value = doc.CreateTextNode(password);
            password_Application.AppendChild(password_value);
            station_settings.AppendChild(password_Application);

            doc.Save("..\\Settings.xml");
        }

        public static void loadDocument(string operacion)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.PreserveWhitespace = true;
                doc.Load("..\\Settings.xml");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = "XML_ENC_RSA_KEY";

            RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(cspParams);
            if (operacion == "encripta")
            {
                try
                {
                    Encrypt(doc, "body", "EncryptedElement1", rsaKey, "rsaKey");
                    doc.Save("..\\Settings.xml");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    rsaKey.Clear();
                }
            }
            else
            {
                try
                {
                    Desencrypta(doc, rsaKey, "rsaKey");
                    doc.Save("..\\Settings.xml");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    rsaKey.Clear();
                }  
            }
        }

        protected static void Encrypt(XmlDocument Doc, string ElementToEncrypt, string EncryptionElementId, RSA alg, string KeyName)
        {
            if (Doc == null)
                throw new ArgumentNullException("Doc");
            if (ElementToEncrypt == null)
                throw new ArgumentNullException("ElementToEncrypt");
            if (EncryptionElementId == null)
                throw new ArgumentNullException("EncryptionElementId");
            if (alg == null)
                throw new ArgumentNullException("Alg");
            if (KeyName == null)
                throw new ArgumentNullException("KeyName");

            XmlElement elementtoEncrypt = Doc.GetElementsByTagName(ElementToEncrypt)[0] as XmlElement;

            if (elementtoEncrypt == null)
                throw new XmlException("El elemento especificado no se ubica en el archivo xml contacte al ingeniero de pruebas");

            Aes sessionKey = null;
            try
            {
                sessionKey = Aes.Create();
                EncryptedXml eXml = new EncryptedXml();
                byte[] encryptedElement = eXml.EncryptData(elementtoEncrypt, sessionKey, false);

                EncryptedData edElement = new EncryptedData();
                edElement.Type = EncryptedXml.XmlEncElementUrl;
                edElement.Id = EncryptionElementId;

                edElement.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncAES256Url);
                EncryptedKey ek = new EncryptedKey();

                byte[] encryptedKey = EncryptedXml.EncryptKey(sessionKey.Key, alg, false);
                ek.CipherData = new CipherData(encryptedKey);
                ek.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncRSA15Url);

                DataReference dRef = new DataReference();

                dRef.Uri = "#" + EncryptionElementId;

                ek.AddReference(dRef);

                edElement.KeyInfo.AddClause(new KeyInfoEncryptedKey(ek));

                KeyInfoName kin = new KeyInfoName();
                kin.Value = KeyName;
                ek.KeyInfo.AddClause(kin);
                edElement.CipherData.CipherValue = encryptedElement;

                EncryptedXml.ReplaceElement(elementtoEncrypt, edElement, false);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (sessionKey != null)
                    sessionKey.Clear();
            }
        }

        protected static void Desencrypta (XmlDocument Doc, RSA Alg, string KeyName)
        {
            if (Doc == null)
                throw new ArgumentNullException("Document");
            if (Alg == null)
                throw new ArgumentNullException("Alg");
            if (KeyName == null)
                throw new ArgumentNullException("KeyName");

            EncryptedXml exml = new EncryptedXml(Doc);
            exml.AddKeyNameMapping(KeyName, Alg);
            exml.DecryptDocument();
        }

        public Tuple<string,string,string,string,string,string> LeeXML(string Doc)
        {
            XmlTextReader reader = new XmlTextReader(Doc);
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch(reader.Name.ToString())
                    {
                        case "Status":
                            this.status = reader.ReadString();
                            break;
                        case "Port_COM":
                            this.portCom = reader.ReadString();
                            break;
                        case "Baude_Rate":
                            this.baudeRate = reader.ReadString();
                            break;
                        case "Pass_Log_File_Path":
                            this.passLogFile = reader.ReadString();
                            break;
                        case "Fail_Log_File_Path":
                            this.failLogFile = reader.ReadString();
                            break;
                        case "Password_Application":
                            this.password = reader.ReadString();
                            break;
                        default:
                            continue;
                    }
                }
            }
            reader.Close();
            return Tuple.Create(this.status,this.portCom,this.baudeRate, this.passLogFile, this.failLogFile, this.password);
        }
    }
}

