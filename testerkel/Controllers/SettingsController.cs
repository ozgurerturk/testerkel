using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using testerkel.Data;
using testerkel.Models;

public class SettingsController : Controller
{
    private readonly ErkelErpDbContext _context;

    public SettingsController(ErkelErpDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        //return View("/Views/Settings/Index.cshtml");
        return View();
    }

    // GET: /Settings/Units
    [HttpGet]
    public async Task<IActionResult> Units()
    {
        var items = await _context.UnitAliases
            .AsNoTracking()
            .OrderBy(x => x.UnitType)
            .ThenBy(x => x.Alias)
            .ToListAsync();

        ViewBag.UnitTypes = GetUnitTypeSelectList();
        return View("/Views/Settings/Units.cshtml", items);
    }

    // POST: /Settings/CreateUnitAlias
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUnitAlias(UnitAlias model)
    {
        model.Alias = (model.Alias ?? "").Trim();

        if (string.IsNullOrWhiteSpace(model.Alias))
            ModelState.AddModelError(nameof(model.Alias), "Alias zorunludur.");

        var exists = await _context.UnitAliases
            .AnyAsync(x => x.Alias.ToLower() == model.Alias.ToLower());

        if (exists)
            ModelState.AddModelError(nameof(model.Alias), "Bu alias zaten var.");

        if (!ModelState.IsValid)
        {
            var items = await _context.UnitAliases
                .AsNoTracking()
                .OrderBy(x => x.UnitType)
                .ThenBy(x => x.Alias)
                .ToListAsync();

            ViewBag.UnitTypes = GetUnitTypeSelectList();
            return View("/Views/Settings/Units.cshtml", items);
        }

        _context.UnitAliases.Add(model);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Birim alias eklendi.";
        return RedirectToAction(nameof(Units));
    }

    // GET: /Settings/EditUnitAlias/5
    [HttpGet]
    public async Task<IActionResult> EditUnitAlias(int id)
    {
        var item = await _context.UnitAliases.FindAsync(id);
        if (item == null) return NotFound();

        ViewBag.UnitTypes = GetUnitTypeSelectList();
        return View("/Views/Settings/EditUnitAlias.cshtml", item);
    }

    // POST: /Settings/EditUnitAlias
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUnitAlias(UnitAlias model)
    {
        model.Alias = (model.Alias ?? "").Trim();

        if (string.IsNullOrWhiteSpace(model.Alias))
            ModelState.AddModelError(nameof(model.Alias), "Alias zorunludur.");

        var exists = await _context.UnitAliases.AnyAsync(x =>
            x.Id != model.Id && x.Alias.ToLower() == model.Alias.ToLower());

        if (exists)
            ModelState.AddModelError(nameof(model.Alias), "Bu alias zaten var.");

        if (!ModelState.IsValid)
        {
            ViewBag.UnitTypes = GetUnitTypeSelectList();
            return View("/Views/Settings/EditUnitAlias.cshtml", model);
        }

        _context.UnitAliases.Update(model);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Birim alias güncellendi.";
        return RedirectToAction(nameof(Units));
    }

    // POST: /Settings/DeleteUnitAlias
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUnitAlias(int id)
    {
        var item = await _context.UnitAliases.FindAsync(id);
        if (item == null) return RedirectToAction(nameof(Units));

        _context.UnitAliases.Remove(item);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Birim alias silindi.";
        return RedirectToAction(nameof(Units));
    }

    private static List<SelectListItem> GetUnitTypeSelectList()
    {
        return Enum.GetValues<UnitType>()
            .Select(x => new SelectListItem
            {
                Value = ((byte)x).ToString(),
                Text = x.ToString()
            })
            .ToList();
    }
}
