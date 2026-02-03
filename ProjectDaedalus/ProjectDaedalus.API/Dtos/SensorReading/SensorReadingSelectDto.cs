namespace ProjectDaedalus.API.Dtos.SensorReading
{
    public class SensorReadingSelectDto
    {
        public int DeviceId { get; set; }

        public DateTime? Timestamp { get; set; }

        public decimal MoistureLevel { get; set; }
    }
}