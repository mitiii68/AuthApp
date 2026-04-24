using AuthApp.Data;
using AuthApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Controllers
{
    public class TagCategoriesController : Controller
    {
        private readonly AppDbContext _context;
             public TagCategoriesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.TagCategories.ToListAsync();
            return View(categories);
        }


        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _context.TagCategories.Add(new TagCategory
                {
                    Name = name
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.TagCategories.FindAsync(id);

            if (category != null)
            {
                _context.TagCategories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        

    }
}