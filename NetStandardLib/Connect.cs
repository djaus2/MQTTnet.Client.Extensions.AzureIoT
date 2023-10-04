using System;
using System.Reflection;
using System.Text.Json;
using System.Text;
using MQTTnet.Client.Extensions.AzureIoT;

namespace NetStandardLib
{
    public static class Connect2Hub
    {
        public static void Connect()
        {
            IotHubDeviceClient device;
            Console.WriteLine("Initialize...");
            string connectionstring = Secrets.DEVICE_CONNECTION_STRING;
            device = new IotHubDeviceClient(connectionstring);
            device.OpenAsync().Wait();

            Console.WriteLine("Connected");
            int counter = 137;
            var msg = new TelemetryMessage(new { counter, Environment.WorkingSet });

            Console.WriteLine("Sending Telemetry");
            device.SendTelemetryAsync(msg).Wait();
            Console.WriteLine("Sent OK");

        }
    }
}
