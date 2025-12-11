namespace ProjectDaeadalus.Bridge.Models
{
    /// <summary>
    /// Represents the JSON structure received from Arduino
    /// Maps directly to the Arduino's JSON output format
    /// </summary>
    public class ArduinoMessage
    {
        public string hardwareidentifier { get; set; }
        public long timestamp { get; set; }
        public int? moisturelevel { get; set; }
        public string? error { get; set; }        // For error messages
        public string? secret {get; set; }
        public string type { get; set; }
        
        /// <summary>
        /// Determines the type of message received from Arduino
        /// </summary>
        public ArduinoMessageType GetMessageType()
        {
            return type switch
            {
                "HANDSHAKE" => ArduinoMessageType.Handshake,
                "DATA" => ArduinoMessageType.SensorReading,
                "ERROR" => ArduinoMessageType.Error, 
                _ => ArduinoMessageType.Unknown,
            };
        }
        
        /// <summary>
        /// Validates that the Arduino message has required fields
        /// </summary>
        public bool IsValid => !string.IsNullOrEmpty(hardwareidentifier);
    }
    
    
    /// <summary>
    /// Enumeration of different Arduino message types
    /// Helps with message processing and routing
    /// </summary>
    public enum ArduinoMessageType
    {
        Handshake,
        SensorReading,
        Error,
        Unknown
    }
}