using AuthApp.Data;
using AuthApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Controllers
{
    public class TagsController : Controller
    {
        private readonly AppDbContext _context;

        public TagsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tags = await _context.Tags
                .Include(t => t.Category)
                .ToListAsync();

            ViewBag.Categories = await _context.TagCategories.ToListAsync();
            return View(tags);
        }
        [HttpPost]
        public async Task<IActionResult> Create(string name, int categoryId)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _context.Tags.Add(new Tag
                {
                    Name = name,
                   TagCategoryId = categoryId
                });

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var tag = await _context.Tags.FindAsync(id);

            if (tag != null)
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}