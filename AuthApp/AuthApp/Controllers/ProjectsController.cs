using AuthApp.Data;
using AuthApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
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

        
        [HttpGet]
        public IActionResult SearchContracts(string? q, int projectId)
        {
            
            var attachedIds = _context.ProjectContracts
                .Where(pc => pc.ProjectId == projectId)
                .Select(pc => pc.ContractId)
                .ToHashSet();

            var items = _context.Contracts
                .Where(c =>
                    !attachedIds.Contains(c.Id) &&
                    (string.IsNullOrEmpty(q) ||
                     c.ContractNumber!.Contains(q) ||
                     (c.FullName != null && c.FullName.Contains(q))))
                .OrderByDescending(c => c.ConclusionDate)
                .Take(20)
                .Select(c => new {
                    c.Id,
                    c.ContractNumber,
                    subject = c.FullName,
                    signDate = c.ConclusionDate.HasValue
                        ? c.ConclusionDate.Value.ToString("dd.MM.yyyy")
                        : ""
                })
                .ToList();

            return Json(items);
        }

        public async Task<IActionResult> Index(string? search, string? status, int page = 1, int pageSize = 10)
        {
            var query = _context.Projects
                .Include(p => p.ProjectContracts)
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
        public async Task<IActionResult> Create(Project project, [FromForm] List<int> newContractIds)
        {
            ValidateDates(project);

            if (!ModelState.IsValid)
            {
                FillFormViewBag();
                return View("CreateEdit", project);
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            if (newContractIds != null && newContractIds.Count > 0)
            {
                foreach (var cid in newContractIds.Distinct())
                {
                    _context.ProjectContracts.Add(new ProjectContract
                    {
                        ProjectId  = project.Id,
                        ContractId = cid,
                        AttachedAt = DateTime.Now
                    });
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectContracts)
                    .ThenInclude(pc => pc.Contract)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound();
            FillFormViewBag();
            return View("CreateEdit", project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.Id) return BadRequest();

            ValidateDates(project);

            if (!ModelState.IsValid)
            {
                project.ProjectContracts = await _context.ProjectContracts
                    .Include(pc => pc.Contract)
                    .Where(pc => pc.ProjectId == id)
                    .ToListAsync();
                FillFormViewBag();
                return View("CreateEdit", project);
            }

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectContracts)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AttachContract(int projectId, int contractId)
        {
            var alreadyAttached = await _context.ProjectContracts
                .AnyAsync(pc => pc.ProjectId == projectId && pc.ContractId == contractId);

            if (!alreadyAttached)
            {
                _context.ProjectContracts.Add(new ProjectContract
                {
                    ProjectId  = projectId,
                    ContractId = contractId,
                    AttachedAt = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), new { id = projectId });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetachContract(int projectContractId, int projectId)
        {
            var link = await _context.ProjectContracts.FindAsync(projectContractId);
            if (link != null)
            {
                _context.ProjectContracts.Remove(link);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Edit), new { id = projectId });
        }

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
        }
    }
}
