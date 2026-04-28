using AuthApp.Data;
using AuthApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Controllers
{
    public class TagsController : Controller
    {
        private readonly AppDbContext _db;

        public TagsController(AppDbContext db)
        {
            _db = db;
        }

        // MVC — страница /Tags/Index
        public async Task<IActionResult> Index()
        {
            var tags = await _db.Tags
                .Include(t => t.TagCategoryTags)
                    .ThenInclude(tct => tct.TagCategory)
                .ToListAsync();

            ViewBag.Categories = await _db.TagCategories.ToListAsync();

            return View(tags);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, int categoryId)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var tag = new Tag { Name = name };
                _db.Tags.Add(tag);
                await _db.SaveChangesAsync();

                _db.TagCategoryTags.Add(new TagCategoryTag
                {
                    TagId = tag.Id,
                    TagCategoryId = categoryId
                });
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var tag = await _db.Tags.FindAsync(id);
            if (tag != null)
            {
                _db.Tags.Remove(tag);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // API — GET /api/tags/by-categories?categoryIds=1&categoryIds=2
        [HttpGet("/api/tags/by-categories")]
        public async Task<IActionResult> GetTagsByCategories([FromQuery] List<int> categoryIds)
        {
            IQueryable<Tag> query = _db.Tags;

            if (categoryIds != null && categoryIds.Count > 0)
            {
                query = query
                    .Where(t => t.TagCategoryTags
                        .Any(tct => categoryIds.Contains(tct.TagCategoryId)));
            }

            var tags = await query
                .OrderBy(t => t.Name)
                .Select(t => new { t.Id, t.Name })
                .ToListAsync();

            return Ok(tags);
        }
    }
}
