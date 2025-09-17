namespace ProjectDaedalus.API.Dtos.UserPlant
{
    public class UserPlantCreateDto
    {
        public int? UserPlantId { get; set; }
        public int UserId { get; set; }
        public int PlantId { get; set; }
        public int DeviceId { get; set; }
        public DateTime? DateAdded { get; set; }
    }
}
