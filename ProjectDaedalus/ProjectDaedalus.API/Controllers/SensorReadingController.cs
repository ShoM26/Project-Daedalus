using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.API.Dtos;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces; // assuming entities live in Core
using ProjectDaedalus.Infrastructure.Data; // DbContext

namespace ProjectDaedalus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorReadingsController : ControllerBase
    {
        private readonly DaedalusContext _context;
        private readonly IUserPlantRepository _userPlantRepository;
        private readonly ISensorReadingRepository _sensorReadingRepository;

        public SensorReadingsController(DaedalusContext context, ISensorReadingRepository sensorReadingRepository, IUserPlantRepository userPlantRepository)
        {
            _context = context;
            _sensorReadingRepository = sensorReadingRepository;
            _userPlantRepository = userPlantRepository;
        }

        // POST: api/sensorreadings
        [HttpPost]
        public async Task<IActionResult> PostReading([FromBody] SensorReadingDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid payload.");
            }

            // Look up device by identifier
            var device = await _context.Devices
                .FirstAsync(d => d.HardwareIdentifier == dto.HardwareIdentifier);

            if (device == null)
            {
                return BadRequest($"Device with identifier '{dto.HardwareIdentifier}' not found.");
            }

            // Map DTO → Entity
            var reading = new SensorHistory
            {
                DeviceId = device.DeviceId,
                TimeStamp = dto.Timestamp ?? DateTime.UtcNow,
                MoistureLevel = dto.MoistureLevel
            };

            _context.SensorHistories.Add(reading);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Reading saved successfully.",
                Device = device.ConnectionAddress,
                Timestamp = reading.TimeStamp,
                MoistureLevel = reading.MoistureLevel
            });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SensorReadingDTO>>> GetSensorReadingsByPlant(int userPlantId)
        {
            try
            {
                // First get the UserPlant to find the associated device
                var userPlant = await _userPlantRepository.GetByIdAsync(userPlantId);
                if (userPlant == null)
                {
                    return NotFound($"Plant with ID {userPlantId} not found");
                }

                // Get sensor readings for the device associated with this plant
                var sensorReadings = await _sensorReadingRepository.GetReadingsByDeviceIdAsync(userPlant.DeviceId);
       
                // Convert to DTOs
                var sensorReadingDtos = sensorReadings.Select(reading => new SensorReadingDTO
                {
                    HardwareIdentifier = reading.Device.HardwareIdentifier,
                    MoistureLevel = reading.MoistureLevel,
                    Timestamp = reading.TimeStamp
                });

                return Ok(sensorReadingDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}