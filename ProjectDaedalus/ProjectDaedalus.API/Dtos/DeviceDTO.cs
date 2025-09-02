namespace ProjectDaedalus.API.Dtos
{
    public class DeviceDTO
    {
        public int? DeviceId { get; set; } // Nullable for POST requests
        public string HardwareIdentifier { get; set; } = string.Empty;
        public string ConnectionType { get; set; } = string.Empty;
        public string ConnectionAddress { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? LastSeen { get; set; }
        public int? UserId { get; set; } // Nullable for flexibility
    
        // Computed property for status checking
        public bool IsOnline => Status == "Active" && LastSeen > DateTime.UtcNow.AddMinutes(-5);
    }
}

