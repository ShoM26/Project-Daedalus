using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.API.Dtos;
using ProjectDaedalus.Core.Entities;   // assuming entities live in Core
using ProjectDaedalus.Infrastructure.Data; // DbContext

namespace ProjectDaedalus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorReadingsController : ControllerBase
    {
        private readonly DaedalusContext _context;

        public SensorReadingsController(DaedalusContext context)
        {
            _context = context;
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
                .FirstOrDefaultAsync(d => d.ConnectionAddress == dto.DeviceIdentifier);

            if (device == null)
            {
                return BadRequest($"Device with identifier '{dto.DeviceIdentifier}' not found.");
            }

            // Map DTO → Entity
            var reading = new SensorHistory
            {
                DeviceId = device.DeviceId, // Not PK
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
    }
}