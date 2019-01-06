using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CjoSurveyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            MyComPort comPort = new MyComPort();
            Task startPolling = Task.Factory.StartNew(() => comPort.ListenForData(comPort.ComPort));

            comPort.EndPoll = Console.ReadLine();

            try
            {
                comPort.SerialPort.Close();
                comPort.SerialPort.Dispose();
            }
            catch (Exception)
            {

                throw;
            }
            Console.ReadKey();
        }
    }
}
