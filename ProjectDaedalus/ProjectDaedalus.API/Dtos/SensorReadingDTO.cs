namespace ProjectDaedalus.API.Dtos
{
    public class SensorReadingDTO
    {
        // Unique identifier
        public required string HardwareIdentifier { get; set; }

        // When the reading was taken
        public DateTime? Timestamp { get; set; }

        // Soil moisture value (raw)
        public decimal MoistureLevel { get; set; }
    }
}

