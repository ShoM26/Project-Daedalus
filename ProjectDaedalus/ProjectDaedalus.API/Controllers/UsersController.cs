using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectDaedalus.API.Dtos.User;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces; // assuming entities live in Core
using ProjectDaedalus.Infrastructure.Data; // DbContext

namespace ProjectDaedalus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        public UsersController(DaedalusContext context, IUserRepository userRepository,
            IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
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

                // Convert to DTO for security
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
                var existingUser = await _userRepository.EmailExistsAsync(dto.Email);
                if (existingUser)
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

                //Update in database
                var updatedUser = await _userRepository.UpdateAsync(existingUser);

                //Return updated device as DTO for security
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
                // Validate input
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new LoginFailureDto 
                    { 
                        Success = false, 
                        Message = "Username and password are required" 
                    });
                }

                // Validate credentials against database
                var user = await _userRepository.ValidateUserCredentialsAsync(request.Username, request.Password);
            
                if (user == null)
                {
                    return Unauthorized(new LoginFailureDto 
                    { 
                        Success = false, 
                        Message = "Invalid username or password" 
                    });
                }

                // Generate JWT token
                var token = GenerateJwtToken(user);

                // Return success response
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
                // Log the exception (add proper logging here)
                Console.WriteLine($"Login error: {ex.Message}");
            
                return StatusCode(500, new LoginFailureDto 
                { 
                    Success = false, 
                    Message = "An error occurred during login" 
                });
            }
        }
        private string GenerateJwtToken(User user)
        {
            // JWT configuration - reads from appsettings.json
            var jwtKey = _configuration["Jwt:Key"] ?? "your-super-secret-jwt-key-for-development-minimum-32-characters";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "ProjectDaedalus";
            var jwtExpireHours = int.Parse(_configuration["Jwt:ExpireHours"] ?? "24");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtKey);
        
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", user.UserId.ToString()),
                    new Claim("username", user.Username),
                    new Claim("email", user.Email),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(jwtExpireHours),
                Issuer = jwtIssuer,
                Audience = jwtIssuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
        
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
