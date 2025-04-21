using Microsoft.AspNetCore.Mvc;
using NotificationAPI.DbContexts;
using NotificationAPI.Entities;
using NotificationAPI.Models;
using NotificationAPI.Repositories;
using NotificationAPI.Services;

namespace NotificationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly INotificationRepository _notificationRepository;

        public NotificationsController(
            INotificationService notificationService,
            INotificationRepository notificationRepository)
        {
            _notificationService = notificationService;
            _notificationRepository = notificationRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
        {
            var notification = await _notificationService.CreateNotification(request);

            return CreatedAtAction(nameof(GetById), new { id = notification.Id }, notification);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
                return NotFound();

            return Ok(notification);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notifications = await _notificationRepository.GetAllAsync();

            return Ok(notifications);
        }

        [HttpPost("{id}/force-send")]
        public async Task<IActionResult> ForceSend(Guid id)
        {
            //var notification = await _context.Notifications.FindAsync(id);
            //if (notification == null) return NotFound();

            //if (notification.Status is NotificationStatus.Sent or NotificationStatus.Cancelled)
            //    return BadRequest("Cannot force-send already sent or canceled notification");

            //notification.ForceSend = true;
            //await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            //var notification = await _context.Notifications.FindAsync(id);
            //if (notification == null) return NotFound();

            //if (notification.Status == NotificationStatus.Cancelled)
            //    return BadRequest("Cannot cancel already sent notification");

            //notification.Status = NotificationStatus.Cancelled;
            //await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
