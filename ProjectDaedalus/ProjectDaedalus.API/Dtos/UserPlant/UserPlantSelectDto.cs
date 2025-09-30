using ProjectDaedalus.API.Dtos.Device;
using ProjectDaedalus.API.Dtos.Plant;

namespace ProjectDaedalus.API.Dtos.UserPlant
{
    public class UserPlantSelectDto
    {
        public int? UserPlantId { get; set; }
        public int PlantId { get; set; }
        public PlantDto? Plant { get; set; }
        public int DeviceId { get; set; }
        public DeviceDto? Device { get; set; }
    }
}