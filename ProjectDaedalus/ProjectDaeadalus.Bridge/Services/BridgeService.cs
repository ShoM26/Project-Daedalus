using System.IO.Ports;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
        private readonly HttpClient _httpClient;
        private SerialPort _serialPort;
        private bool _isRunning = true;
        private bool _disposed = false;
        private readonly IInternalApiService _internalApiService ;

        /// <summary>
        /// Initializes the bridge service with configuration and HTTP client
        /// </summary>
        public BridgeService(IInternalApiService apiService)
        {
            _internalApiService = apiService;
            _config = new BridgeConfig();
            _config.Validate(); // Ensure configuration is valid
            
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(_config.HttpTimeoutMs)
            };
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
            Console.WriteLine($"Connecting to Arduino on {_config.ComPort}...");
            
            // Display available COM ports for diagnostics
            DisplayAvailablePorts();
            
            // Initialize and configure serial port
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

                // Route message based on type
                switch (arduinoMessage.GetMessageType())
                {
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

        /// <summary>
        /// Handles error messages from Arduino
        /// </summary>
        private void HandleErrorMessage(ArduinoMessage message)
        {
            Console.WriteLine($"Arduino Error from {message.hardwareidentifier}: {message.error}");
        }

        /// <summary>
        /// Processes sensor readings and sends them to the API
        /// </summary>
        private async Task HandleSensorReadingAsync(ArduinoMessage message)
        {
            try
            {
                // Convert Arduino message to API DTO format
                var sensorReading = new SensorReadingInsertDto()
                {
                    HardwareIdentifier = message.hardwareidentifier,
                    MoistureLevel = message.moisturelevel.Value,
                    Timestamp = DateTime.UtcNow,
                };

                await SendToApiAsync(sensorReading);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing sensor reading: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Register device when it is its first time logging in
        /// </summary>
        /// TODO:Make sure to write this and be careful about what data you're allowing into the database
        public async Task RegisterNewDevice(string hardwareIdentifier, int userId)
        {
            var registerDto = new DeviceDto
            {
                
                HardwareIdentifier = hardwareIdentifier,
                ConnectionType = "",
                ConnectionAddress = "",
                UserId = userId
            };

            await _internalApiService.PostAsync<object>("devices/internal/register", registerDto);
        }

        /// <summary>
        /// Sends sensor data to the API with retry logic
        /// </summary>
        private async Task SendToApiAsync(SensorReadingInsertDto sensorReading)
        {
            var apiDto = new SensorReadingInsertDto
            {
                HardwareIdentifier = sensorReading.HardwareIdentifier,
                MoistureLevel = sensorReading.MoistureLevel,
                Timestamp = sensorReading.Timestamp
            };
            await _internalApiService.PostAsync<object>("sensorreadings/internal", apiDto);
        }
        //TODO: Is it a good idea to run other internal api calls here?

        #region Helper Methods

        /// <summary>
        /// Displays current configuration settings
        /// </summary>
        private void DisplayConfiguration()
        {
            Console.WriteLine("Configuration:");
            Console.WriteLine($"COM Port: {_config.ComPort}");
            Console.WriteLine($"Baud Rate: {_config.BaudRate}");
            Console.WriteLine($"API URL: {_config.ApiBaseUrl}{_config.SensorEndpoint}");
            Console.WriteLine($"Reconnect Delay: {_config.ReconnectDelayMs}ms");
            Console.WriteLine();
        }

        /// <summary>
        /// Shows available COM ports for debugging
        /// </summary>
        private void DisplayAvailablePorts()
        {
            string[] availablePorts = SerialPort.GetPortNames();
            Console.WriteLine($"Available COM ports: {string.Join(", ", availablePorts)}");
        }

        /// <summary>
        /// Configures the serial port with proper settings
        /// </summary>
        private void InitializeSerialPort()
        {
            _serialPort = new SerialPort(_config.ComPort, _config.BaudRate)
            {
                ReadTimeout = _config.SerialTimeoutMs,
                NewLine = "\n" // Arduino uses \n for line endings
            };
        }

        /// <summary>
        /// Sets up graceful shutdown handling for Ctrl+C
        /// </summary>
        private void SetupGracefulShutdown()
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                _isRunning = false;
                Console.WriteLine("\nShutdown requested... Cleaning up...");
            };
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Properly disposes of resources
        /// </summary>
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

                    _httpClient?.Dispose();
                }
                _disposed = true;
            }
        }

        #endregion
    }
}