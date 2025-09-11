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
        public int? moisture_raw { get; set; }
        public string? error { get; set; }        // For error messages
        
        /// <summary>
        /// Determines the type of message received from Arduino
        /// </summary>
        public ArduinoMessageType GetMessageType()
        {
            if (!string.IsNullOrEmpty(error))
                return ArduinoMessageType.Error;
                
            if (moisture_raw.HasValue)
                return ArduinoMessageType.SensorReading;
                
            return ArduinoMessageType.Unknown;
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
        Unknown,
        SensorReading,
        Error
    }
}