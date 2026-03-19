using System.Collections.Concurrent;

namespace FougeraClub.Web.Notifications
{
    public sealed class InMemoryNotificationStore : INotificationStore
    {
        private readonly ConcurrentQueue<NotificationItem> _notifications = new();
        private const int MaxItems = 300;

        public void Add(string title, string message)
        {
            _notifications.Enqueue(new NotificationItem
            {
                Title = title,
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false
            });

            while (_notifications.Count > MaxItems)
            {
                _notifications.TryDequeue(out _);
            }
        }

        public IReadOnlyList<NotificationItem> GetUnread(int take = 20)
        {
            return _notifications
                .Where(n => !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .Take(take)
                .ToList();
        }

        public int GetUnreadCount()
        {
            return _notifications.Count(n => !n.IsRead);
        }

        public void MarkAllAsRead()
        {
            foreach (var item in _notifications)
            {
                item.IsRead = true;
            }
        }
    }
}
