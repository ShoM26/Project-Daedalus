using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.API.Dtos.Device;
using ProjectDaedalus.API.Dtos.Plant;
using ProjectDaedalus.API.Dtos.UserPlant;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces; // assuming entities live in Core
using ProjectDaedalus.Infrastructure.Data; // DbContext

namespace ProjectDaedalus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserPlantsController : ControllerBase
    {
        private readonly DaedalusContext _context;
        private readonly IUserPlantRepository _userPlantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserPlantsController(DaedalusContext context, IUserPlantRepository userPlantRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _userPlantRepository = userPlantRepository;
        }
        // GET all user's plants
        [HttpGet("user/{userId}/plants")]
        public async Task<ActionResult<IEnumerable<UserPlant>>> GetUserPlants(int userId)
        {
            try
            {
                var plants = await _userPlantRepository.GetUserPlantsByUserIdAsync(userId);
                if (!plants.Any())
                    return NotFound($"No plants found for user {userId}.");
    
                return Ok(plants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving user plants.");
            }
        }
        // GET specific user's plant by UserPlant ID
        [HttpGet("{userPlantId}")]
        public async Task<ActionResult<UserPlantCreateDto>> GetUserPlant(int userPlantId)
        {
            try
            {
                var userPlant = await _userPlantRepository.GetByIdAsync(userPlantId);
                if (userPlant == null)
                {
                    return NotFound($"User plant with ID {userPlantId} not found.");
                }
                var userPlantDto = new UserPlantCreateDto
                {
                    UserPlantId = userPlant.UserPlantId,
                    UserId = userPlant.UserId,
                    PlantId = userPlant.PlantId,
                    DeviceId = userPlant.DeviceId,
                };
                return Ok(userPlantDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the user plant.");
            }
        }
        // GET specific user's plant by device ID
        [HttpGet("device/{deviceId}/plant")]
        public async Task<ActionResult<UserPlantCreateDto>> GetUserPlantByDevice(int deviceId)
        {
            try
            {
                var userPlant = await _userPlantRepository.GetUserPlantByDeviceIdAsync(deviceId);
                if (userPlant == null)
                {
                    return NotFound($"No plant assignment found for device {deviceId}.");
                }
                var userPlantDto = new UserPlantCreateDto
                {
                    UserPlantId = userPlant.UserPlantId,
                    UserId = userPlant.UserId,
                    PlantId = userPlant.PlantId,
                    DeviceId = userPlant.DeviceId,
                };
                return Ok(userPlantDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the user plant by device.");
            }
        }

        // POST assign a new plant to a user with device
        [HttpPost]
        public async Task<ActionResult<UserPlantCreateDto>> CreateUserPlant(UserPlantCreateDto dto)
        {
            try
            {
                // Validate that the combination doesn't already exist
                var exists = await _userPlantRepository.UserPlantExistsAsync(
                    dto.UserId,
                    dto.PlantId,
                    dto.DeviceId);
                if (exists)
                {
                    return BadRequest("This plant-device assignment already exists for the user.");
                }
                var userPlant = new UserPlant
                {
                    UserId = dto.UserId,
                    PlantId = dto.PlantId,
                    DeviceId = dto.DeviceId,
                    DateAdded = DateTime.Now
                };

                var createdUserPlant = await _userPlantRepository.AddAsync(userPlant);
                var userPlantDto = new UserPlantCreateDto
                {
                    UserPlantId = createdUserPlant.UserPlantId,
                    UserId = createdUserPlant.UserId,
                    PlantId = createdUserPlant.PlantId,
                    DeviceId = createdUserPlant.DeviceId,
                    DateAdded = createdUserPlant.DateAdded
                };
                return CreatedAtAction(nameof(GetUserPlant),
                    new { userPlantId = userPlantDto.UserPlantId }, userPlantDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the user plant assignment.");
            }
        }
        // PUT update device assignment
        [HttpPut("device/{deviceId}")]
        public async Task<ActionResult> UpdateDeviceAssignment(int deviceId, UserPlantCreateDto dto)
        {
            try
            {
                var existingUserPlant = await _userPlantRepository.GetUserPlantByDeviceIdAsync(deviceId);

                if (existingUserPlant == null)
                {
                    return NotFound($"No pairing with device Id {deviceId} found.");
                }

                // Check if the new device assignment would create a duplicate
                var wouldDuplicate = await _userPlantRepository.UserPlantExistsAsync(
                    dto.UserId,
                    dto.PlantId,
                    dto.DeviceId);
                if (wouldDuplicate)
                {
                    return BadRequest("This device is already assigned to this plant for this user.");
                }
                // Update the device assignment
                existingUserPlant.DeviceId = dto.DeviceId;
                existingUserPlant.UserId = dto.UserId;
                existingUserPlant.PlantId = dto.PlantId;
                existingUserPlant.DateAdded = DateTime.Now;
                await _userPlantRepository.UpdateAsync(existingUserPlant);
                await _unitOfWork.SaveChangesAsync();
                return Ok(existingUserPlant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the device assignment.");
            }
        }
        // DELETE remove plant assignment from user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserPlant(int id)
        {
            try
            {
                var userPlant = await _userPlantRepository.GetByIdAsync(id);
                if (userPlant == null)
                {
                    return NotFound($"User plant with ID {id} not found.");
                }
                await _userPlantRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the user plant assignment.");
            }
        }
        //GET all plants belonging to user
        [HttpGet("{userId}/plants")]
        public async Task<ActionResult<IEnumerable<UserPlantSelectDto>>> GetPlantsBelongingToUser(int userId)
        {
            try
            {
                var userPlants = await _userPlantRepository.GetUserPlantsByUserIdAsync(userId);
                if (!userPlants.Any())
                {
                    return NotFound($"User with id {userId} not found");
                }
                    
                var plantsDto = userPlants.Select(up => new UserPlantSelectDto
                {
                    UserPlantId = up.UserPlantId,
                    PlantId = up.PlantId,
                    DeviceId = up.DeviceId,
                    Plant = new PlantDto
                    {
                        FamiliarName = up.Plant.FamiliarName,
                        ScientificName = up.Plant.ScientificName,
                        MoistureLowRange = up.Plant.MoistureLowRange,
                        MoistureHighRange = up.Plant.MoistureHighRange,
                        FunFact = up.Plant.FunFact
                    },
                    Device = new DeviceDto
                    {
                        DeviceId = up.Device.DeviceId,
                        HardwareIdentifier = up.Device.HardwareIdentifier,
                        ConnectionType = up.Device.ConnectionType,
                        ConnectionAddress = up.Device.ConnectionAddress
                    }
                }).ToList();
                return Ok(plantsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //Get the device associated with a particular userplantid
        [HttpGet("{userplantId}/device")]
        public async Task<ActionResult<UserPlantSelectDto>> GetDeviceOfUserplant(int userplantId)
        {
            try
            {
                var userPlant = await  _userPlantRepository.GetByIdWithDeviceAsync(userplantId);
                if (userPlant == null)
                {
                    return NotFound($"Userplant with ID {userplantId} not found.");
                }

                var deviceDto = new UserPlantSelectDto
                {
                    UserPlantId = userPlant.UserPlantId,
                    DeviceId = userPlant.DeviceId,
                    Device = new DeviceDto
                    {
                        DeviceId = userPlant.Device.DeviceId,
                        HardwareIdentifier = userPlant.Device.HardwareIdentifier,
                        ConnectionType = userPlant.Device.ConnectionType,
                        ConnectionAddress = userPlant.Device.ConnectionAddress
                    }
                };
                return Ok(deviceDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
    }
}
