using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.Infrastructure.Data;

namespace ProjectDaedalus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class NotificationsController : ControllerBase
    {
        private readonly DaedalusContext _context;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(
            DaedalusContext context,
            ILogger<NotificationsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        private int GetAuthenticatedUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value 
                              ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }

            return userId;
        }

        /// <summary>
        /// Get count of unread notifications for the current user
        /// Called by frontend polling (every 2 minutes)
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                var count = await _context.Notifications
                    .Include(n => n.UserPlant)
                    .Where(n => n.UserPlant.UserId == userId && !n.IsRead)
                    .CountAsync();

                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get unread notification count for user");
                return StatusCode(500, new { error = "Failed to retrieve notification count" });
            }
        }

        /// <summary>
        /// Get list of notifications for the current user
        /// Called when user opens the notification modal
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] bool unreadOnly = false,
            [FromQuery] int limit = 50)
        {
            try
            {
                var userId = GetAuthenticatedUserId(); 
                var query = _context.Notifications
                    .Include(n => n.UserPlant)
                        .ThenInclude(up => up.Plant)
                    .Where(n => n.UserPlant.UserId == userId);

                if (unreadOnly)
                {
                    query = query.Where(n => !n.IsRead);
                }

                var notifications = await query
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(limit)
                    .Select(n => new
                    {
                        id = n.NotificationId,
                        message = n.Message,
                        type = n.NotificationType.ToString(),
                        isRead = n.IsRead,
                        createdAt = n.CreatedAt,
                        userPlantId = n.UserPlantId,
                        plantName = n.UserPlant.Plant.FamiliarName
                    })
                    .ToListAsync();

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get notifications for user");
                return StatusCode(500, new { error = "Failed to retrieve notifications" });
            }
        }

        /// <summary>
        /// Mark a specific notification as read
        /// Called when user clicks "mark as read" on individual notification
        /// </summary>
        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                var notification = await _context.Notifications
                    .Include(n => n.UserPlant)
                    .FirstOrDefaultAsync(n => n.NotificationId == id && n.UserPlant.UserId == userId);

                if (notification == null)
                {
                    return NotFound(new { error = "Notification not found" });
                }

                if (notification.IsRead)
                {
                    return Ok(new { message = "Notification already marked as read" });
                }

                notification.IsRead = true;
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Marked notification {NotificationId} as read for user {UserId}", 
                    id, 
                    userId);

                return Ok(new { success = true, message = "Notification marked as read" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark notification {NotificationId} as read", id);
                return StatusCode(500, new { error = "Failed to update notification" });
            }
        }

        /// <summary>
        /// Mark all notifications as read for the current user
        /// Optional convenience endpoint
        /// </summary>
        [HttpPatch("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                var unreadNotifications = await _context.Notifications
                    .Include(n => n.UserPlant)
                    .Where(n => n.UserPlant.UserId == userId && !n.IsRead)
                    .ToListAsync();

                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Marked {Count} notifications as read for user {UserId}", 
                    unreadNotifications.Count, 
                    userId);

                return Ok(new { 
                    success = true, 
                    count = unreadNotifications.Count,
                    message = $"Marked {unreadNotifications.Count} notifications as read" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark all notifications as read for user");
                return StatusCode(500, new { error = "Failed to update notifications" });
            }
        }
    }
}