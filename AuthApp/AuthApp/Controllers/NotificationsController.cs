using AuthApp.Data;
using AuthApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public NotificationsController(AppDbContext db)
        {
            _db = db;
        }

        // Загрузить непрочитанные при входе на сайт
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnread()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Ok(new List<object>());

            var notifs = await _db.UserNotifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .Select(n => new {
                    n.Id,
                    n.Type,
                    n.Title,
                    n.Message,
                    n.Url,
                    n.IsRead,
                    SentAt = n.CreatedAt
                })
                .ToListAsync();

            return Ok(notifs);
        }

        // Пометить все как прочитанные
        [HttpPost("read-all")]
        public async Task<IActionResult> ReadAll()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Ok();

            await _db.UserNotifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));

            return Ok();
        }

        private int GetCurrentUserId()
        {
            var s = HttpContext.Session.GetString("UserId");
            return s != null && int.TryParse(s, out var id) ? id : 0;
        }
    }
}
