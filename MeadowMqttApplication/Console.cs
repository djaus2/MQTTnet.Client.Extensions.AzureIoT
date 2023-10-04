using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client;
using MQTTnet.Core;
using MQTTnet.Core.Client;
using MQTTnet.Core.Packets;
using MQTTnet.Core.Protocol;
using MQTTnet.Protocol;
using System;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new MqttClientTcpOptions()
            {
                Server = "myIoTHub.azure-devices.net",
                Port = 8883,
                 //AddressFamily=     
                //ClientId = "device1",
                //UserName = "myIoTHub.azure-devices.net/device1/api-version=2018-06-30",
                //Password = "SharedAccessSignature sr=myIoTHub.azure-devices.net%2Fdevices%2Fdevice1&sig=****&se=1592830262",
               // ProtocolVersion = MQTTnet.Core.Serializer.MqttProtocolVersion.V311,
                TlsOptions = new MqttClientTlsOptions() { UseTls = true }//,
                //CleanSession = true
            };

            var factory = new MqttClientFactory();
            var mqttClient = factory.CreateMqttClient();

            // handlers
            mqttClient.Connected += delegate (object sender, EventArgs e)
            {
                Console.WriteLine("Connected");
            };
            mqttClient.Disconnected += delegate (object sender, EventArgs e)
            {
                Console.WriteLine("Disconnected");
            };
            mqttClient.ApplicationMessageReceived += delegate (object sender, MqttApplicationMessageReceivedEventArgs e)
            {
                Console.WriteLine(Encoding.ASCII.GetString(e.ApplicationMessage.Payload));
            };

            mqttClient.ConnectAsync(options).Wait();

            // subscribe on the topics
            var topicFilters = new[] {
                new TopicFilter("devices/device1/messages/devicebound/#", MqttQualityOfServiceLevel.AtLeastOnce),
                new TopicFilter("$iothub/twin/PATCH/properties/desired/#", MqttQualityOfServiceLevel.AtLeastOnce),
                new TopicFilter("$iothub/methods/POST/#", MqttQualityOfServiceLevel.AtLeastOnce)
            };
            mqttClient.SubscribeAsync(topicFilters).Wait();


            // publish message 
            var topic = $"devices/device1/messages/events/$.ct=application%2Fjson&$.ce=utf-8";
            var payload = Encoding.ASCII.GetBytes("Hello IoT Hub");
            var message = new MqttApplicationMessage(topic, payload, MqttQualityOfServiceLevel.AtLeastOnce, false);
            mqttClient.PublishAsync(message);

            Console.Read();
        }
    }
}
