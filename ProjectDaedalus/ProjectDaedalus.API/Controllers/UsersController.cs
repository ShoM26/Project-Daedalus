using Microsoft.AspNetCore.Mvc;
using ProjectDaedalus.API.Dtos.User;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces; 
using ProjectDaedalus.API.Services;

namespace ProjectDaedalus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        public UsersController(IUserRepository userRepository, 
            IUnitOfWork unitOfWork, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }
        //GET user profile
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var u = await _userRepository.GetByIdAsync(userId);
                if (u == null)
                {
                    return NotFound($"User with id {userId} not found");
                }
                
                var user = new UserDto
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
        public async Task<IActionResult> CreateUser([FromBody] UserDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new{ message = "Invalid Payload"});
            }

            try
            {
                if (_userRepository == null)
                {
                    return StatusCode(500, new {message = "UserRepository is not injected"});
                }
                var emailExists = await _userRepository.EmailExistsAsync(dto.Email);
                if (emailExists)
                {
                    return Conflict(new {message = "User with that email already exists"});
                }

                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    Password = dto.Password,
                    CreatedAt = DateTime.Now
                };
                var createdUser = await _userRepository.AddAsync(user);

                var resultDto = new LoginResponseDto
                {
                    UserId = createdUser.UserId,
                    Username = createdUser.Username,
                    Email = createdUser.Email,
                    Success = true
                };
                await _unitOfWork.SaveChangesAsync();
                return CreatedAtAction(nameof(GetUserById), new { userId = createdUser.UserId }, resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {message = $"Internal server error: {ex.Message}"});
            }
        }
        //PUT update a user
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserDto dto)
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
                
                var updatedUser = await _userRepository.UpdateAsync(existingUser);
                
                var resultDto = new UserDto
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
        
        //User sign in Request
        [HttpPost("login")]
        public async Task<IActionResult> UserSignIn([FromBody] LoginRequestDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new LoginFailureDto 
                    { 
                        Success = false, 
                        Message = "Email and Password are required" 
                    });
                }
                
                var user = await _userRepository.ValidateUserCredentialsAsync(request.Email, request.Password);
            
                if (user == null)
                {
                    return Unauthorized(new LoginFailureDto 
                    { 
                        Success = false, 
                        Message = "Invalid email or password" 
                    });
                }
                
                var token = _jwtService.GenerateToken(user.UserId, user.Username);

                return Ok(new LoginResponseDto
                {
                    Success = true,
                    Token = token,
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
            
                return StatusCode(500, new LoginFailureDto 
                { 
                    Success = false, 
                    Message = "An error occurred during login" 
                });
            }
        }
    }
}
