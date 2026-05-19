using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthApp.Data;
using AuthApp.Models;

namespace AuthApp.Controllers.Api
{
    [ApiController]
    [Route("api/positions")]
    public class PositionsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public PositionsController(AppDbContext db)
        {
            _db = db;
        }

        private bool IsAuthenticated()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("user"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!IsAuthenticated()) return Unauthorized();

            var positions = await _db.Positions
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.CreatedAt,
                    UserCount = p.Users.Count()
                })
                .ToListAsync();

            return Ok(positions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!IsAuthenticated()) return Unauthorized();

            var pos = await _db.Positions.FindAsync(id);
            if (pos == null) return NotFound();
            return Ok(new { pos.Id, pos.Name, pos.CreatedAt });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PositionDto dto)
        {
            if (!IsAuthenticated()) return Unauthorized();

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Название не может быть пустым");

            var trimmed = dto.Name.Trim();

            var exists = await _db.Positions
                .AnyAsync(p => p.Name.ToLower() == trimmed.ToLower());
            if (exists)
                return Conflict("Должность с таким названием уже существует");

            var pos = new Position
            {
                Name      = trimmed,
                CreatedAt = DateTime.UtcNow
            };
            _db.Positions.Add(pos);
            await _db.SaveChangesAsync();

            return Ok(new { pos.Id, pos.Name, pos.CreatedAt });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PositionDto dto)
        {
            if (!IsAuthenticated()) return Unauthorized();

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Название не может быть пустым");

            var pos = await _db.Positions.FindAsync(id);
            if (pos == null) return NotFound();

            var trimmed = dto.Name.Trim();

            var exists = await _db.Positions
                .AnyAsync(p => p.Name.ToLower() == trimmed.ToLower() && p.Id != id);
            if (exists)
                return Conflict("Должность с таким названием уже существует");

            pos.Name = trimmed;
            await _db.SaveChangesAsync();

            return Ok(new { pos.Id, pos.Name, pos.CreatedAt });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAuthenticated()) return Unauthorized();

            var pos = await _db.Positions.FindAsync(id);
            if (pos == null) return NotFound();

            _db.Positions.Remove(pos);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }

    public class PositionDto
    {
        public string Name { get; set; } = "";
    }
}
