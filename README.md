# MQTTnet.Client.Extensions.AzureIoT-master

Cloned from [iotmodels/MQTTnet.Client.Extensions.AzureIoT](https://github.com/iotmodels/MQTTnet.Client.Extensions.AzureIoT)

## Added
- NetStandardLib
  - NetStandardTestConsoleApp
  - NetStandardTestMeadowApp
- MeadowMqttApplication

## About
- NetStandardLib*  
Uses the **MQTTnet.Client.Extensions.AzureIoT** lib to open a connection to an Azure IoT Hub using DeviceConnectionString and send one telemetry packet.
  - NetStandardTestConsoleApp  
Desktop (.Net Core 3.1) test app for NetStandardLib _(Works)_
  - NetStandardTestMeadowApp  
Meadow Project Lab V3 test app for NetStandardLib _(Fails)_
- MeadowMqttApplication* _(Work in progress)_  
_Working on this_
  - This directly uses MQTTnet.
  - Runs without crashing but does not connect.
  - Needs Authetication etc.
  - Can queue telemetry

*Require a Secrets class file such as:
```cs
  public static string deviceId = "";
  public static string hubName = "";
  public static string IoTHubConnectionString = "";
  public static string DEVICE_CONNECTION_STRING = "";
  public static string IOT_CONFIG_IOTHUB_FQDN = $"{hubName}.azure-devices.net";
```
