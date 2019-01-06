using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
/*
 * 
*/
namespace CjoSurveyApp
{
    class MyComPort : MyDevice, INotifyPropertyChanged
    {
        object processDataLock = new object(); // Lock for serial port read function. Prevents more than one message to be processed at a time
        static readonly byte[] pollNav = { 0xB5, 0x62, 0x01, 0x02, 0x00, 0x00, 0x03, 0x0A };  // Polling message for NAVPOSLLH message      
        SerialPort serialPort;
        byte[] payload;
        string endPoll = "continue";
        StringBuilder package = new StringBuilder("No valid message");

        public event PropertyChangedEventHandler PropertyChanged;

        public MyComPort()
        {                        
        }

        public byte[] Payload
        {
            get
            {
                return payload;
            }
            set
            {
                payload = value;                
            }
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string EndPoll { get => endPoll; set => endPoll = value; }
        public SerialPort SerialPort { get => serialPort; set => serialPort = value; }
        public StringBuilder Package
        {
            get
            {
                return package;
            }
            set
            {
                package = value;
                OnPropertyChanged("Package");
            }
        }

        #region PollData Method for polling NAV messages from Ublox GNSS RX
        public void PollData(string comPort)
        {            
            SerialPort = new SerialPort(comPort, 9600, Parity.None, 8, StopBits.One); 
            SerialPort.DataReceived += DataReceived;
            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nComport failed to open: \n" + ex.ToString()); 
            }
            //While loop to poll messages from Ublox RX
            while (endPoll != "quit")
            {                
                Task write = Task.Factory.StartNew(() => 
                    {
                        if (serialPort.IsOpen)
                        {
                            serialPort.Write(pollNav, 0, 8); 
                        }
                        Thread.Sleep(800); // Polling interval
                    }
                );
                write.Wait();
            }
        }
        #endregion //PollData

        #region DataReceived Method called by the DataReceived event handler
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            
            byte[] buffer = new byte[port.BytesToRead]; // byte array to store read data

            try
            {
                if (port != null)
                {
                    lock (processDataLock)
                    {
                        port?.Read(buffer, 0, buffer.Length);
                        Task process = Task.Factory.StartNew(() => ProcessData(buffer));
                        process.Wait();
                    }                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion //DataReceived method

        #region ProcessData Method
        //To decode the received data from the port. Passes the extracted NAVPOSLLH message to ParseNav class. Writes the decoded position to console  
        private void ProcessData(byte[] Data)
        {
            Queue<byte> currentlyProcessed = new Queue<byte>(Data.Length);
            int count = 0; // variable used to cycle through all bytes in Data[]
            ushort payloadLength = 0;
            
            int currentState = 0;
            
            try
            {
                //while loop used to read all bytes in Data[] to Queue and pass complete message to ParseNav Class. 
                //ParseNav returns a StringBuilder and updates the Package property which fires an INotifyPropertyChanged event
                while (count < Data.Length - 1)
                {
                    byte currentByte = Data[count];
                    currentlyProcessed.Enqueue(currentByte); 
                    bool fail = false;                 
                    //Switch case to check for Message header and Payload Length
                    switch (currentState)
                    {
                        case 0:
                            if (currentByte != 0xB5)
                                fail = true;
                            break;
                        case 1:
                            if (currentByte != 0x62)
                                fail = true;
                            break;
                        case 4:
                            payloadLength = currentByte;
                            break;
                        case 5:
                            payloadLength |= ((ushort)(currentByte << 8));
                            break;
                    }

                    if (fail)
                    {
                        currentState = 0;
                        currentlyProcessed.Clear();
                    }
                    if (currentState != 6)
                    {
                        currentState++;                        
                    }
                    if (currentState == 6)
                    {                        
                        if (payloadLength > 0)
                        {
                            payloadLength--; 
                        }
                        else
                        {
                            currentState++;
                        }                            
                    }                    
                    if (currentState == 8)
                    {
                        try
                        {                            
                            var arr = currentlyProcessed.ToArray();
                            Package = ParseNav.ParseNow(arr);                                                       
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        finally
                        {
                            currentlyProcessed.Clear();
                            currentState = 0;
                        }
                    }
                    count++;                    
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString()); 
            }           
        }
        #endregion // ProcessData method
    }
}
