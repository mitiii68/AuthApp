using AuthApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Controllers
{
    [Route("api/kato")]
    [ApiController]
    public class KatoController : ControllerBase
    {
        private readonly AppDbContext _db;
        private const string VKO = "63";

        public KatoController(AppDbContext db) => _db = db;

        
        [HttpGet("ping")]
        public async Task<IActionResult> Ping()
        {
            try
            {
                var count = await _db.KatoEntries.CountAsync();
                var sample = await _db.KatoEntries.FirstOrDefaultAsync();
                return Ok(new
                {
                    ok = true,
                    count = count,
                    sample = sample?.RusName ?? "таблица пуста"
                });
            }
            catch (Exception ex)
            {
                return Ok(new { ok = false, error = ex.Message });
            }
        }

        [HttpGet("districts")]
        public async Task<IActionResult> Districts()
        {
            var rows = await _db.KatoEntries
                .Where(e => e.Ab == VKO && e.Cd != "00"
                         && e.Ef == "00" && e.Hij == "000")
                .OrderBy(e => e.RusName)
                .Select(e => new { code = e.Cd, name = e.RusName, fullCode = e.Code })
                .ToListAsync();

            return Ok(rows);
        }

        [HttpGet("rural/{districtCd}")]
        public async Task<IActionResult> Rural(string districtCd)
        {
            var rows = await _db.KatoEntries
                .Where(e => e.Ab == VKO && e.Cd == districtCd
                         && e.Ef != "00" && e.Hij == "000")
                .OrderBy(e => e.RusName)
                .Select(e => new { code = e.Ef, name = e.RusName, fullCode = e.Code })
                .ToListAsync();

            return Ok(rows);
        }

        [HttpGet("settlements/{districtCd}/{ruralCd}")]
        public async Task<IActionResult> Settlements(string districtCd, string ruralCd)
        {
            IQueryable<AuthApp.Models.KatoEntry> query;

            if (ruralCd == "00")
                query = _db.KatoEntries.Where(e =>
                    e.Ab == VKO && e.Cd == districtCd &&
                    e.Ef == "00" && e.Hij != "000");
            else
                query = _db.KatoEntries.Where(e =>
                    e.Ab == VKO && e.Cd == districtCd &&
                    e.Ef == ruralCd && e.Hij != "000");

            var rows = await query
                .OrderBy(e => e.RusName)
                .Select(e => new { name = e.RusName, fullCode = e.Code })
                .ToListAsync();

            return Ok(rows);
        }

        
    }
}
