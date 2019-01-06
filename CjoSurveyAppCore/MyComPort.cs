using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CjoSurveyApp
{

    class MyComPort : MyDevice
    {
        object processDataLock = new object();
        static readonly byte[] pollNav = { 0xB5, 0x62, 0x01, 0x02, 0x00, 0x00, 0x03, 0x0A };
        MyDevice myDevice;
        SerialPort serialPort;
        byte[] payload;
        string endPoll = "continue";

        public MyComPort()
        {
            myDevice = new MyDevice();             
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
                OutPutMessage(value);
            }
        }

        public string EndPoll { get => endPoll; set => endPoll = value; }
        public SerialPort SerialPort { get => serialPort; set => serialPort = value; }

        public void ListenForData(string comPort)
        {            
            serialPort = new SerialPort(comPort, 9600, Parity.None, 8, StopBits.One);
            serialPort.DataReceived += DataReceived;
            serialPort.Open();
            
            while (endPoll != "quit")
            {                
                Task write = Task.Factory.StartNew(() => 
                    {
                        if (serialPort.IsOpen)
                        {
                            serialPort.Write(pollNav, 0, 8); 
                        }
                        Thread.Sleep(100);
                    }
                );
                write.Wait();
            }
        }

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

        //To decode the received data from the port.   
        private void ProcessData(byte[] Data)
        {
            Queue<byte> currentlyProcessed = new Queue<byte>(Data.Length);
            int count = 0;
            ushort payloadLength = 0;
            StringBuilder package;
            int currentState = 0;
            
            try
            {
                while (count < Data.Length - 1)
                {
                    byte currentByte = Data[count];
                    currentlyProcessed.Enqueue(currentByte);
                    bool fail = false;                 

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
                        //case 2:
                        //    if (currentByte != 0x01)
                        //        fail = true;
                        //    break;
                        //case 3:
                        //    if (currentByte != 0x02)
                        //        fail = true;
                        //    break;
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
                    if (!(currentState == 6))
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
                            package = ParseNav.ParseNow(arr);
                            Console.WriteLine(package.ToString());                            
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
        #region ReadComPort stream.Read
        public Task<string> ReadComPort()
        {
            using (serialPort = new SerialPort(myDevice.ComPort, 9600, Parity.None, 8, StopBits.One))
            {
                serialPort.Open();
                byte[] buffer = new byte[4096];
                Stream stream = serialPort.BaseStream;

                while (true)
                {
                    stream.Read(buffer, 0, buffer.Length);             
                }

            }                  
            
            //Action kickoffRead = null;
            //kickoffRead = (Action)(() => serialPort.BaseStream.BeginRead(buffer, 0, buffer.Length, delegate (IAsyncResult ar)
            //{
            //    try
            //    {
            //        int count = serialPort.BaseStream.EndRead(ar);
            //        byte[] dst = new byte[count];
            //        Buffer.BlockCopy(buffer, 0, dst, 0, count);
            //        RaiseAppSerialDataEvent(dst);
            //    }
            //    catch (Exception exception)
            //    {
            //        Console.WriteLine(exception.ToString());
            //    }
            //    kickoffRead();
            //}, null)); kickoffRead();
            
        }
        #endregion //ReadComPort
        
        public void ReadComPortAsync()
        {
            using (serialPort = new SerialPort(myDevice.ComPort, 9600, Parity.None, 8, StopBits.One))
            {
                serialPort.Open();
                byte[] buffer = new byte[4096];
                Stream stream = serialPort.BaseStream;
                stream.Read(buffer, 0, buffer.Length);
                
                
                Task decodeData = new Task(() => ProcessData(buffer));
                decodeData.Start();
                decodeData.Wait();
 

            }
        }

        //    byte[] readBytes;
        //    int count = 0;            
        //    Queue<byte> currentlyProcessed = new Queue<byte>(1024);


        //    using (var serialPort = new SerialPort(myDevice.ComPort, 9600, Parity.None, 8, StopBits.One)
        //    {
        //        Handshake = Handshake.None,
        //        ReadTimeout = 1000,
        //        WriteTimeout = 1000
        //    })
        //    {
        //        serialPort.Open();
        //        Stream stream = serialPort.BaseStream;
        //        stream.ReadAsync(readBytes = new byte[1024], 0, 1024);

        //        try
        //        {
        //            while (count < 1024)
        //            {
        //                int temp = readBytes[count];
        //                byte currentByte = (byte)temp;
        //                currentlyProcessed.Enqueue(currentByte);
        //                bool fail = false;
        //                ushort payloadLength = 0;

        //                switch (currentState)
        //                {
        //                    case 0:
        //                        if (currentByte != 0xB5)
        //                            fail = true;
        //                        break;
        //                    case 1:
        //                        if (currentByte != 0x62)
        //                            fail = true;
        //                        break;
        //                    case 4:
        //                        payloadLength = currentByte;
        //                        break;
        //                    case 5:
        //                        payloadLength |= ((ushort)(currentByte << 8));
        //                        break;
        //                }

        //                if (fail)
        //                {
        //                    currentState = 0;
        //                    currentlyProcessed.Clear();
        //                }
        //                else if (currentState != 6)
        //                {
        //                    currentState++;
        //                }
        //                else if (currentState == 6)
        //                {
        //                    if (payloadLength > 0)
        //                        payloadLength--;
        //                    else
        //                        currentState++;
        //                }

        //                if (currentState == 8)
        //                {
        //                    try
        //                    {
        //                        var arr = currentlyProcessed.ToArray();
        //                        package = ParseNav.ParseNow(arr);
        //                        Console.WriteLine(package.ToString());
        //                    }
        //                    catch (Exception)
        //                    {

        //                        throw;
        //                    }
        //                    finally
        //                    {
        //                        currentlyProcessed.Clear();
        //                        currentState = 0;
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }
        //    }
        //    //stream.
        //    //serialPort.DataReceived += new SerialDataReceivedEventHandler(ReadDataEventHandler);
        //    //serialPort.Open();
        //    Console.ReadKey();
        //}
        private void OutPutMessage(byte[] payload)
        {
            StringBuilder sb = ParseNav.ParseNow(payload);
            //foreach (var item in payload)
            //{
            //    sb.Append(item.ToString());
            //}
            Console.WriteLine(sb.ToString());
            Console.WriteLine();
            
        }

        private void ReadDataEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            int count = serialPort.BytesToRead;
            byte[] temp = new byte[count];
            if (count != 0)
            {
                for (int i = 0; i < count; i++)
                {
                    temp[i] = (byte)serialPort.ReadByte();
                    //Console.WriteLine(payload[i]);
                }
                Payload = temp;
            }
        }


        //public async void ReadFromPort()
        //{
        //    byte[] buffer = new byte[5];
        //    int read = await serialPort.BaseStream.ReadAsync(buffer, )
        //}
    }
}
