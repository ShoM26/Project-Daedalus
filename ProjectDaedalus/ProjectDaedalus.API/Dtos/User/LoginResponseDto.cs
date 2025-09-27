namespace ProjectDaedalus.API.Dtos.User
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
