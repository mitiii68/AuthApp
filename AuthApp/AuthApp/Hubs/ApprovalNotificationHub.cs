using Microsoft.AspNetCore.SignalR;

namespace AuthApp.Hubs
{
    public class ApprovalNotificationHub : Hub
    {
        // Клиент передаёт свой userId (берётся из сессии на клиенте)
        public async Task Join(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
    }

    // DTO уведомления
    public class ApprovalNotificationDto
    {
        public string Type    { get; set; } = "approval"; // started | decided | approved
        public string Title   { get; set; } = "";
        public string Message { get; set; } = "";
        public string? Url    { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }

    // Расширение для отправки из контроллеров/сервисов
    public static class ApprovalHubExtensions
    {
        public static async Task SendApprovalNotification(
            this IHubContext<ApprovalNotificationHub> hub,
            int recipientUserId,
            ApprovalNotificationDto dto)
        {
            await hub.Clients
                     .Group($"user_{recipientUserId}")
                     .SendAsync("ReceiveNotification", dto);
        }
    }
}
