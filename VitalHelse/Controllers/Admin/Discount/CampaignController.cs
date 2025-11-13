using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalHelse.Data;
using VitalHelse.Models.Discount;

namespace VitalHelse.Controllers.Admin.Discount;

[Authorize(Roles = "Admin,Staff")]
public class CampaignController : Controller
{
    private readonly ApplicationDbContext _context;

    public CampaignController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("Admin/Campaigns")]
    public IActionResult Index()
    {
        var campaigns = _context.Campaigns
            .OrderByDescending(c => c.Start)
            .ToList();

        // All categories for the multiselect
        var categories = _context.Categories
            .Where(c => c.ParentCategoryId == null)
            .ToList();

        ViewBag.AllCategories = categories;

        // Create lookup so we can show category names in the table
        ViewBag.CategoryLookup = campaigns.ToDictionary(
            camp => camp.Id,
            camp => _context.Categories
                .Where(cat => camp.CategoryIds.Contains(cat.CategoryId))
                .Select(cat => cat.CategoryName)
                .ToList()
        );

        return View("Campaign", campaigns);
    }
    
    [HttpPost("Admin/Campaigns/Create")]
    public IActionResult Create(string Name, DateTime Start, DateTime End, List<int> CategoryIds)
    {
        var camp = new Campaign
        {
            Name = Name,
            Start = Start,
            End = End,
            CategoryIds = CategoryIds ?? new List<int>(),
            IsActive = true
        };

        _context.Campaigns.Add(camp);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
    
    [HttpPost("Admin/Campaigns/Toggle")]
    public IActionResult Toggle(int id)
    {
        var c = _context.Campaigns.Find(id);
        if (c == null) return NotFound();

        c.IsActive = !c.IsActive;
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
    
    [HttpPost("Admin/Campaigns/Delete")]
    public IActionResult Delete(int id)
    {
        var camp = _context.Campaigns.Find(id);
        if (camp == null) return NotFound();

        _context.Campaigns.Remove(camp);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

}