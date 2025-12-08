namespace ProjectDaedalus.API.Dtos.Device
{
    public class RegisterDeviceDto
    {
        public required string HardwareIdentifier { get; set; }
        public string ConnectionType { get; set; } = string.Empty;
        public string ConnectionAddress { get; set; } = string.Empty;
        public required string UserToken  { get; set; }
    }
}