using Microsoft.AspNetCore.Mvc;
using ProjectDaedalus.API.Dtos.SensorReading;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;

namespace ProjectDaedalus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorReadingsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISensorReadingRepository _sensorReadingRepository;
        private readonly IDeviceRepository _deviceRepository;

        public SensorReadingsController(IUnitOfWork unitOfWork, ISensorReadingRepository sensorReadingRepository, 
            IDeviceRepository deviceRepository)
        {
            _unitOfWork = unitOfWork;
            _sensorReadingRepository = sensorReadingRepository;
            _deviceRepository = deviceRepository;
        }

        // POST: api/sensorreadings
        [HttpPost("internal")]
        //[InternalApi]
        public async Task<IActionResult> CreateFromBridge([FromBody] SensorReadingInsertDto dto)
        {
            Console.WriteLine("=== API Method Hit ===");
            Console.WriteLine($"ModelState Valid: {ModelState.IsValid}");
    
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Field: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                return BadRequest(ModelState);
            }
    
            Console.WriteLine($"DTO received: DeviceId={dto.HardwareIdentifier}, MoistureValue={dto?.MoistureLevel}");

            if (dto == null)
            {
                return BadRequest("Invalid payload.");
            }
            var device = await _deviceRepository.GetDeviceByHardwareIdentifierAsync(dto.HardwareIdentifier);

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
            
            await _sensorReadingRepository.AddAsync(reading);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new
            {
                Message = "Reading saved successfully.",
                Device = device.ConnectionAddress,
                Timestamp = reading.TimeStamp,
                MoistureLevel = reading.MoistureLevel
            });
        }

        [HttpGet("device/{deviceId}/readings")]
        public async Task<ActionResult<IEnumerable<SensorReadingSelectDto>>> GetAllReadingsByDeviceIdAsync(int deviceId)
        {
            try
            {
                var readings = await _sensorReadingRepository.GetReadingsByDeviceIdAsync(deviceId);
                if (!readings.Any())
                {
                    return NotFound($"Device with ID {deviceId} not found");
                }
                
                var sensorReadingDtos = readings.Select(reading => new SensorReadingSelectDto
                {
                    DeviceId = reading.DeviceId,
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
        //Get the latest reading of a device
        [HttpGet("device/{deviceId}/reading")]
        public async Task<IActionResult> GetLastReadingByDevice(int deviceId)
        {
            try
            {
                var reading = await _sensorReadingRepository.GetLatestReadingByDeviceIdAsync(deviceId);
                if (reading == null)
                {
                    return NoContent(); //204 handled by front end
                }
                
                return Ok(reading);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("device/{deviceId}/range")]
        public async Task<ActionResult<IEnumerable<SensorReadingSelectDto>>> GetReadingRangeByDevice(int deviceId,
            [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest("Start date cannot be after end date");
                }
                var readings = await _sensorReadingRepository.GetReadingsForDeviceByRangeAsync(deviceId, startDate, endDate);
                if (!readings.Any())
                {
                    return NoContent(); //204 handled by front end
                }
                var readingDtos = readings.Select(r => new SensorReadingSelectDto
                {
                    DeviceId = r.DeviceId,
                    MoistureLevel = r.MoistureLevel,
                    Timestamp = r.TimeStamp
                });
                return Ok(readingDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //DELETE within a certain time frame
        [HttpDelete]
        public async Task<IActionResult> DeleteReadingsAfterDate([FromQuery] DateTime cutoff)
        {
            try
            {
                if (cutoff > DateTime.UtcNow)
                {
                    BadRequest("Invalid cutoff");
                }

                var count = await _sensorReadingRepository.DeleteOldReadingsAsync(cutoff);
                return Ok(count);
            }
            catch
            {
                return StatusCode(500, "An error occurred while deleting");
            }
        }
        //DELETE all by deviceid
        [HttpDelete("{deviceId}")]
        public async Task<IActionResult> DeleteAllReadingsByDevice(int deviceId)
        {
            try
            {
                var readings = await _sensorReadingRepository.GetReadingsByDeviceIdAsync(deviceId);
                if (!readings.Any())
                {
                    return NoContent(); //204 handled by front end
                }

                await _sensorReadingRepository.DeleteManyAsync(readings);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}