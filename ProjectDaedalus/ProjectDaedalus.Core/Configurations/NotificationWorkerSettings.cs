namespace ProjectDaedalus.Core.Configurations
{
    public class NotificationWorkerSettings
    {
        public int IntervalMinutes { get; set; } = 5;
        
        public bool Enabled { get; set; } = true;
        
        public string DashboardBaseUrl { get; set; } = "http://localhost:5713";
    }
}