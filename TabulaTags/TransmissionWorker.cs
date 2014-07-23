using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using System.Diagnostics;
using System.IO;
using LibUsbDotNet.DeviceNotify;


namespace TabulaTags
{
    public class TransmissionWorker
    {
        public Dispatcher d;
        string message = "default string";

        public static UsbDeviceFinder MyUsbFinder = new UsbDeviceFinder("36ffd9053441373932461043"); //////////////should be made more modular, workable for now tho
        public static DateTime LastDataEventDate = DateTime.Now;
        public static UsbDevice MyUsbDevice;
        UsbEndpointReader reader; UsbEndpointWriter writer;

        public Responder RESPONSE { get; set; }

        public string debugLog = "start of log \n";
        
      
        public void theThing()
        {
            
            //pystuff();
            find_and_open();

            //testing closing to start
            try { CMDSend("ok"); }
            catch (Exception e) { }
          
            
            while (true)
            {
                display(receive());///////////////////////this loops inside recieve(); should only need to be called again if there is an exception
                //CMDSend("ok");//testing open close

            }

        }

        #region showing stuff +dispatcher 
        public void display(string s)
        {
            message = s;
            d.Invoke(new Action(show));

        }

        public void display()
        {
            message = "disp";
            d.Invoke(new Action(show));

        }

        public void show()
        {
            RESPONSE.show(message);
        }
        #endregion

        #region transmission 


        public int send(string p)
        {

            //byte[] message = new byte[p.Length * sizeof(char)];
            //System.Buffer.BlockCopy(p.ToCharArray(), 0, message, 0, message.Length);
            return send(Encoding.Default.GetBytes(p));
            //send(message);
        }

        public int send(byte[] messageBytes)
        {
            int bytesWritten;
            ErrorCode ec = writer.Write(messageBytes, 3000, out bytesWritten);
            
            //UsbError err = 
            //var v = err.Win32ErrorNumber;
            

            if (ec != ErrorCode.None)
            {
                int i = UsbDevice.LastErrorNumber;
                return i;
                //throw new Exception(UsbDevice.LastErrorString);
            }

            return bytesWritten;

        }

        public void CMDSend(string message)
        {


            
            UsbDevice.Exit(); //close so that writer can claim interface
            MyUsbDevice.Close();

            
            Process p = new Process();
            String dir = Directory.GetCurrentDirectory();
            dir = dir.Substring(0, dir.Length - ("/bin/debug".Length - 1));
            p.StartInfo = new ProcessStartInfo(@"C:\Python27\python.exe", 
                                                Path.Combine(dir, "SendScript.py")){
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true, 
                WorkingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            };

            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            string error = p.StandardError.ReadToEnd();
            p.WaitForExit();

            UsbDevice.Exit(); //close so that writer can claim interface
            MyUsbDevice.Close();

            find_and_open();//open so that reader can claim interface

            if (p.ExitCode != 0)
            {
                Console.WriteLine(p.ExitCode);
            }


        }

        public string receive()
        {
            message = "";
            try
            {

                byte[] readBuffer = new byte[1024];

                ErrorCode ec = new ErrorCode();
                while (ec == ErrorCode.None)
                {
                    int bytesRead;
                    ec = reader.Read(readBuffer, 1000, out bytesRead);
                    //message = Encoding.Default.GetString(readBuffer, 0, bytesRead);
                    //message = readBuffer.ToString();
                    //char[] chars = new char[readBuffer.Length / sizeof(char)];
                    //System.Buffer.BlockCopy(readBuffer, 0, chars, 0, readBuffer.Length);
                    //message= new string(chars);
                    message = readBuffer[0].ToString() + " " + readBuffer[1].ToString() + " " + readBuffer[2].ToString() + " " + readBuffer[3].ToString() + readBuffer[4].ToString() + " " + readBuffer[5].ToString() + " " + readBuffer[5].ToString();
                    display(message);//////////////////////////////////////////////call out here
                    
                    if (!message.Contains("TimedOut"))
                        debugLog += " LOG: " + message;

                    readBuffer = new byte[1024];
                    //CMDSend("ok");
                }

                message=ec.ToString();
                return message;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public UsbDevice find_and_open()
        {
            UsbRegDeviceList devices = UsbDevice.AllDevices;/////////////////////////////modify to search devices for sifteo base
            MyUsbDevice = devices[1].Device;
            IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;

            if (!ReferenceEquals(wholeUsbDevice, null))
            {
                // This is a "whole" USB device. Before it can be used, 
                // the desired configuration and interface must be selected.

                // Select config #1
                wholeUsbDevice.SetConfiguration(1);

                // Claim interface #0.
                wholeUsbDevice.ClaimInterface(0);
                // open read endpoint 1.
                reader = MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
                
                // open write endpoint 1.
                writer = MyUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep02);
                
                //writer = MyUsbDevice.OpenEndpointWriter(new WriteEndpointID());
            }
            return MyUsbDevice;



        }


        #endregion

   

    }
}
