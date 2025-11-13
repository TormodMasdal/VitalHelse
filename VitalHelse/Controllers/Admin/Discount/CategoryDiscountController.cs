using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalHelse.Data;
using VitalHelse.Models.Discount;

namespace VitalHelse.Controllers.Admin.Discount;

[Authorize(Roles = "Admin,Staff")]
public class CategoryDiscountController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoryDiscountController(ApplicationDbContext context)
    {
        _context = context;
    }

    public class CategoryDiscountViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int DiscountPercent { get; set; }
    }

    [HttpGet("Admin/Rabattsatser")]
    public IActionResult Index()
    {
        var topCategories = _context.Categories
            .Where(c => c.ParentCategoryId == null)
            .ToList();

        var dbDiscounts = _context.CategoryDiscounts.ToList();

        var model = topCategories.Select(c => new CategoryDiscountViewModel
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            DiscountPercent = dbDiscounts.FirstOrDefault(d => d.CategoryId == c.CategoryId)?.DiscountPercent ?? 0
        }).ToList();

        return View("CategoryDiscount", model);
    }
    
    [HttpPost("Admin/CategoryDiscounts/SaveAll")]
    public IActionResult SaveAll(List<CategoryDiscountViewModel> model)
    {
        foreach (var item in model)
        {
            var existing = _context.CategoryDiscounts
                .FirstOrDefault(cd => cd.CategoryId == item.CategoryId);

            if (existing == null)
            {
                // Create new discount entry
                _context.CategoryDiscounts.Add(new Models.Discount.CategoryDiscount
                {
                    CategoryId = item.CategoryId,
                    DiscountPercent = item.DiscountPercent
                });
            }
            else
            {
                // Update existing
                existing.DiscountPercent = item.DiscountPercent;
            }
        }

        _context.SaveChanges();
        return RedirectToAction("Index");
    }

}