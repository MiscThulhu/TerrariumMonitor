using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace TerrariumMonitor
{
    public class SerialReader
    {
        public string COM { get; set; }
        public int Boud { get; set; }
        public bool Readable { get; set; }

        private string str_SerialOut;
        private string str_InData;

        private SerialPort s_Reader;

        public string Get_SerialOut()
        {
            return str_SerialOut;
        }

        public string OpenPort()
        {
            string str_Report = "Port Opened";

            s_Reader = new SerialPort();
            s_Reader.PortName = COM;
            s_Reader.BaudRate = Boud;

            try
            {
                s_Reader.Open();
                s_Reader.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            }
            catch (UnauthorizedAccessException)
            {
                str_Report = "ERROR: Acces denied / Port in Use.";
            }
            catch (ArgumentOutOfRangeException)
            {
                str_Report = "ERROR: Port Settings Incorrect.";
            }
            catch (ArgumentException)
            {
                str_Report = "ERROR: Port Name Invalid.";
            }
            catch (InvalidOperationException)
            {
                str_Report = "ERROR: Port Already Open.";
            }
            catch
            {
                str_Report = "ERROR: Failed to Open Port.";
            }

            return str_Report;
        }

        public void Close_Port()
        {
            s_Reader.Close();
        }

        public SerialReader Clone_Reader()
        {
            SerialReader sr_Clone = (SerialReader)this.MemberwiseClone();

            sr_Clone.Close_Port();

            return sr_Clone;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort s_Port = (SerialPort)sender;
            str_InData += s_Port.ReadExisting();

            if (str_InData[0] != '<' && str_InData != "")
            {
                for (int i = 0; i < str_InData.Length; i++)
                {
                    if (str_InData[i] == '<')
                    {
                        str_InData = str_InData.Remove(0, i);
                    }
                }
            }

            if (str_InData[str_InData.Length - 1] == '>')
            {
                str_SerialOut = str_InData;
                str_InData = "";
                Readable = true;
            }
        }

    }
}
