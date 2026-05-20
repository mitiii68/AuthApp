using AuthApp.Data;
using AuthApp.Enums;
using AuthApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Controllers
{
    public class ContractsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ContractsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
        }

        private async Task<bool> IsContractLockedAsync(int contractId)
        {
            var docs = await _context.ContractDocument
                .Where(cd => cd.ContractId == contractId)
                .ToListAsync();

            return docs.Count > 0 && docs.All(d => d.ApprovalStatus == DocumentApprovalStatus.Approved);
        }

        private string CurrentUserEmail()
            => HttpContext.Session.GetString("UserEmail") ?? "неизвестный";

        private async Task LogActionAsync(string action)
        {
            _context.UserActionLog.Add(new UserActionLog
            {
                UserEmail = CurrentUserEmail(),
                Action = action,
                ActionTime = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }

        private void FillViewBag()
        {
            ViewBag.Stages = Enum.GetValues<ContractStage>();
            ViewBag.Types = Enum.GetValues<ContractType>();
        }


        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin" && role != "User") return RedirectToAction("Index", "Home");

            var query = _context.Contracts
                .Include(c => c.Counterparty)
                .Include(c => c.ResponsibleUser)
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c =>
                    (c.FullName != null && c.FullName.Contains(search)) ||
                    (c.ContractNumber != null && c.ContractNumber.Contains(search)));

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Clamp(page, 1, Math.Max(1, totalPages));

            var contracts = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalItems;
            ViewBag.Search = search;
            ViewBag.PageSize = pageSize;

            return View(contracts);
        }


        public async Task<IActionResult> Create()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            FillViewBag();
            ViewBag.Counterparties = await _context.Counterparties.ToListAsync();
            ViewBag.Users = await _context.Users.ToListAsync();

            ViewBag.SelectedDocuments = new List<int>();

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Contract contract, List<int> participantIds, string amountRaw, List<int> documentIds)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            if (string.IsNullOrWhiteSpace(contract.FullName))
                ModelState.AddModelError("FullName", "Полное наименование обязательно.");
            if (string.IsNullOrWhiteSpace(contract.ShortName))
                ModelState.AddModelError("ShortName", "Краткое наименование обязательно.");
            if (string.IsNullOrWhiteSpace(contract.ContractNumber))
                ModelState.AddModelError("ContractNumber", "Номер договора обязателен.");
            if (contract.CounterpartyId == null || contract.CounterpartyId == 0)
                ModelState.AddModelError("CounterpartyId", "Контрагент обязателен.");
            if (contract.ResponsibleUserId == null || contract.ResponsibleUserId == 0)
                ModelState.AddModelError("ResponsibleUserId", "Ответственный обязателен.");
            if (string.IsNullOrWhiteSpace(amountRaw))
                ModelState.AddModelError("amountRaw", "Сумма с НДС обязательна.");
            if (contract.ConclusionDate == null)
                ModelState.AddModelError("ConclusionDate", "Дата заключения обязательна.");
            if (contract.ExecutionStartDate == null)
                ModelState.AddModelError("ExecutionStartDate", "Дата начала исполнения обязательна.");
            if (contract.ClosingDate == null)
                ModelState.AddModelError("ClosingDate", "Дата закрытия обязательна.");
            if (contract.ConclusionDate.HasValue && contract.ConclusionDate.Value.Date > DateTime.Today)
                ModelState.AddModelError("ConclusionDate", "Дата заключения не может быть в будущем.");
            if (contract.ConclusionDate.HasValue && contract.ExecutionStartDate.HasValue
                && contract.ExecutionStartDate < contract.ConclusionDate)
                ModelState.AddModelError("ExecutionStartDate", "Дата начала исполнения не может быть раньше даты заключения.");
            if (contract.ExecutionStartDate.HasValue && contract.ClosingDate.HasValue
                && contract.ClosingDate < contract.ExecutionStartDate)
                ModelState.AddModelError("ClosingDate", "Дата закрытия не может быть раньше даты начала исполнения.");
            if (string.IsNullOrWhiteSpace(contract.ResponsibleFromCustomer))
                ModelState.AddModelError("ResponsibleFromCustomer", "Ответственный со стороны заказчика обязателен.");
            if (participantIds == null || participantIds.Count == 0)
                ModelState.AddModelError("participantIds", "Выберите хотя бы одного участника.");
            if (documentIds == null || documentIds.Count == 0)
                ModelState.AddModelError("documentIds", "Прикрепите хотя бы один документ.");

            if (!ModelState.IsValid)
            {
                FillViewBag();
                ViewBag.Counterparties = await _context.Counterparties.ToListAsync();
                ViewBag.Users = await _context.Users.ToListAsync();
                ViewBag.SelectedDocuments = documentIds ?? new List<int>();
                return View(contract);
            }

            contract.CreatedAt = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(amountRaw))
            {
                var normalized = amountRaw.Replace(",", ".");
                if (decimal.TryParse(normalized, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var amount))
                    contract.AmountWithVat = amount;
            }

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            foreach (var userId in participantIds)
            {
                _context.ContractParticipants.Add(new ContractParticipant
                {
                    ContractId = contract.Id,
                    UserId = userId
                });
            }

            foreach (var docId in documentIds)
            {
                _context.ContractDocument.Add(new ContractDocument
                {
                    ContractId     = contract.Id,
                    FileDocumentId = docId
                });
            }

            await _context.SaveChangesAsync();

            await LogActionAsync($"Создал договор «{contract.FullName}»");

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            if (await IsContractLockedAsync(id))
            {
                TempData["Error"] = "Редактирование заблокировано: все документы договора согласованы.";
                return RedirectToAction("Index");
            }

            var contract = await _context.Contracts
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (contract == null) return NotFound();

            FillViewBag();
            ViewBag.Counterparties = await _context.Counterparties.ToListAsync();
            ViewBag.Users = await _context.Users.ToListAsync();
            ViewBag.SelectedParticipants = contract.Participants.Select(p => p.UserId).ToList();

            ViewBag.SelectedDocuments = await _context.ContractDocument
                 .Where(cd => cd.ContractId == id)
                 .Select(cd => cd.FileDocumentId)
                 .ToListAsync();
            return View(contract);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Contract contract, List<int> participantIds, string amountRaw, List<int> documentIds)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            if (await IsContractLockedAsync(id))
            {
                TempData["Error"] = "Редактирование заблокировано: все документы договора согласованы.";
                return RedirectToAction("Index");
            }

            var existing = await _context.Contracts
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existing == null) return NotFound();

            existing.FullName = contract.FullName;
            existing.ShortName = contract.ShortName;
            existing.CounterpartyId = contract.CounterpartyId;
            existing.Stage = contract.Stage;
            existing.Type = contract.Type;
            existing.ResponsibleFromCustomer = contract.ResponsibleFromCustomer;
            existing.ResponsibleUserId = contract.ResponsibleUserId;
            existing.ConclusionDate = contract.ConclusionDate;
            existing.ClosingDate = contract.ClosingDate;
            existing.ExecutionStartDate = contract.ExecutionStartDate;
            existing.ContractNumber = contract.ContractNumber;
            existing.SourceContractId = contract.SourceContractId;
            existing.ProjectId = contract.ProjectId;

            if (!string.IsNullOrWhiteSpace(amountRaw))
            {
                var normalized = amountRaw.Replace(",", ".");
                if (decimal.TryParse(normalized, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var amount))
                    existing.AmountWithVat = amount;
            }
            else
            {
                existing.AmountWithVat = null;
            }

            var existingUserIds = existing.Participants.Select(p => p.UserId).ToHashSet();
            var newUserIds = participantIds.ToHashSet();
            var toRemove = existing.Participants
                .Where(p => !newUserIds.Contains(p.UserId))
                .ToList();
            _context.ContractParticipants.RemoveRange(toRemove);

            foreach (var userId in newUserIds.Except(existingUserIds))
            {
                _context.ContractParticipants.Add(new ContractParticipant
                {
                    ContractId = id,
                    UserId = userId
                });
            }

            var oldDocs = await _context.ContractDocument
                .Where(cd => cd.ContractId == id)
                .ToListAsync();

            var docsToRemove = oldDocs
                .Where(d => !documentIds.Contains(d.FileDocumentId ?? 0)
                    && d.ApprovalStatus != DocumentApprovalStatus.Approved
                    && d.ApprovalStatus != DocumentApprovalStatus.InApproval)
                .ToList();
            _context.ContractDocument.RemoveRange(docsToRemove);

            var existingFileIds = oldDocs.Select(d => d.FileDocumentId).ToHashSet();
            foreach (var docId in documentIds)
            {
                if (!existingFileIds.Contains(docId))
                {
                    _context.ContractDocument.Add(new ContractDocument
                    {
                        ContractId     = id,
                        FileDocumentId = docId
                    });
                }
            }

            await _context.SaveChangesAsync();
            await LogActionAsync($"Отредактировал договор «{existing.FullName}»");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var contract = await _context.Contracts
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contract != null)
            {
                _context.ContractParticipants.RemoveRange(contract.Participants);
                _context.Contracts.Remove(contract);
                await _context.SaveChangesAsync();
                await LogActionAsync($"Удалил договор «{contract.FullName}»");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetContractFiles()
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == "Договор");
            if (tag == null) return Json(new List<object>());

            var files = await _context.FileDocuments
                .Where(d => !d.IsDeleted && d.FileTags.Any(ft => ft.TagId == tag.Id))
                .Select(d => new { id = d.Id, name = d.FileName, ext = d.Extension })
                .ToListAsync();

            return Json(files);
        }



        [HttpPost]
        public async Task<IActionResult> UploadContractFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "Файл не выбран" });

            var allowedExtensions = new[] { ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".zip", ".rar", ".7z" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return Json(new { success = false, message = "Недопустимый формат файла" });

            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var uniqueFileName = Guid.NewGuid() + extension;
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            var document = new FileDocuments
            {
                FileName = file.FileName,
                FilePath = "/uploads/" + uniqueFileName,
                Extension = extension,
                UploadDate = DateTime.Now
            };

            _context.FileDocuments.Add(document);
            await _context.SaveChangesAsync();

           
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == "Договор");
            if (tag != null)
            {
                _context.FileTags.Add(new FileTag
                {
                    FileDocumentsId = document.Id,
                    TagId = tag.Id
                });
                await _context.SaveChangesAsync();
            }

            await LogActionAsync($"Загрузил документ договора «{file.FileName}»");

            return Json(new { success = true, id = document.Id, name = document.FileName, ext = document.Extension });

        }

        public async Task<IActionResult> ApprovalHistory(int id)
        {
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contract == null) return NotFound();

            ViewBag.ContractId = id;
            ViewBag.ContractName = contract.FullName ?? contract.ShortName ?? $"Договор #{id}";
            return View();
        }
    }
}