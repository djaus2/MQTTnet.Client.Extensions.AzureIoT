using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Peripherals.Leds;
using System;
using System.Threading;
using System.Threading.Tasks;

using NetStandardLib;

namespace MeadowApp
{
    // Change F7CoreComputeV2 to F7FeatherV2 (or F7FeatherV1) for Feather boards
    public class MeadowApp : App<F7CoreComputeV2>
    {


        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            Resolver.Log.Info("Hello, Meadow Core-Compute!");

            return base.Run();
        }

        public override Task Initialize()
        {
            //IotHubDeviceClient device;
            Resolver.Log.Info("Initialize...");
            //string connectionstring = Secrets.DEVICE_CONNECTION_STRING;
            //device = new IotHubDeviceClient(connectionstring);
            //device.OpenAsync().Wait();
            Resolver.Log.Info("Run...1");
            NetStandardLib.Connect2Hub.Connect();
            Resolver.Log.Info("Run...2");

            return base.Initialize();
        }
    }
}