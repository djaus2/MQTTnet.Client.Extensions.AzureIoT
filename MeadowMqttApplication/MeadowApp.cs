using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Peripherals.Leds;
using System;
using System.Threading;
using System.Threading.Tasks;
using Meadow.Gateways.Bluetooth;

using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using Meadow.Hardware;
using System.Configuration;

using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using Serilog;
using MQTTnet.Adapter;
using MQTTnet.Client.Receiving;
using System.Security.Authentication;
//https://dev.to/eduardojuliao/basic-mqtt-with-c-1f88

//using System.Security.Authentication;

namespace MeadowApplication3
{
    // Change F7CoreComputeV2 to F7FeatherV2 (or F7FeatherV1) for Feather boards
    public class MeadowApp : App<F7CoreComputeV2>
    {
        public override  Task Run()
        {
            Resolver.Log.Info("Run...");

            //var client = new MqttClient("azure-iot-device");
            //var client = new MqttClient("azure-iot-device-mqtt");
            //azure - iot - device - mqtt

            Resolver.Log.Info("Hello, Meadow Core-Compute!");


            while (true) { Thread.Sleep(500); };

            return base.Run();
        }

        private static DeviceClient s_deviceClient;
        private static string s_connectionString = "";

        const string AZ_IOT_HUB_CLIENT_C2D_SUBSCRIBE_TOPIC = "\"devices/+/messages/devicebound/#\"";

        private static async Task SendDeviceToCloudMessagesAsync()
        {
            // Initial telemetry values
            double minTemperature = 20;
            double  minHumidity = 60;
            Random rand = new Random();
            while (true)
            {
                double currentTemperature = minTemperature + rand.NextDouble() * 15;
                double currentHumidity = minHumidity + rand.NextDouble() * 20;

                // Create JSON message
                var telemetryDataPoint = new
                {
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                Message message = new Message(Encoding.ASCII.GetBytes(messageString));

                // Add a custom application property to the message.
                // An IoT hub can filter on these properties without access to the message body.
                message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");


                string json = JsonConvert.SerializeObject(new { message = "Heyo :)", sent = DateTimeOffset.UtcNow });
                var result = await _mqttClient.PublishAsync("ozz2dev/topic/json", json);

                //Task.Delay(1000).GetAwaiter().GetResult();

                // Send the telemetry message
                //await s_deviceClient.SendEventAsync(message);
                //Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(5000);
                Resolver.Log.Info($"MsgQ Count: {_mqttClient.PendingApplicationMessagesCount}");
            }
        }


        private static IManagedMqttClient _mqttClient;


        private static void BuildTcpOptions(MqttClientTcpOptions message)
        {
            var optionstcp = new MqttClientTcpOptions()
            {
                Server = Secrets.IOT_CONFIG_IOTHUB_FQDN,
                Port = Secrets.MqttPort,
                //AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork,    
                //ClientId = "device1",
                //UserName = "myIoTHub.azure-devices.net/device1/api-version=2018-06-30",
                //Password = "SharedAccessSignature sr=myIoTHub.azure-devices.net%2Fdevices%2Fdevice1&sig=****&se=1592830262",
                // ProtocolVersion = MQTTnet.Core.Serializer.MqttProtocolVersion.V311,
                TlsOptions = new MqttClientTlsOptions() { UseTls = true }//,
                                                                         //CleanSession = true
            };
        }

        private  async void NetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            Resolver.Log.Info("WiFi Connected. Starting MQTT...");
            //DisplayController.Instance.ShowConnected();

            // Connect to the IoT hub using the MQTT protocol
            //string OPTION_CONNECTION_TIMEOUTOPTION_CONNECTION_TIMEOUT = "300";
            //ClientOptions options = new ClientOptions()
            //{
            //    //ModelId = "tempModuleId",
            //    //DefaultOperationTimeoutInMilliseconds = 300000
            //};



            //options.
            //DeviceClient.DefaultOperationTimeoutInMilliseconds = 300000;
            Console.WriteLine(DeviceClient.DefaultOperationTimeoutInMilliseconds);
            
            // https://dev.to/eduardojuliao/basic-mqtt-with-c-1f88
            try
            {

                
                string UserName = $"{Secrets.IOT_CONFIG_IOTHUB_FQDN}/{Secrets.deviceId}/api-version=2018-06-30";
                string Password = $"SharedAccessSignature sr={Secrets.IOT_CONFIG_IOTHUB_FQDN}%2Fdevices%2F{Secrets.deviceId}&sig=****&se=1592830262";

                string method = "";
                byte[] data = new byte[0];

                MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
                                        .WithClientId(Secrets.deviceId)
                                        .WithTls()
                                        .WithAuthentication(method,data)
                                        .WithTcpServer(Secrets.IOT_CONFIG_IOTHUB_FQDN, Secrets.MqttPort);
       
                // Create client options objects
                ManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
                                        .WithAutoReconnectDelay(TimeSpan.FromSeconds(60))
                                        .WithClientOptions(builder.Build())
                                        .Build();

                // Creates the client object
                _mqttClient = new MqttFactory().CreateManagedMqttClient();

                // Set up handlers
                _mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
                _mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
                _mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);

                _mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(a => {
                    Resolver.Log.Info($"Message recieved:  a.ApplicationMessage");;
                });

                // Starts a connec.tion with the Broker
                //if (mqtt_client.connect(mqtt_client_id, mqtt_username, sas_token))  From Arduino Code
                await _mqttClient.StartAsync(options);
                Resolver.Log.Info("MQTT Client Started...");

                await _mqttClient.SubscribeAsync(AZ_IOT_HUB_CLIENT_C2D_SUBSCRIBE_TOPIC);
                


                
//s_deviceClient = DeviceClient.CreateFromConnectionString(Secrets.DEVICE_CONNECTION_STRING, TransportType.Mqtt);
s_deviceClient = DeviceClient.CreateFromConnectionString(Secrets.DEVICE_CONNECTION_STRING, "DeviceId=ozz2dev", 
    TransportType.Mqtt);
                //DeviceClient.CreateFromConnectionString(Secrets.IoTHubConnectionString + $";DeviceId={ozz2dev.deviceId}", Microsoft.Azure.Devices.Client.TransportType.Amqp);*/
            } catch (Exception ex)
            {

            }
            await SendDeviceToCloudMessagesAsync();

            //projectLab.EnvironmentalSensor.StartUpdating(TimeSpan.FromSeconds(15));

            //onboardLed.SetColor(Color.Green);
        }

        public void StartWiFi()
        {
            var task = Task.Run(async () =>
            {
                var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
                wifi.NetworkConnected += NetworkConnected;
                Resolver.Log.Info("Connecting WiFi...");
                await wifi.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
            });;
        }

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");
            s_connectionString = Secrets.DEVICE_CONNECTION_STRING;
            Resolver.Log.Info("IoT Hub Quickstarts #1 - Simulated device. Ctrl-C to exit.\n");
            Resolver.Log.Info("Using Env Var IOTHUB_DEVICE_CONN_STRING = " + s_connectionString);


            StartWiFi();


            return base.Initialize();
        }

        public static void OnConnected(MqttClientConnectedEventArgs obj)
        {
            Resolver.Log.Info("Successfully connected.");
        }

        public static void OnConnectingFailed(ManagedProcessFailedEventArgs obj)
        {
            Resolver.Log.Info("Couldn't connect to broker.");
        }

        public static void OnDisconnected(MqttClientDisconnectedEventArgs obj)
        {
            _mqttClient?.Dispose();
            Resolver.Log.Info("Successfully disconnected.");
        }
    }
}