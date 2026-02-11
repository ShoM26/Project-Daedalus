using System.IO.Ports;
using System.Net;
using System.Text.Json;
using ProjectDaeadalus.Bridge.Configuration;
using ProjectDaeadalus.Bridge.Models;
using ProjectDaedalus.API.Dtos.Device;
using ProjectDaedalus.API.Dtos.SensorReading;

namespace ProjectDaeadalus.Bridge.Services
{
    /// <summary>
    /// Main service class that handles Arduino communication and API integration
    /// Demonstrates separation of concerns and proper resource management
    /// </summary>
    public class BridgeService : IDisposable
    {
        private readonly BridgeConfig _config;
        private readonly string _expectedSecret;
        private SerialPort _serialPort;
        private bool _isRunning = true;
        private bool _disposed = false;
        private readonly IInternalApiService _internalApiService;

        /// <summary>
        /// Initializes the bridge service with configuration and HTTP client
        /// </summary>
        public BridgeService(IInternalApiService apiService, IConfiguration appSettings, BridgeConfig config)
        {
            _internalApiService = apiService;
            _config = config;
            _config.Validate();
            _expectedSecret = appSettings["HardwareSettings:HardwareKey"];
            if (string.IsNullOrEmpty(_expectedSecret))
            {
                Console.WriteLine("[Warning] Hardware Secret us missing in appsettings.json");
            }
            
        }

        /// <summary>
        /// Main entry point for running the bridge service
        /// Handles the complete lifecycle with graceful shutdown
        /// </summary>
        public async Task RunAsync()
        {
            DisplayConfiguration();
            SetupGracefulShutdown();

            // Main connection loop with auto-reconnect capability
            while (_isRunning)
            {
                try
                {
                    await ConnectAndListenAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection error: {ex.Message}");
                    
                    if (_isRunning)
                    {
                        Console.WriteLine($"Retrying in {_config.ReconnectDelayMs / 1000} seconds...");
                        await Task.Delay(_config.ReconnectDelayMs);
                    }
                }
            }

            Console.WriteLine("Bridge service stopped successfully.");
        }

        /// <summary>
        /// Establishes connection to Arduino and processes incoming data
        /// </summary>
        private async Task ConnectAndListenAsync()
        {
            _config.ComPort = AutoDetectPort();
            InitializeSerialPort();
            
            try
            {
                _serialPort.Open();
                Console.WriteLine("Connected! Listening for Arduino data...\n");
            }
            catch (UnauthorizedAccessException)
            {
                throw new Exception($"Access denied to {_config.ComPort}. Is another program using it?");
            }
            catch (ArgumentException)
            {
                throw new Exception($"Invalid COM port: {_config.ComPort}. Check your Arduino connection.");
            }
            
            // Main data processing loop
            await ProcessArduinoDataAsync();
        }
        
        private string CleanJsonString(string rawJson)
        {
            if (string.IsNullOrEmpty(rawJson))
                return rawJson;

            return rawJson
                .Replace("\0", "")      // Remove null bytes
                .Replace("\x00", "")    // Remove null bytes (hex format)
                .Trim();                // Remove leading/trailing whitespace
        }

        /// <summary>
        /// Processes incoming data from Arduino in a continuous loop
        /// </summary>
        private async Task ProcessArduinoDataAsync()
        {
            while (_isRunning && _serialPort.IsOpen)
            {
                try
                {
                    string jsonLine = CleanJsonString(_serialPort.ReadLine());
                    
                    if (!string.IsNullOrEmpty(jsonLine))
                    {
                        Console.WriteLine($"Received: {jsonLine}");
                        await HandleArduinoMessageAsync(jsonLine);
                    }
                }
                catch (TimeoutException)
                {
                    // Normal timeout - continue listening
                    continue;
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Serial port was closed");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Read error: {ex.Message}");
                    break;
                }
            }
        }

        /// <summary>
        /// Parses and routes Arduino messages based on their type
        /// </summary>
        private async Task HandleArduinoMessageAsync(string jsonLine)
        {
            try
            {
                var arduinoMessage = JsonSerializer.Deserialize<ArduinoMessage>(jsonLine, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (!arduinoMessage.IsValid)
                {
                    Console.WriteLine("Warning: Received invalid message (missing device_id)");
                    return;
                }
                
                switch (arduinoMessage.GetMessageType())
                {
                    case ArduinoMessageType.Handshake:
                        await HandleDeviceHandshake(arduinoMessage);
                        break;
                    case ArduinoMessageType.Error:
                        HandleErrorMessage(arduinoMessage);
                        break;
                    
                    case ArduinoMessageType.SensorReading:
                        await HandleSensorReadingAsync(arduinoMessage);
                        break;
                    
                    case ArduinoMessageType.Unknown:
                        Console.WriteLine($"Unknown message format from {arduinoMessage.hardwareidentifier}");
                        break;
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON parsing error: {ex.Message}");
                Console.WriteLine($"Raw data: {jsonLine}");
            }
        }
        
        private void HandleErrorMessage(ArduinoMessage message)
        {
            Console.WriteLine($"Arduino Error from {message.hardwareidentifier}: {message.error}");
        }
        
        private async Task HandleSensorReadingAsync(ArduinoMessage message)
        {
            try
            {
                var sensorReading = new SensorReadingInsertDto()
                {
                    HardwareIdentifier = message.hardwareidentifier,
                    MoistureLevel = message.moisture.Value,
                    Timestamp = DateTime.Now,
                };

                await SendDataToApiAsync(sensorReading);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing sensor reading: {ex.Message}");
            }
        }

        private async Task HandleDeviceHandshake(ArduinoMessage message)
        {
            try
            {
                var handshake = new HandshakeDto
                {
                    HardwareIdentifier = message.hardwareidentifier,
                    HardwareSecret = message.secret
                };
                
                if (handshake.HardwareSecret == _expectedSecret)
                {
                    //Call separate methods for update/register
                    // waits for you to press the button
                    var response = await _internalApiService.CheckDeviceStatusAsync($"Devices/{message.hardwareidentifier}");
                    if (response == HttpStatusCode.NoContent){
                        Console.WriteLine("Awaiting the call to register the new device");
                        await RegisterNewDevice(handshake.HardwareIdentifier);
                    }
                    if(response == HttpStatusCode.OK)
                    {
                        await UpdateExistingDevice(handshake);
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect hardware secret");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing device handshake: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Register device when it is its first time logging in
        /// </summary>
        private async Task RegisterNewDevice(string hardwareIdentifier)
        {
            //Grab token
            string userToken;
            try
            {
                if (_config.UserToken.Length == 0 || _config.UserToken == null)
                {
                    Console.WriteLine("User Token is not populated, Retrying in 5 seconds");
                    await Task.Delay(500);
                    await RegisterNewDevice(hardwareIdentifier);
                }
                userToken = _config.UserToken;
                Console.WriteLine("Token is not empty or null");
                var registerDto = new RegisterDeviceDto
                {
                    HardwareIdentifier = hardwareIdentifier,
                    UserToken = userToken,
                    ConnectionAddress = _config.ComPort,
                    ConnectionType = "USB"
                };
                Console.WriteLine("Sending the dto off to the api call");
                var response =
                    await _internalApiService.FirstRegisterDeviceAsync<AckMessage>(registerDto, userToken);
                if (response != null && response.Success)
                {
                    SendViaSerial(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering device: {ex.Message}");
            }
        }

        public async Task UpdateExistingDevice(HandshakeDto dto)
        {
            
            var response = await _internalApiService.PutAsync<AckMessage>($"Devices/update", dto);
            if (response != null && response.Success)
            {
                SendViaSerial(response.Message);
            }
        }
        
        private async Task SendDataToApiAsync(SensorReadingInsertDto sensorReading)
        {
            var apiDto = new SensorReadingInsertDto
            {
                HardwareIdentifier = sensorReading.HardwareIdentifier,
                MoistureLevel = sensorReading.MoistureLevel,
                Timestamp = sensorReading.Timestamp
            };
            await _internalApiService.PostAsync<object>("sensorreadings/internal", apiDto);
        }
        #region Helper Methods
        
        private void DisplayConfiguration()
        {
            Console.WriteLine("Configuration:");
            Console.WriteLine($"COM Port: {_config.ComPort}");
            Console.WriteLine($"Baud Rate: {_config.BaudRate}");
            Console.WriteLine($"API URL: {_config.ApiBaseUrl}{_config.SensorEndpoint}");
            Console.WriteLine($"Reconnect Delay: {_config.ReconnectDelayMs}ms");
            Console.WriteLine();
        }
        
        private static string AutoDetectPort()
        {
            string[] availablePorts = SerialPort.GetPortNames();
            Console.WriteLine($"Available COM ports: {string.Join(", ", availablePorts)}");
            while (true)
            {
                if (availablePorts.Length == 0)
                {
                    Console.WriteLine("No COM ports available");
                    Thread.Sleep(3000);
                    continue;
                }
                foreach (var portName in availablePorts)
                {
                    try
                    {
                        using (SerialPort port = new SerialPort(portName, 9600))
                        {
                            port.ReadTimeout = 15000;
                            port.Open();

                            for (int i = 0; i < 3; i++)
                            {
                                try
                                {
                                    string message = port.ReadLine();
                                    if (!string.IsNullOrEmpty(message) && message.Contains("HANDSHAKE"))
                                    {
                                        Console.WriteLine($"Found port: {message}");
                                        return portName;
                                    }
                                }
                                catch (TimeoutException)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        Console.WriteLine($"{portName} is a ghost port");
                    }
                    catch (IOException)
                    {
                        Console.WriteLine($"{portName} IO error");
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine($"{portName} Access error");
                    }
                }
                Console.WriteLine("Device not found yet");
                Thread.Sleep(3000);
            }
        }
        
        
        private void InitializeSerialPort()
        {
            _serialPort = new SerialPort(_config.ComPort, _config.BaudRate)
            {
                ReadTimeout = _config.SerialTimeoutMs,
                NewLine = "\n" // Arduino uses \n for line endings
            };
        }
        
        private void SetupGracefulShutdown()
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                _isRunning = false;
                Console.WriteLine("\nShutdown requested... Cleaning up...");
            };
        }

        private void SendViaSerial(object jsonMessage)
        {
            if (!_serialPort.IsOpen) return;
            var json = JsonSerializer.Serialize(jsonMessage);
            _serialPort.WriteLine(json);
            Console.WriteLine($"Sent ACK message {json}");
        }

        /*
        public void SendViaBluetooth(string jsonMessage)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(jsonMessage + "\n");
            _bluetoothStream.Write(data, 0, data.Length);
        }*/

        #endregion
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected dispose method for proper resource cleanup
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        if (_serialPort?.IsOpen == true)
                        {
                            _serialPort.Close();
                        }
                        _serialPort?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Cleanup warning: {ex.Message}");
                    }
                }
                _disposed = true;
            }
        }
    }
}