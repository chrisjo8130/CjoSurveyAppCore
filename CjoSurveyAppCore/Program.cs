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
            comPort.PropertyChanged += ComPort_PropertyChanged;
            Task startPolling = Task.Factory.StartNew(() => comPort.PollData(comPort.ComPort)); 

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

        private static void ComPort_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine((sender as MyComPort).Package.ToString()); 
        }
    }
}
