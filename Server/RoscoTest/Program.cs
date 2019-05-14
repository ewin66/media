using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FutureConcepts.Media.Server.RoscoTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TestRosco();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Test failed!");
                Console.WriteLine(exc.Message);
            }
        }

        private static void TestRosco()
        {
            RVisionControl ctl = new RVisionControl();
            Console.WriteLine("Opening COM1");
            ctl.Open("COM1");
            byte[] deviceInfo = ctl.GetDeviceInfo();
            Console.WriteLine("DeviceInfo:");
            for (int i = 0; i < deviceInfo.Length; i++)
            {
                Console.Write("{0} ", deviceInfo[i]);
            }
            Console.WriteLine(" ");
            byte[] revisionInfo = ctl.GetRevisionInfo();
            Console.WriteLine("RevisionInfo:");
            for (int i = 0; i < revisionInfo.Length; i++)
            {
                Console.Write("{0} ", revisionInfo[i]);
            }
            Console.WriteLine(" ");
            string serialNumbers = Encoding.ASCII.GetString(ctl.GetSerialNumbers());
            Console.WriteLine("Serial Numbers:");
            Console.WriteLine(serialNumbers);
            Console.WriteLine("Sending PanTiltUp");
            ctl.PanTiltUp(40);
            Thread.Sleep(2000);
            Console.WriteLine("Sending PanTiltStop");
            int panAngle;
            int tiltAngle;
            ctl.PanTiltStop(out panAngle, out tiltAngle);
            Console.WriteLine("PanAngle={0} TiltAngle={1}", panAngle, tiltAngle);
            ctl.Dispose();
            ctl = null;
        }
    }
}
