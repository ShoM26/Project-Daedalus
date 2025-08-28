namespace ProjectDaedalus.API.Dtos
{
    public class SensorReadingDTO
    {
        // Unique identifier for the device (MAC, UUID, or assigned string)
        public string DeviceIdentifier { get; set; } = string.Empty;

        // When the reading was taken
        public DateTime? Timestamp { get; set; }

        // Soil moisture value (raw)
        public decimal MoistureLevel { get; set; }
    }
}

