namespace ProjectDaedalus.API.Dtos.Device
{
    public class AckMessage
    {
        public bool Success { get; set; }
        public required string Message { get; set; }
    }   
}