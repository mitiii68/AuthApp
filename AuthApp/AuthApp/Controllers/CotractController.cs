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

        public ContractsController(AppDbContext context)
        {
            _context = context;
        }


        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
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
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

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

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Contract contract, List<int> participantIds, string amountRaw)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

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
            await _context.SaveChangesAsync();

            await LogActionAsync($"Создал договор «{contract.FullName}»");

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var contract = await _context.Contracts
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contract == null) return NotFound();

            FillViewBag();
            ViewBag.Counterparties = await _context.Counterparties.ToListAsync();
            ViewBag.Users = await _context.Users.ToListAsync();
            ViewBag.SelectedParticipants = contract.Participants.Select(p => p.UserId).ToList();

            return View(contract);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Contract contract, List<int> participantIds, string amountRaw)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

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

            _context.ContractParticipants.RemoveRange(existing.Participants);
            foreach (var userId in participantIds)
            {
                _context.ContractParticipants.Add(new ContractParticipant
                {
                    ContractId = id,
                    UserId = userId
                });
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
    }
}