namespace FougeraClub.Web.Notifications
{
    public interface INotificationStore
    {
        void Add(string title, string message);
        IReadOnlyList<NotificationItem> GetUnread(int take = 20);
        int GetUnreadCount();
        void MarkAllAsRead();
    }
}
