namespace ProjectDaedalus.API.Dtos.Device
{
    public class DeviceDto
    {
        public int DeviceId { get; set; }
        public string HardwareIdentifier { get; set; } = string.Empty;
        public string ConnectionType { get; set; } = string.Empty;
        public string ConnectionAddress { get; set; } = string.Empty;
        public string? Status { get; set; } = string.Empty;
        public DateTime? LastSeen { get; set; }
        public int? UserId { get; set; }
        public bool IsOnline => Status == "Active" && LastSeen > DateTime.UtcNow.AddMinutes(-5);
    }
}

