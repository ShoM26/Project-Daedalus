namespace ProjectDaedalus.Core.Configuration
{
    public class NotificationWorkerSettings
    {
        /// <summary>
        /// How often the worker runs (in minutes)
        /// </summary>
        public int IntervalMinutes { get; set; } = 5;
        
        /// <summary>
        /// Enable/disable the worker
        /// </summary>
        public bool Enabled { get; set; } = true;
        
        /// <summary>
        /// Base URL for dashboard links in emails
        /// </summary>
        public string DashboardBaseUrl { get; set; } = "http://localhost:5713";
    }
}