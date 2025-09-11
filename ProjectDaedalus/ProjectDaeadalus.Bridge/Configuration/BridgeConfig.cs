namespace ProjectDaeadalus.Bridge.Configuration
{
    /// <summary>
    /// Configuration settings for the Arduino Bridge
    /// Centralizes all configurable parameters for easy maintenance
    /// </summary>
    public class BridgeConfig
    {
        // Serial Communication Settings
        public string ComPort { get; set; } = "COM3"; 
        public int BaudRate { get; set; } = 9600;

        // API Connection Settings  
        public string ApiBaseUrl { get; set; } = "http://localhost:5278";  // API URL
        public string SensorEndpoint { get; set; } = "/api/sensorreadings";
        
        // Connection Management
        public int ReconnectDelayMs { get; set; } = 5000;  // Wait 5 seconds before reconnecting
        public int SerialTimeoutMs { get; set; } = 1000;   // Serial port read timeout
        
        // Data Processing
        public int MaxRetryAttempts { get; set; } = 3;     // HTTP request retries
        public int HttpTimeoutMs { get; set; } = 30000;    // HTTP request timeout
        
        /// <summary>
        /// Validates configuration settings
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrEmpty(ComPort))
                throw new ArgumentException("ComPort cannot be empty");
                
            if (BaudRate <= 0)
                throw new ArgumentException("BaudRate must be positive");
                
            if (string.IsNullOrEmpty(ApiBaseUrl))
                throw new ArgumentException("ApiBaseUrl cannot be empty");
                
            if (ReconnectDelayMs < 1000)
                throw new ArgumentException("ReconnectDelayMs should be at least 1000ms");
        }
    }
}