using System;
using System.Collections.Generic;
using System.Text;
using FutureConcepts.Media.Contract;
using System.Reflection;
using System.Diagnostics;

namespace FutureConcepts.Media.MicrowaveReceiverController
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Type implementation;
                string TCPAddress, ComPort;
                int TCPPort, baud;
                bool UsesTCP;
                SetupSession(args, out implementation, out TCPAddress, out TCPPort, out UsesTCP, out ComPort, out baud);

                using (MicrowaveReceiver rx = (MicrowaveReceiver)Activator.CreateInstance(implementation, TCPAddress, TCPPort, UsesTCP, ComPort, baud))
                {
                    if (rx.Connected)
                    {
                        Console.WriteLine("Ready.            (type help? to see full command listing)");
                        Console.Write(" > ");
                        string command = Console.ReadLine();
                        string[] set = command.Split(new char[] { ' ' }, 2);

                        int stressRepeat = 1;

                        int stressSuccess = 0;
                        int stressFailure = 0;
                        Stopwatch stopwatch = new Stopwatch();
                        UInt64 totalTime = 0;

                        while (!set[0].Equals("quit"))
                        {
                            stressSuccess = 0;
                            stressFailure = 0;
                            totalTime = 0;
                            for (int i = 0; i < stressRepeat; i++)
                            {
                                if ((set[0] == "stress") && (i > 0)) break;

                                switch (set[0])
                                {
                                    case "help":
                                    case "help?":
                                    case "?":
                                        ShowHelp();
                                        break;
                                    case "band?": Console.Out.WriteLine("Band: " + rx.GetBand());
                                        break;
                                    case "freq?":
                                        try
                                        {
                                            stopwatch.Stop();
                                            stopwatch.Reset();
                                            stopwatch.Start();
                                            MicrowaveTuning t = rx.GetTuning();
                                            stopwatch.Stop();
                                            

                                            Console.WriteLine("Current Tuning Information");
                                            Console.WriteLine("    Frequency: " + t.Frequency + " Hz");
                                            Console.WriteLine("    Bandwidth: " + t.ChannelBandwidth + " Hz");
                                            Console.WriteLine("    Transport: " + t.TransportMode);
                                            Console.WriteLine("   Modulation: " + t.Modulation);
                                            Console.WriteLine("          FEC: " + t.ForwardErrorCorrection);
                                            Console.WriteLine("   Guard Int.: " + t.GuardInterval);
                                            Console.WriteLine("   Encryption: " + ((t.Encryption == null) ? "<null>" : (t.Encryption.Type + " " + t.Encryption.KeyLength + " bit")));
                                            stressSuccess++;
                                            totalTime += (UInt64)stopwatch.ElapsedMilliseconds;
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.ToString());
                                            stressFailure++;
                                        }
                                        break;
                                    case "str?":
                                    case "strength?":
                                        try
                                        {
                                            stopwatch.Stop();
                                            stopwatch.Reset();
                                            stopwatch.Start();
                                            MicrowaveLinkQuality q = rx.GetLinkQuality();
                                            stopwatch.Stop();

                                            Console.WriteLine("Link Quality");
                                            Console.WriteLine("         RCL: " + q.ReceivedCarrierLevel + " dB");
                                            Console.WriteLine("         SNR: " + q.SignalToNoiseRatio + " dB");
                                            Console.WriteLine("      Pre EC: " + q.BitErrorRatioPre);
                                            Console.WriteLine("     Post EC: " + q.BitErrorRatioPost);
                                            Console.WriteLine("  Tuner Lock: " + q.TunerLocked);
                                            Console.WriteLine("  Demod Lock: " + q.DemodulatorLocked);
                                            Console.WriteLine("     TS Lock: " + q.TransportStreamLocked);
                                            Console.WriteLine("    FEC Lock: " + q.FECLocked);
                                            Console.WriteLine("Decoder Lock: " + q.DecoderLocked);
                                            stressSuccess++;
                                            totalTime += (UInt64)stopwatch.ElapsedMilliseconds;
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.ToString());
                                            stressFailure++;
                                        }
                                        break;
                                    //case "band":
                                    //    MicrowaveReceiver.OperatingBand band = MicrowaveReceiver.OperatingBand.NotSet;
                                    //    switch (set[1])
                                    //    {
                                    //        case "S": band = MicrowaveReceiver.OperatingBand.S;
                                    //            break;
                                    //        case "L": band = MicrowaveReceiver.OperatingBand.L;
                                    //            break;
                                    //        case "C": band = MicrowaveReceiver.OperatingBand.C;
                                    //            break;
                                    //        case "X": band = MicrowaveReceiver.OperatingBand.X;
                                    //            break;
                                    //        default: band = MicrowaveReceiver.OperatingBand.NotSet;
                                    //            break;
                                    //    }
                                    //    bool result = rx.SetBand(band);
                                    //    Console.Out.WriteLine(result ? "Successful!" : "Failed!");
                                    //    break;
                                    case "freq":
                                        try
                                        {
                                            int freq = -1;
                                            if (int.TryParse(set[1], out freq))
                                            {
                                                MicrowaveCapabilities caps = rx.GetCapabilities();
                                                if (caps.AutoTuningRequirements != (int)MicrowaveTuning.Parameters.Frequency)
                                                {
                                                    Console.WriteLine("  WARNING: not all AutoTune requirements met");
                                                }
                                                
                                                rx.SetTuning(new MicrowaveTuning(caps.AutoTuning) { FrequencyMHz = (UInt64)freq });
                                                Console.WriteLine("OK");
                                                stressSuccess++;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Failure");
                                            Console.WriteLine(ex.ToString());
                                            stressFailure++;
                                        }
                                        break;
                                    case "test":
                                        Console.WriteLine("Test Connection: " + rx.TestConnection());
                                        break;
                                    case "peak":
                                        Console.WriteLine("Not implemented");
                                        break;
                                    case "stress":
                                        try
                                        {
                                            stressRepeat = int.Parse(set[1]);
                                            if (stressRepeat <= 1)
                                            {
                                                stressRepeat = 1;
                                                Console.WriteLine("Stress test mode OFF");
                                            }
                                            else
                                            {
                                                Console.WriteLine("Commands will be executed " + stressRepeat + " times");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex);
                                        }
                                        break;
                                    case "stress?":
                                        if (stressRepeat <= 1)
                                        {
                                            Console.WriteLine("Stress test mode OFF");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Stress repeat = " + stressRepeat);
                                        }
                                        break;
                                    case "info?":
                                        Console.WriteLine(rx.GetDeviceInfo());
                                        break;
                                    case "raw":
                                        Console.WriteLine(rx.RawCommand(set[1]));
                                        break;
                                    default:
                                        Console.WriteLine("-not a command-");
                                        break;
                                }
                            }

                            if (stressRepeat > 1)
                            {
                                Console.WriteLine();
                                Console.WriteLine("   -- Stress Test Results --");
                                Console.WriteLine(" " + stressSuccess + " / " + stressRepeat + " Succeeded");
                                Console.WriteLine(" " + stressFailure + " / " + stressRepeat + " Failed");
                                Console.WriteLine(" " + (UInt64)((double)totalTime / (double)stressSuccess) + " ms per successful operation (avg)");
                            }


                            Console.Write(" > ");
                            command = Console.ReadLine();
                            set = command.Split(new char[] { ' ' });
                        }
                    }
                    else
                    {
                        Console.Out.WriteLine("Not Connected");
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("FAILTOWN");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("press enter to quit");
                Console.ReadLine();
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("-----Commands:-----");
            Console.WriteLine("  band?");
            Console.WriteLine("    displays the band the receiver is tuned to");
            Console.WriteLine("  str?");
            Console.WriteLine("    displays the link quality");
            Console.WriteLine("  freq?");
            Console.WriteLine("    displays the current tuning");
            Console.WriteLine("  freq <frequency in Hz>");
            Console.WriteLine("    sets the tuning to the specified frequency, in Hz");
            Console.WriteLine("    (ex 2450 MHz = 2450000000)");
            Console.WriteLine("  test");
            Console.WriteLine("    calls TestConnection");
            Console.WriteLine("  info?");
            Console.WriteLine("    calls GetDeviceInfo -- should show serial number, etc");
            Console.WriteLine("  quit");
            Console.WriteLine("    exit the program");
            Console.WriteLine("  stress <n>");
            Console.WriteLine("    stress test mode: causes any subsequent commands to be executed <n> times.");
            Console.WriteLine("  stress?");
            Console.WriteLine("    display stress mode status");
            Console.WriteLine("  raw <packet>");
            Console.WriteLine("    sends raw data to the device. Check info? for device-specific syntax.");
            Console.WriteLine("-------------------");
        }

        private static void SetupSession(string[] args, out Type implementation, out string TCPAddress, out int TCPPort, out bool UsesTCP, out string ComPort, out int baud)
        {
            bool hasConnectionInfo = false;

            ComPort = "COM8";
            baud = 9600;
            TCPPort = 7004;
            UsesTCP = true;
            TCPAddress = "10.0.201.212";

            string connectionStringInput = null;
            if (args.Length == 1)
            {
                connectionStringInput = args[0];
            }
            do
            {
                if (connectionStringInput == null)
                {
                    Console.WriteLine("Enter connection string (e.x. ip.add.re.ss:port -or- COMx@baudrate)");
                    Console.Write(" > ");
                    connectionStringInput = Console.ReadLine();
                }

                if (connectionStringInput.Contains("@"))
                {
                    string[] parts = connectionStringInput.Split('@');
                    if ((parts.Length == 2) && (int.TryParse(parts[1], out baud)))
                    {
                        Console.WriteLine("Connecting via serial: " + parts[0] + " @ " + baud + " baud");
                        ComPort = parts[0];
                        UsesTCP = false;
                        hasConnectionInfo = true;
                    }
                }
                else if (connectionStringInput.Contains(":"))
                {
                    string[] parts = connectionStringInput.Split(':');
                    if ((parts.Length == 2) && (int.TryParse(parts[1], out TCPPort)))
                    {
                        Console.WriteLine("Connecting via TCP, host = " + parts[0] + " port = " + TCPPort);
                        TCPAddress = parts[0];
                        UsesTCP = true;
                        hasConnectionInfo = true;
                    }
                }

                if (!hasConnectionInfo)
                {
                    connectionStringInput = null;
                }

            } while (!hasConnectionInfo);

            hasConnectionInfo = false;


            List<Type> impls = GetImplementations();
            int index;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Select class:");
                
                for (int i = 0; i < impls.Count; i++)
                {
                    Console.WriteLine(" " + i + ". " + impls[i].Name);
                }
                Console.Write(" enter # > ");
                string input = Console.ReadLine();

                hasConnectionInfo = true;

                if (!int.TryParse(input, out index))
                {
                    hasConnectionInfo = false;
                    Console.WriteLine("please enter a real number");
                }
                if ((index >= impls.Count) || (index < 0))
                {
                    hasConnectionInfo = false;
                    Console.WriteLine("Enter a number in the correct range");
                }
            }
            while (!hasConnectionInfo);

            implementation = impls[index];
            
        }

        private static List<Type> GetImplementations()
        {
            List<Type> subclasses = new List<Type>();
            Type mrxParent = typeof(MicrowaveReceiver);
            Assembly me = Assembly.GetAssembly(mrxParent);
            foreach (Type t in me.GetTypes())
            {
                if ((mrxParent.IsAssignableFrom(t)) && (t != mrxParent))
                {
                    subclasses.Add(t);
                }
            }

            return subclasses;
        }
    }
}

