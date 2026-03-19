using FougeraClub.Web.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace FougeraClub.Web.Controllers
{
    [Route("Admin/[controller]/[action]")]
    public class NotificationsController : Controller
    {
        private readonly INotificationStore _notificationStore;

        public NotificationsController(INotificationStore notificationStore)
        {
            _notificationStore = notificationStore;
        }

        [HttpGet]
        public IActionResult GetUnread()
        {
            var unread = _notificationStore.GetUnread()
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Message,
                    CreatedAt = n.CreatedAt.ToString("yyyy-MM-dd HH:mm")
                });

            return Json(unread);
        }

        [HttpGet]
        public IActionResult GetUnreadCount()
        {
            return Json(_notificationStore.GetUnreadCount());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAllRead()
        {
            _notificationStore.MarkAllAsRead();
            return Ok();
        }
    }
}
