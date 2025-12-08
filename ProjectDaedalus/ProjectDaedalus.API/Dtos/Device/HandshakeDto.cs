namespace ProjectDaedalus.API.Dtos.Device
{ 
    public class HandshakeDto
    {
        public required string HardwareIdentifier { get; set; }
        public required string HardwareSecret { get; set; }
    }   
}