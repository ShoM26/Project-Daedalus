using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.API.Dtos;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces; // assuming entities live in Core
using ProjectDaedalus.Infrastructure.Data; // DbContext

namespace ProjectDaedalus.API.Controllers
{
    public class UsersController : ControllerBase
    {
        private readonly DaedalusContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IPlantRepository _plantRepository;
        //GET user profile
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var u = await _userRepository.GetByIdAsync(userId);
                if (u == null)
                {
                    return NotFound($"User with id {userId} not found");
                }

                // Convert to DTO for security
                var user = new UserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    Password = u.Password,
                    CreatedAt = u.CreatedAt
                };
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //POST create a new user
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid Payload");
            }

            try
            {
                var existingUser = await _userRepository.GetByUsernameAsync(dto.Username);
                if (existingUser != null)
                {
                    return Conflict($"User with username {dto.Username} already exists");
                }
                
                var emailExists = await _userRepository.EmailExistsAsync(dto.Email);
                if (emailExists)
                {
                    return Conflict($"Email {dto.Email} already exists");
                }

                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    Password = dto.Password,
                    CreatedAt = DateTime.Now
                };
                var createdUser = await _userRepository.AddAsync(user);

                var resultDto = new UserDTO
                {
                    UserId = createdUser.UserId,
                    Username = createdUser.Username,
                    Email = createdUser.Email,
                    Password = createdUser.Password,
                    CreatedAt = createdUser.CreatedAt
                };
                return CreatedAtAction(nameof(GetUserById), new { userId = createdUser.UserId }, resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //PUT update a user
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid Payload");
            }

            try
            {
                var existingUser = await _userRepository.GetByIdAsync(userId);
                if (existingUser == null)
                {
                    return BadRequest($"User with id {userId} not found");
                }

                if (!string.IsNullOrEmpty(dto.Username) && dto.Username != existingUser.Username)
                {
                    var duplicateUser = await _userRepository.GetByUsernameAsync(dto.Username);
                    if (duplicateUser != null && duplicateUser.Username != existingUser.Username)
                    {
                        return Conflict($"User with username  {dto.Username} already exists");
                    }
                }
                
                if (!string.IsNullOrEmpty(dto.Email) && dto.Email != existingUser.Email)
                {
                    var duplicateUser = await _userRepository.GetByUsernameAsync(dto.Email);
                    if (duplicateUser != null && duplicateUser.Email != existingUser.Email)
                    {
                        return Conflict($"User with email  {dto.Email} already exists");
                    }
                }

                if (!string.IsNullOrEmpty(dto.Username))
                    existingUser.Username = dto.Username;
                if (!string.IsNullOrEmpty(dto.Email))
                    existingUser.Email = dto.Email;
                if (!string.IsNullOrEmpty(dto.Password))
                    existingUser.Password = dto.Password;

                //Update in database
                var updatedUser = await _userRepository.UpdateAsync(existingUser);

                //Return updated device as DTO for security
                var resultDto = new UserDTO
                {
                    Username = updatedUser.Username,
                    Email = updatedUser.Email,
                    Password = updatedUser.Password,
                    CreatedAt = updatedUser.CreatedAt
                };
                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //DELETE a user
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return NotFound($"User with id {userId} not found");
                }

                await _userRepository.DeleteAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //GET all plants belonging to user
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Plant>>> GetPlantsBelongingToUser(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return NotFound($"User with id {userId} not found");
                }

                var plants = user.UserPlants.Select(up => new PlantDTO
                {
                    PlantId = up.Plant.PlantId,
                    ScientificName = up.Plant.ScientificName,
                    FamiliarName = up.Plant.FamiliarName,
                    FunFact = up.Plant.FunFact,
                    MoistureLowRange = up.Plant.MoistureLowRange,
                    MoistureHighRange = up.Plant.MoistureHighRange
                }).ToList();
                return Ok(plants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
