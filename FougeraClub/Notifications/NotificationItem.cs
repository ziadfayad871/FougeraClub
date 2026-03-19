namespace FougeraClub.Web.Notifications
{
    public sealed class NotificationItem
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Title { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; } = DateTime.Now;
        public bool IsRead { get; set; }
    }
}
