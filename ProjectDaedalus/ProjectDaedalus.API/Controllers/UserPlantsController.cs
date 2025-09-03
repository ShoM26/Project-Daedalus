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
    public class UserPlantsController : ControllerBase
    {
        private readonly DaedalusContext _context;
        private readonly IUserPlantRepository _userPlantRepository;

        public UserPlantsController(DaedalusContext context, IUserPlantRepository userPlantRepository)
        {
            _context = context;
            _userPlantRepository = userPlantRepository;
        }
        // GET all user's plants
        [HttpGet("user/{userId}/plants")]
        public async Task<ActionResult<IEnumerable<UserPlantsDTO>>> GetUserPlants(int userId)
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
        public async Task<ActionResult<UserPlantsDTO>> GetUserPlant(int userPlantId)
        {
            try
            {
                var userPlant = await _userPlantRepository.GetByIdAsync(userPlantId);
                if (userPlant == null)
                {
                    return NotFound($"User plant with ID {userPlantId} not found.");
                }
                var userPlantDto = new UserPlantsDTO
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
        public async Task<ActionResult<UserPlantsDTO>> GetUserPlantByDevice(int deviceId)
        {
            try
            {
                var userPlant = await _userPlantRepository.GetUserPlantByDeviceIdAsync(deviceId);
                if (userPlant == null)
                {
                    return NotFound($"No plant assignment found for device {deviceId}.");
                }
                var userPlantDto = new UserPlantsDTO
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
        public async Task<ActionResult<UserPlantsDTO>> CreateUserPlant(UserPlantsDTO dto)
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
                var userPlantDto = new UserPlantsDTO
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
        [HttpPut("{id}/device")]
        public async Task<ActionResult> UpdateDeviceAssignment(int id, UserPlantsDTO dto)
        {
            try
            {
                var existingUserPlant = await _userPlantRepository.GetByIdAsync(id);

                if (existingUserPlant == null)
                {
                    return NotFound($"User plant with ID {id} not found.");
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
                await _userPlantRepository.UpdateAsync(existingUserPlant);
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
        [HttpGet("userplants/{userId}/plants")]
        public async Task<ActionResult<IEnumerable<PlantDTO>>> GetPlantsBelongingToUser(int userId)
        {
            try
            {
                var userPlants = await _userPlantRepository.GetUserPlantsByUserIdAsync(userId);
                if (!userPlants.Any())
                {
                    return NotFound($"User with id {userId} not found");
                }
                    
                var plantsDto = userPlants.Select(up => new PlantDTO
                {
                    PlantId = up.Plant.PlantId,
                    ScientificName = up.Plant.ScientificName,
                    FamiliarName = up.Plant.FamiliarName,
                    FunFact = up.Plant.FunFact,
                    MoistureLowRange = up.Plant.MoistureLowRange,
                    MoistureHighRange = up.Plant.MoistureHighRange
                }).ToList();
                return Ok(plantsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
