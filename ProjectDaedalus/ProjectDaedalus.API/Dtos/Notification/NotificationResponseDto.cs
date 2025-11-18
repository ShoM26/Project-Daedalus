namespace ProjectDaedalus.API.Dtos.Notification
{
    public class NotificationResponseDto
    {
        public int NotificationId { get; set; }
        public int UserPlantId { get; set; }
        public string Message { get; set; }
        public string NotificationType { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserPlantName { get; set; }
    }
}

