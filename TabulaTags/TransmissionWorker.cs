using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Main;

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
      
        public void theThing()
        {
            //while (true)
            //    Console.WriteLine("Alpha.Beta is running in its own thread.");
            //pystuff();

            find_and_open();
            while (true)
            {
                display(receive());///////////////////////this loops then loops inside; should only need to be called again if there is an exception
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


        public void send(string p)
        {

            byte[] message = new byte[p.Length * sizeof(char)];
            System.Buffer.BlockCopy(p.ToCharArray(), 0, message, 0, message.Length);
            send(message);
        }

        public void send(byte[] message)
        {
            ErrorCode ec = ErrorCode.None;
            int bytesWritten;

            ec = writer.Write(message, 2000, out bytesWritten);

            if (ec != ErrorCode.None) throw new Exception(UsbDevice.LastErrorString);

        }

        public string receive()
        {
            try
            {

                byte[] readBuffer = new byte[1024];

                ErrorCode ec = new ErrorCode();
                while (ec == ErrorCode.None)
                {
                    int bytesRead;
                    ec = reader.Read(readBuffer, 100, out bytesRead);
                    //message = Encoding.Default.GetString(readBuffer, 0, bytesRead);
                    //message = readBuffer.ToString();

                    char[] chars = new char[readBuffer.Length / sizeof(char)];
                    System.Buffer.BlockCopy(readBuffer, 0, chars, 0, readBuffer.Length);
                    message= new string(chars);

                    display(message);//////////////////////////////////////////////call out here
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
            }

            // open read endpoint 1.
            reader = MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);

            // open write endpoint 1.
            writer = MyUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);

            return MyUsbDevice;


        }


        #endregion

   

    }
}
