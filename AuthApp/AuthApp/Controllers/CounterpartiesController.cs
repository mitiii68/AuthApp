using AuthApp.Data;
using AuthApp.Enums;
using AuthApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Controllers;

public class CounterpartiesController : Controller
{
    private readonly AppDbContext _db;

    public CounterpartiesController(AppDbContext db) => _db = db;

    
    private void FillViewBag()
    {
        ViewBag.TypeList = new SelectList(new[]
        {
            new { Value = (int)CounterpartyType.Competitor,  Text = "Конкурент" },
            new { Value = (int)CounterpartyType.Client,      Text = "Клиент" },
            new { Value = (int)CounterpartyType.Integrator,  Text = "Интегратор" },
            new { Value = (int)CounterpartyType.Partner,     Text = "Партнёр" },
            new { Value = (int)CounterpartyType.Press,       Text = "Пресса" },
            new { Value = (int)CounterpartyType.Other,       Text = "Другое" },
        }, "Value", "Text");

        ViewBag.IndustryList = new SelectList(new[]
        {
            new { Value = (int)Industry.NonFerrousMetallurgy, Text = "Цветная металлургия" },
            new { Value = (int)Industry.Communications,       Text = "Коммуникации" },
            new { Value = (int)Industry.Construction,         Text = "Строительство" },
            new { Value = (int)Industry.Consulting,           Text = "Консалтинг" },
            new { Value = (int)Industry.Education,            Text = "Образование" },
            new { Value = (int)Industry.ElectricPower,        Text = "Электроэнергетика" },
            new { Value = (int)Industry.Entertainment,        Text = "Развлечения" },
        }, "Value", "Text");

        ViewBag.OrganizationTypeList = new SelectList(new[]
        {
            new { Value = (int)OrganizationType.StateInstitution, Text = "Государственное учреждение" },
            new { Value = (int)OrganizationType.Business,         Text = "Бизнес" },
        }, "Value", "Text");
    }

    
    public async Task<IActionResult> Index()
    {
        var list = await _db.Counterparties.ToListAsync();
        return View(list);
    }

    
    public IActionResult Create()
    {
        FillViewBag();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Counterparty model)
    {
        if (!ModelState.IsValid)
        {
            FillViewBag();
            return View(model);
        }
        _db.Counterparties.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.Counterparties.FindAsync(id);
        if (item is null) return NotFound();
        FillViewBag();
        return View(item);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Counterparty model)
    {
        if (!ModelState.IsValid)
        {
            FillViewBag();
            return View(model);
        }
        _db.Counterparties.Update(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

   
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Counterparties.FindAsync(id);
        if (item is null) return NotFound();
        _db.Counterparties.Remove(item);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}