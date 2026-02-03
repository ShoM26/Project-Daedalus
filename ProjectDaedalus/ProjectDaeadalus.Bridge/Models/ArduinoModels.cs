namespace ProjectDaeadalus.Bridge.Models
{
    /// <summary>
    /// Represents the JSON structure received from Arduino
    /// Maps directly to the Arduino's JSON output format
    /// </summary>
    public class ArduinoMessage
    {
        public string hardwareidentifier { get; set; }
        public int? moisture { get; set; }
        public string? error { get; set; }        
        public string? secret {get; set; }
        public string type { get; set; }
        
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
        
        public bool IsValid => !string.IsNullOrEmpty(hardwareidentifier);
    }
    
    public enum ArduinoMessageType
    {
        Handshake,
        SensorReading,
        Error,
        Unknown
    }
}