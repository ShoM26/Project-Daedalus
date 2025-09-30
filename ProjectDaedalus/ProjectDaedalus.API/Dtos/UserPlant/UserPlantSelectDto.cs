using ProjectDaedalus.API.Dtos.Plant;

namespace ProjectDaedalus.API.Dtos.UserPlant
{
    public class UserPlantSelectDto
    {
        public int? UserPlantId { get; set; }
        public int PlantId { get; set; }
        public PlantDto Plant { get; set; }
    }
}