using AuthApp.Data;
using AuthApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        private static readonly string[] AllowedExtensions =
            { ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".zip", ".rar", ".7z" };

        public ProjectsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context     = context;
            _environment = environment;
        }

        
        private void FillFormViewBag()
        {
            ViewBag.Users = GetUserNames();

            ViewBag.Counterparties = _context.Counterparties
                .OrderBy(c => c.Name)
                .Select(c => new { c.Id, c.Name, c.ShortName, c.IinBin })
                .ToList()
                .Select(c => new {
                    c.Id,
                    c.Name,
                    Display = string.IsNullOrEmpty(c.ShortName) ? c.Name : $"{c.Name} ({c.ShortName})"
                })
                .ToList<dynamic>();

            ViewBag.ProjectGroups = _context.Projects
                .Where(p => !string.IsNullOrEmpty(p.ProjectGroup))
                .Select(p => p.ProjectGroup!)
                .Distinct()
                .OrderBy(g => g)
                .ToList();

            ViewBag.Departments = _context.Projects
                .Where(p => !string.IsNullOrEmpty(p.Department))
                .Select(p => p.Department!)
                .Distinct()
                .OrderBy(d => d)
                .ToList();
        }

        [HttpGet]
        public IActionResult SearchCounterparties(string? q)
        {
            var items = _context.Counterparties
                .Where(c => string.IsNullOrEmpty(q) || c.Name.Contains(q) ||
                            (c.ShortName != null && c.ShortName.Contains(q)))
                .OrderBy(c => c.Name)
                .Take(20)
                .Select(c => new {
                    c.Id,
                    c.Name,
                    display = string.IsNullOrEmpty(c.ShortName) ? c.Name : $"{c.Name} ({c.ShortName})"
                })
                .ToList();
            return Json(items);
        }

        public async Task<IActionResult> Index(string? search, string? status, int page = 1, int pageSize = 10)
        {
            var query = _context.Projects
                .Include(p => p.ProjectDocuments)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p =>
                    p.Name!.Contains(search) ||
                    (p.Team != null && p.Team.Contains(search)));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(p => p.Status == status);

            query = query.OrderByDescending(p => p.CreatedAt);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Clamp(page, 1, Math.Max(1, totalPages));

            var projects = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page       = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalItems;
            ViewBag.Search     = search;
            ViewBag.Status     = status;
            ViewBag.PageSize   = pageSize;

            return View(projects);
        }

        public IActionResult Create()
        {
            FillFormViewBag();
            return View("CreateEdit", new Project());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project, List<IFormFile>? documents)
        {
            ValidateDates(project);

            if (!ModelState.IsValid)
            {
                FillFormViewBag();
                return View("CreateEdit", project);
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            if (documents != null && documents.Any())
                await SaveDocumentsAsync(project.Id, documents);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectDocuments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound();
            FillFormViewBag();
            return View("CreateEdit", project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project, List<IFormFile>? documents)
        {
            if (id != project.Id) return BadRequest();

            ValidateDates(project);

            if (!ModelState.IsValid)
            {
                project.ProjectDocuments = await _context.ProjectDocuments
                    .Where(d => d.ProjectId == id).ToListAsync();
                FillFormViewBag();
                return View("CreateEdit", project);
            }

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            if (documents != null && documents.Any())
                await SaveDocumentsAsync(project.Id, documents);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectDocuments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound();

          
            foreach (var doc in project.ProjectDocuments)
                DeletePhysicalFile(doc.FilePath);

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDocument(int docId, int projectId)
        {
            var doc = await _context.ProjectDocuments.FindAsync(docId);
            if (doc != null)
            {
                DeletePhysicalFile(doc.FilePath);
                _context.ProjectDocuments.Remove(doc);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Edit), new { id = projectId });
        }

        public async Task<IActionResult> DownloadDocument(int docId)
        {
            var doc = await _context.ProjectDocuments.FindAsync(docId);
            if (doc == null || string.IsNullOrEmpty(doc.FilePath)) return NotFound();

            var fullPath = GetFullPath(doc.FilePath);
            if (!System.IO.File.Exists(fullPath)) return NotFound();

            var contentType = GetContentType(doc.Extension);
            var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            return File(bytes, contentType, doc.FileName);
        }

        private async Task SaveDocumentsAsync(int projectId, List<IFormFile> files)
        {
            var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads", "projects");
            Directory.CreateDirectory(uploadsDir);

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(ext)) continue;

                var uniqueName = Guid.NewGuid() + ext;
                var fullPath   = Path.Combine(uploadsDir, uniqueName);

                await using (var stream = new FileStream(fullPath, FileMode.Create))
                    await file.CopyToAsync(stream);

                _context.ProjectDocuments.Add(new ProjectDocument
                {
                    ProjectId  = projectId,
                    FileName   = file.FileName,
                    FilePath   = "/uploads/projects/" + uniqueName,
                    Extension  = ext,
                    UploadDate = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
        }

        private void DeletePhysicalFile(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;
            var full = GetFullPath(relativePath);
            if (System.IO.File.Exists(full)) System.IO.File.Delete(full);
        }

        private string GetFullPath(string relativePath) =>
            Path.Combine(_environment.WebRootPath,
                relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        private static string GetContentType(string? ext) => ext?.ToLower() switch
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
        private List<string> GetUserNames() =>
            _context.Users
                .Where(u => u.IsConfirmed && !u.IsBlocked)
                .OrderBy(u => u.FullName)
                .Select(u => u.FullName!)
                .ToList();

        private void ValidateDates(Project p)
        {
            if (p.StartDate.HasValue)
            {
                if (p.PlannedEndDate.HasValue && p.PlannedEndDate < p.StartDate)
                    ModelState.AddModelError(nameof(p.PlannedEndDate),
                        "Плановая дата завершения не может быть раньше даты начала.");

                if (p.ActualEndDate.HasValue && p.ActualEndDate < p.StartDate)
                    ModelState.AddModelError(nameof(p.ActualEndDate),
                        "Фактическая дата завершения не может быть раньше даты начала.");
            }

            if (p.PlannedEndDate.HasValue && p.ActualEndDate.HasValue
                && p.ActualEndDate < p.PlannedEndDate)
            {
                
            }
        }
    }
}
