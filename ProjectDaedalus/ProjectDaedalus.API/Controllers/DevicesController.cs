using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using ProjectDaedalus.API.Attributes;
using ProjectDaedalus.API.Dtos;
using ProjectDaedalus.API.Dtos.Device;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Data;

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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceDto config)
        {
            Console.WriteLine("Made it into the POST api call");
            if (config.UserToken == null)
            {
                return BadRequest("Invalid token");
            }
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;

                // 2. If null, try the Microsoft specific name
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                }

                // 3. If STILL null, the token is valid but has no ID inside.
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Token is valid, but contains no User ID.");
                }

                // 4. Convert to Int (assuming your DB uses Integers)
                int userId = int.Parse(userIdClaim);
                
                var device = new Device
                {
                    HardwareIdentifier = config.HardwareIdentifier,
                    ConnectionType = config.ConnectionType,
                    ConnectionAddress = config.ConnectionAddress,
                    UserId = userId,
                    Status = "Active",
                    LastSeen = DateTime.Now
                };
                Console.WriteLine("Device created");
                var createdDevice = await _deviceRepository.AddAsync(device);
                Console.WriteLine($"Added the device {createdDevice} to the database, sending ack message");
                return Ok(new AckMessage
                {
                    Success = true,
                    Message = new { type = "ACK"}
                });
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
        [Authorize]
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
                var d = await _deviceRepository.GetByIdAsync(deviceId);
                if (d == null)
                {
                    return NotFound($"Device {deviceId} not found");
                }

                // Convert to DTO
                var device = new DeviceDto
                {
                    HardwareIdentifier = d.HardwareIdentifier,
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
        //GET check if a device with given hardwareidentifier already exists
        [HttpGet("{hardwareIdentifier}")]
        public async Task<IActionResult> GetDeviceByHardwareIdentifier(string hardwareIdentifier)
        {
            try
            {
                var exists = await _deviceRepository.GetDeviceByHardwareIdentifierAsync(hardwareIdentifier);
                if (exists == null)
                {
                    return NoContent();
                }

                return Ok(exists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateDevice([FromBody] HandshakeDto dto)
        {
            if (dto == null)
                {
                    return BadRequest("Request body is null");
                }
            Console.WriteLine($"API reicieved dto with identifier {dto.HardwareIdentifier}");
                if (string.IsNullOrEmpty(dto.HardwareIdentifier))
                {
                    return BadRequest("HardwareIdentifier is required");
                }
           
            // Find device by id
            try
            {
                var existingDevice = await _deviceRepository.GetDeviceByHardwareIdentifierAsync(dto.HardwareIdentifier);
                Console.WriteLine($"Device found in api call");
                if (existingDevice == null)
                {
                    return NoContent();
                }

                existingDevice.LastSeen = DateTime.Now;
                // Update in database
                Console.WriteLine("Updated time saving changes now");
                await _deviceRepository.SaveChangesAsync();
                return Ok(new AckMessage
                {
                    Success = true,
                    Message = new { type = "ACK"}
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
