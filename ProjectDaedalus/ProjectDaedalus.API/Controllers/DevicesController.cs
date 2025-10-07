using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.API.Attributes;
using ProjectDaedalus.API.Dtos;
using ProjectDaedalus.API.Dtos.Device;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces; // assuming entities live in Core
using ProjectDaedalus.Infrastructure.Data; // DbContext

namespace ProjectDaedalus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly DaedalusContext _context;
        private readonly IDeviceRepository _deviceRepository;

        public DevicesController(DaedalusContext context, IDeviceRepository deviceRepository)
        {
            _context = context;
            _deviceRepository = deviceRepository;
        }
        //GET all devices for a user
        [HttpGet("user/{userId}/devices")]
        public async Task<ActionResult<IEnumerable<DeviceDto>>> GetAllDevicesOfUser(int userId)
        {
            try
            {
                var device = await _deviceRepository.GetDevicesByUserIdAsync(userId);
                if (device == null)
                {
                    return NotFound($"Devices from user {userId} not found");
                }

                // Convert to DTO
                var devices = device.Select(d => new DeviceDto
                {
                    DeviceId = d.DeviceId,
                    HardwareIdentifier = d.HardwareIdentifier,
                    ConnectionType = d.ConnectionType,
                    ConnectionAddress = d.ConnectionAddress,
                    Status = d.Status,
                    LastSeen = d.LastSeen,
                    UserId = d.UserId
                });
                return Ok(devices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //GET specific device details
        [HttpGet("{deviceId}/device")]
        public async Task<IActionResult> GetDeviceById(int deviceId)
        {
            try
            {
                var d = await _deviceRepository.GetByIdAsync(deviceId);
                if (d == null)
                {
                    return NotFound($"Device {deviceId} not found");
                }

                // Convert to DTO
                var device = new DeviceDto
                {
                    DeviceId = d.DeviceId,
                    HardwareIdentifier = d.HardwareIdentifier,
                    ConnectionType = d.ConnectionType,
                    ConnectionAddress = d.ConnectionAddress,
                    Status = d.Status,
                    LastSeen = d.LastSeen,
                    UserId = d.UserId
                };
                return Ok(device);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //POST register a new Device
        [HttpPost("internal/register")]
        [InternalApi]
        public async Task<IActionResult> RegisterDevice([FromBody] DeviceDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid Payload");
            }

            try
            {
                var exisitingDevice =
                    await _deviceRepository.GetDeviceByHardwareIdentifierAsync(dto.HardwareIdentifier);
                if (exisitingDevice != null)
                {
                    return Conflict($"Device with hardware identifer '{dto.HardwareIdentifier}' already exists");
                }

                var device = new Device
                {
                    HardwareIdentifier = dto.HardwareIdentifier,
                    ConnectionType = dto.ConnectionType,
                    ConnectionAddress = dto.ConnectionAddress,
                    UserId = dto.UserId.Value, //UserId = GetCurrentUserId(),
                    Status = "Active",
                    LastSeen = DateTime.Now
                };
                var createdDevice = await _deviceRepository.AddAsync(device);

                var resultDto = new DeviceDto
                {
                    DeviceId = createdDevice.DeviceId,
                    HardwareIdentifier = createdDevice.HardwareIdentifier,
                    ConnectionType = createdDevice.ConnectionType,
                    ConnectionAddress = createdDevice.ConnectionAddress,
                    UserId = createdDevice.UserId,
                    Status = createdDevice.Status,
                    LastSeen = createdDevice.LastSeen
                };
                return CreatedAtAction(nameof(GetDeviceById), new { deviceId = createdDevice.DeviceId }, resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //PUT update device
        [HttpPut("{deviceId}")]
        public async Task<IActionResult> UpdateDevice(int deviceId, [FromBody] DeviceDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid Payload");
            }
            
            // Find device by id
            try
            {
                var existingDevice = await _deviceRepository.GetByIdAsync(deviceId);
                if (existingDevice == null)
                {
                    return BadRequest($"Device with identifier '{deviceId}' not found");
                }

                if (existingDevice.UserId != dto.UserId.Value)
                {
                    return Forbid("You can only update your own devices");
                }

                if (!string.IsNullOrEmpty(dto.HardwareIdentifier) &&
                    dto.HardwareIdentifier != existingDevice.HardwareIdentifier)
                {
                    var duplicateDevice =
                        await _deviceRepository.GetDeviceByHardwareIdentifierAsync(dto.HardwareIdentifier);
                    if (duplicateDevice != null && duplicateDevice.DeviceId != deviceId)
                    {
                        return Conflict($"Device with hardware identifier '{dto.HardwareIdentifier}' already exists");
                    }
                }

                if (!string.IsNullOrEmpty(dto.HardwareIdentifier))
                    existingDevice.HardwareIdentifier = dto.HardwareIdentifier;

                if (!string.IsNullOrEmpty(dto.ConnectionType))
                    existingDevice.ConnectionType = dto.ConnectionType;

                if (!string.IsNullOrEmpty(dto.ConnectionAddress))
                    existingDevice.ConnectionAddress = dto.ConnectionAddress;

                // Update in database
                var updatedDevice = await _deviceRepository.UpdateAsync(existingDevice);

                // Return updated device as DTO
                var resultDto = new DeviceDto
                {
                    HardwareIdentifier = updatedDevice.HardwareIdentifier,
                    ConnectionType = updatedDevice.ConnectionType,
                    ConnectionAddress = updatedDevice.ConnectionAddress,
                    Status = updatedDevice.Status,
                    LastSeen = updatedDevice.LastSeen,
                    UserId = updatedDevice.UserId,
                };
                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //DELETE remove a device
        [HttpDelete("{deviceId}")]
        public async Task<ActionResult<Device>> DeleteDevice(int deviceId)
        {
            try
            {
                var device = await _deviceRepository.GetByIdAsync(deviceId);
                if (device == null)
                {
                    return BadRequest($"Device with identifier '{deviceId}' not found");
                }

                await _deviceRepository.DeleteAsync(deviceId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //GET check if device is online/connected
        [HttpGet("{deviceId}/status")]
        public async Task<IActionResult> IsDeviceOnline(int deviceId)
        {
            try
            {
                var device = await _deviceRepository.GetByIdAsync(deviceId);
                if (device == null)
                {
                    return NotFound($"Device with identifier '{deviceId}' not found");
                }
                
                var online =  _deviceRepository.IsDeviceOnline(deviceId);
                return Ok(online);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
