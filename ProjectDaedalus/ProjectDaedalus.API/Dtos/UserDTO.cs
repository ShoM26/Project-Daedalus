namespace ProjectDaedalus.API.Dtos
{
    public class UserDTO
    {
        public int? UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; }
    }
}

