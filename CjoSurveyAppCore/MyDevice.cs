using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Microsoft.Win32;

namespace CjoSurveyApp
{
    class MyDevice
    {
        private string comPort;
        private const string VID = "1546";
        private const string PID = "01A8";

        public string ComPort { get => comPort; set => comPort = value; }

        public MyDevice()
        {
            var portNames = SerialPort.GetPortNames();
            ComPort = FindDevice($"^VID_{VID}");                        
        }
        
        private string FindDevice(string stringMatch)
        {
            string port = "0";
            try
            {
                RegistryKey rk1 = Registry.LocalMachine;
                RegistryKey rk2 = rk1.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\USB");
                foreach (var s in rk2.GetSubKeyNames())
                {
                    //Console.WriteLine(s);
                    if (System.Text.RegularExpressions.Regex.IsMatch(s, stringMatch))
                    {
                        RegistryKey rk3 = rk2.OpenSubKey(s);
                        var subKey1 = rk3.GetSubKeyNames();

                        RegistryKey rk4 = rk3.OpenSubKey(subKey1[0]);
                        RegistryKey rk5 = rk4.OpenSubKey("Device Parameters");
                        port = rk5.GetValue("PortName").ToString();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return port;
        }
    }
}
