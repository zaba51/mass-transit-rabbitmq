using Microsoft.AspNetCore.Mvc;
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
            await _notificationService.ForceSend(id);
            return Ok();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            await _notificationService.CancelNotification(id);
            return Ok();
        }

    }
}
