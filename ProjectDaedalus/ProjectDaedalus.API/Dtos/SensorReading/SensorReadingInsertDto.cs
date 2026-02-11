namespace ProjectDaedalus.API.Dtos.SensorReading
{
    public class SensorReadingInsertDto
    {
        public required string HardwareIdentifier { get; set; }

        public DateTime? Timestamp { get; set; }

        public decimal MoistureLevel { get; set; }
    }
}

