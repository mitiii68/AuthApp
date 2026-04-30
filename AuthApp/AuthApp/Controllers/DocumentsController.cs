using AuthApp.Data;
using AuthApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DocumentsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        

        public async Task<IActionResult> Upload()
        {
            ViewBag.Tags = await _context.Tags
                .Include(t => t.TagCategoryTags)
                    .ThenInclude(tct => tct.TagCategory)
                .ToListAsync();

            ViewBag.Categories = await _context.TagCategories.ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, List<int> tagIds)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Error = "Выберите файл";
                ViewBag.Tags = await _context.Tags
                    .Include(t => t.TagCategoryTags)
                        .ThenInclude(tct => tct.TagCategory)
                    .ToListAsync();
                ViewBag.Categories = await _context.TagCategories.ToListAsync();
                return View();
            }

            var allowedExtensions = new[] { ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".zip", ".rar", ".7z" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                ViewBag.Error = "Недопустимый формат файла";
                ViewBag.Tags = await _context.Tags
                    .Include(t => t.TagCategoryTags)
                        .ThenInclude(tct => tct.TagCategory)
                    .ToListAsync();
                ViewBag.Categories = await _context.TagCategories.ToListAsync();
                return View();
            }

            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var uniqueFileName = Guid.NewGuid() + extension;
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var document = new FileDocuments
            {
                FileName = file.FileName,
                FilePath = "/uploads/" + uniqueFileName,
                Extension = extension,
                UploadDate = DateTime.Now
            };

            _context.FileDocuments.Add(document);
            await _context.SaveChangesAsync();

            foreach (var tagId in tagIds)
            {
                _context.FileTags.Add(new FileTag
                {
                    FileDocumentsId = document.Id,
                    TagId = tagId
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var document = await _context.FileDocuments
                .Include(d => d.FileTags)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (document != null)
            {
                
                var fullPath = Path.Combine(_environment.WebRootPath,
                    document.FilePath!.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                
                _context.FileDocuments.Remove(document);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index(
    string search,
    List<int> selectedTags,
    string sortOrder)
        {
            ViewBag.Tags = await _context.Tags
                .Include(t => t.TagCategoryTags)
                    .ThenInclude(tct => tct.TagCategory)
                .ToListAsync();

            var documentsQuery = _context.FileDocuments
                .Include(d => d.FileTags)
                    .ThenInclude(ft => ft.Tag)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                documentsQuery = documentsQuery.Where(d =>
                   d.FileName != null && d.FileName.Contains(search));
            }

            if (selectedTags != null && selectedTags.Any())
            {
                documentsQuery = documentsQuery.Where(d =>
                    d.FileTags.Any(ft =>
                        selectedTags.Contains(ft.TagId)));
            }

            documentsQuery = sortOrder switch
            {
                "new" => documentsQuery.OrderByDescending(d => d.UploadDate),
                "old" => documentsQuery.OrderBy(d => d.UploadDate),
                "az" => documentsQuery.OrderBy(d => d.FileName),
                "za" => documentsQuery.OrderByDescending(d => d.FileName),
                _ => documentsQuery.OrderByDescending(d => d.UploadDate)
            };

            var documents = await documentsQuery.ToListAsync();

            return View(documents);
        }
    }
}
