namespace ProjectDaedalus.Bridge.Models
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
        public string status { get; set; }
        public string message { get; set; }      // For startup/status messages
        public string error { get; set; }        // For error messages
        
        /// <summary>
        /// Determines the type of message received from Arduino
        /// </summary>
        public ArduinoMessageType GetMessageType()
        {
            if (!string.IsNullOrEmpty(error))
                return ArduinoMessageType.Error;
            
            if (!string.IsNullOrEmpty(message))
                return ArduinoMessageType.Status;
                
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
    /// DTO that matches your API's expected format for sensor readings
    /// This is what gets sent to your ASP.NET Core API
    /// </summary>
    public class CreateSensorReadingDto
    {
        public string HardwareIdentifier { get; set; }
        public int MoistureLevel { get; set; }
        public DateTime TimeStamp { get; set; }
        
        /// <summary>
        /// Additional metadata that could be useful for your API
        /// </summary>
        public int? RawValue { get; set; }          // Original sensor reading
        public string Source { get; set; } = "Arduino";  // Data source identifier
    }
    
    /// <summary>
    /// Enumeration of different Arduino message types
    /// Helps with message processing and routing
    /// </summary>
    public enum ArduinoMessageType
    {
        Unknown,
        SensorReading,
        Status,
        Error
    }
}