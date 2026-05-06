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

        private string CurrentUserEmail()
            => HttpContext.Session.GetString("UserEmail") ?? "неизвестный";

        private async Task LogActionAsync(string action)
        {
            _context.UserActionLog.Add(new UserActionLog
            {
                UserEmail  = CurrentUserEmail(),
                Action     = action,
                ActionTime = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }

        // ──────────────────────────────────────────────
        // UPLOAD
        // ──────────────────────────────────────────────

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
                await FillUploadViewBag();
                return View();
            }

            var allowedExtensions = new[] { ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".zip", ".rar", ".7z" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                ViewBag.Error = "Недопустимый формат файла";
                await FillUploadViewBag();
                return View();
            }

            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var uniqueFileName = Guid.NewGuid() + extension;
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            var document = new FileDocuments
            {
                FileName   = file.FileName,
                FilePath   = "/uploads/" + uniqueFileName,
                Extension  = extension,
                UploadDate = DateTime.Now
            };

            _context.FileDocuments.Add(document);
            await _context.SaveChangesAsync();

            foreach (var tagId in tagIds)
            {
                _context.FileTags.Add(new FileTag
                {
                    FileDocumentsId = document.Id,
                    TagId           = tagId
                });
            }

            await _context.SaveChangesAsync();
            await LogActionAsync($"Загрузил документ «{file.FileName}»");

            return RedirectToAction("Index");
        }

        // ──────────────────────────────────────────────
        // MOVE TO TRASH  (бывший Delete — теперь мягкое удаление)
        // ──────────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var document = await _context.FileDocuments.FindAsync(id);

            if (document != null)
            {
                document.IsDeleted = true;
                document.DeletedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                await LogActionAsync($"Переместил документ «{document.FileName}» в корзину");
            }

            return RedirectToAction("Index");
        }

        // ──────────────────────────────────────────────
        // RESTORE
        // ──────────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            var document = await _context.FileDocuments.FindAsync(id);

            if (document != null)
            {
                document.IsDeleted = false;
                document.DeletedAt = null;
                await _context.SaveChangesAsync();
                await LogActionAsync($"Восстановил документ «{document.FileName}» из корзины");
            }

            return RedirectToAction("Trash");
        }

        // ──────────────────────────────────────────────
        // PURGE (безвозвратное удаление из корзины)
        // ──────────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> Purge(int id)
        {
            var document = await _context.FileDocuments
                .Include(d => d.FileTags)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (document != null)
            {
                var fullPath = Path.Combine(_environment.WebRootPath,
                    document.FilePath!.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);

                var fileName = document.FileName;
                _context.FileDocuments.Remove(document);
                await _context.SaveChangesAsync();
                await LogActionAsync($"Безвозвратно удалил документ «{fileName}» из корзины");
            }

            return RedirectToAction("Trash");
        }

        

        public async Task<IActionResult> Download(int id)
        {
            var document = await _context.FileDocuments.FindAsync(id);

            if (document == null || string.IsNullOrEmpty(document.FilePath))
                return NotFound();

            var fullPath = Path.Combine(_environment.WebRootPath,
                document.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            await LogActionAsync($"Скачал документ «{document.FileName}»");

            var contentType = document.Extension?.ToLower() switch
            {
                ".pdf"  => "application/pdf",
                ".doc"  => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls"  => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".zip"  => "application/zip",
                ".rar"  => "application/x-rar-compressed",
                ".7z"   => "application/x-7z-compressed",
                _       => "application/octet-stream"
            };

            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            return File(fileBytes, contentType, document.FileName);
        }

       
        public async Task<IActionResult> Index(
            string search,
            List<int> selectedTags,
            string sortOrder,
            int page = 1,
            int pageSize = 10)
        {
            ViewBag.Tags = await _context.Tags
                .Include(t => t.TagCategoryTags)
                    .ThenInclude(tct => tct.TagCategory)
                .ToListAsync();

           
            ViewBag.TrashCount = await _context.FileDocuments.CountAsync(d => d.IsDeleted);

            
            var userEmail = CurrentUserEmail();
            var favoriteIds = await _context.FavoriteDocuments
                .Where(f => f.UserEmail == userEmail)
                .Select(f => f.FileDocumentsId)
                .ToHashSetAsync();
            ViewBag.FavoriteIds = favoriteIds;
            ViewBag.FavoritesCount = favoriteIds.Count;

            var documentsQuery = _context.FileDocuments
                .Where(d => !d.IsDeleted)                   
                .Include(d => d.FileTags)
                    .ThenInclude(ft => ft.Tag)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                documentsQuery = documentsQuery.Where(d =>
                    d.FileName != null && d.FileName.Contains(search));

            if (selectedTags != null && selectedTags.Any())
                documentsQuery = documentsQuery.Where(d =>
                    d.FileTags.Any(ft => selectedTags.Contains(ft.TagId)));

            documentsQuery = sortOrder switch
            {
                "new" => documentsQuery.OrderByDescending(d => d.UploadDate),
                "old" => documentsQuery.OrderBy(d => d.UploadDate),
                "az"  => documentsQuery.OrderBy(d => d.FileName),
                "za"  => documentsQuery.OrderByDescending(d => d.FileName),
                _     => documentsQuery.OrderByDescending(d => d.UploadDate)
            };

            var totalItems = await documentsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            page = Math.Clamp(page, 1, Math.Max(1, totalPages));

            var documents = await documentsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page       = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalItems;
            ViewBag.SortOrder  = sortOrder;
            ViewBag.Search     = search;
            ViewBag.SelectedTags = selectedTags ?? new List<int>();
            ViewBag.PageSize   = pageSize;

            return View(documents);
        }

       

        public async Task<IActionResult> Trash(
            string search,
            string sortOrder,
            int page = 1,
            int pageSize = 10)
        {
            var query = _context.FileDocuments
                .Where(d => d.IsDeleted)
                .Include(d => d.FileTags)
                    .ThenInclude(ft => ft.Tag)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(d => d.FileName != null && d.FileName.Contains(search));

            query = sortOrder switch
            {
                "old" => query.OrderBy(d => d.DeletedAt),
                "az"  => query.OrderBy(d => d.FileName),
                "za"  => query.OrderByDescending(d => d.FileName),
                _     => query.OrderByDescending(d => d.DeletedAt)  // новые удалённые — первые
            };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            page = Math.Clamp(page, 1, Math.Max(1, totalPages));

            var documents = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page       = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalItems;
            ViewBag.SortOrder  = sortOrder;
            ViewBag.Search     = search;
            ViewBag.PageSize   = pageSize;

            return View(documents);
        }

        // ──────────────────────────────────────────────
        // TOGGLE FAVORITE
        // ──────────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int id, string? returnUrl)
        {
            var userEmail = CurrentUserEmail();
            var existing  = await _context.FavoriteDocuments
                .FirstOrDefaultAsync(f => f.UserEmail == userEmail && f.FileDocumentsId == id);

            if (existing != null)
            {
                _context.FavoriteDocuments.Remove(existing);
                await _context.SaveChangesAsync();
                await LogActionAsync($"Убрал документ (Id={id}) из избранного");
            }
            else
            {
                // Проверяем что документ существует
                var doc = await _context.FileDocuments.FindAsync(id);
                if (doc != null)
                {
                    _context.FavoriteDocuments.Add(new FavoriteDocument
                    {
                        UserEmail       = userEmail,
                        FileDocumentsId = id,
                        AddedAt         = DateTime.Now
                    });
                    await _context.SaveChangesAsync();
                    await LogActionAsync($"Добавил документ «{doc.FileName}» в избранное");
                }
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index");
        }

        // ──────────────────────────────────────────────
        // FAVORITES PAGE
        // ──────────────────────────────────────────────

        public async Task<IActionResult> Favorites(
            string search,
            string sortOrder,
            int page = 1,
            int pageSize = 10)
        {
            var userEmail = CurrentUserEmail();

            var query = _context.FavoriteDocuments
                .Where(f => f.UserEmail == userEmail)
                .Include(f => f.FileDocument)
                    .ThenInclude(d => d!.FileTags)
                        .ThenInclude(ft => ft.Tag)
                .Where(f => f.FileDocument != null && !f.FileDocument.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(f =>
                    f.FileDocument!.FileName != null &&
                    f.FileDocument.FileName.Contains(search));

            query = sortOrder switch
            {
                "old"  => query.OrderBy(f => f.AddedAt),
                "az"   => query.OrderBy(f => f.FileDocument!.FileName),
                "za"   => query.OrderByDescending(f => f.FileDocument!.FileName),
                "date_new" => query.OrderByDescending(f => f.FileDocument!.UploadDate),
                "date_old" => query.OrderBy(f => f.FileDocument!.UploadDate),
                _      => query.OrderByDescending(f => f.AddedAt)
            };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Clamp(page, 1, Math.Max(1, totalPages));

            var favorites = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page       = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalItems;
            ViewBag.SortOrder  = sortOrder;
            ViewBag.Search     = search;
            ViewBag.PageSize   = pageSize;

            return View(favorites);
        }

        private async Task FillUploadViewBag()
        {
            ViewBag.Tags = await _context.Tags
                .Include(t => t.TagCategoryTags)
                    .ThenInclude(tct => tct.TagCategory)
                .ToListAsync();
            ViewBag.Categories = await _context.TagCategories.ToListAsync();
        }
    }
}
