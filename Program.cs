using nanoFramework.Networking;
using Secrets; // Make sure you adjust the template
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using nanoFramework.Hardware.PiPico;

namespace RpiPicoWDemoApp
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("nanoFramework PI-Pico tests!");

            // var uid = Utilities.UniqueDeviceId;
            //Debug.WriteLine(uid);


            Debug.WriteLine("Connect to WiFi");
            CancellationTokenSource cs = new(30000);
            var success = false;
            while (!success)
            {
                success = WifiNetworkHelper.ConnectDhcp(WiFi.Ssid, WiFi.Password, requiresDateTime: true, token: cs.Token);

                if (!success)
                {
                    // Something went wrong, you can get details with the ConnectionError property:
                    Debug.WriteLine($"Can't connect to the network, error: {WifiNetworkHelper.Status}");
                    if (WifiNetworkHelper.HelperException != null)
                    {
                        Debug.WriteLine($"ex: {WifiNetworkHelper.HelperException}");
                    }
                }
                else
                {
                    Debug.WriteLine("Connection Succeeded.");
                    Debug.WriteLine($"RTC = {DateTime.UtcNow}");
                    Debug.WriteLine($"IP = {System.Net.NetworkInformation.IPGlobalProperties.GetIPAddress()}");
                    Debug.WriteLine($"MAC = {GetMacAsString()}");
                    break;
                }
            }

            Debug.WriteLine("Tests end.");

            OnboardAdcDevice adcDevice = new OnboardAdcDevice();

            while(true)
            {
                var mcuTemp = adcDevice.GetInternalTemperature();
                var mcuVoltage = adcDevice.GetInternalVoltage();
                Debug.WriteLine($"T = {mcuTemp} C");
                Debug.WriteLine($"V = {mcuVoltage} V");
                Thread.Sleep(5000);
            }

            //Thread.Sleep(Timeout.Infinite);
        }

        public static string GetMacAsString(char seperator = ':')
        {
            // TODO: Word has it that string builder is slower on nF! (if required, suppress S1643)!
            var sb = new StringBuilder();
            foreach (byte b in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].PhysicalAddress)
            {
                sb.Append(b.ToString("X2"));
                sb.Append(seperator);
            }
            return sb.ToString().TrimEnd(seperator);
        }
    }
}
