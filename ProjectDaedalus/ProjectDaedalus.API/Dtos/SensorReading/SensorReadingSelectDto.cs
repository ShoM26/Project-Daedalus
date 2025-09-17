namespace ProjectDaedalus.API.Dtos.SensorReading
{
    public class SensorReadingSelectDto
    {
        public int DeviceId { get; set; }

        // When the reading was taken
        public DateTime? Timestamp { get; set; }

        // Soil moisture value (raw)
        public decimal MoistureLevel { get; set; }
    }
}