using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalHelse.Data;
using VitalHelse.Models;

namespace VitalHelse.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Discounts")]
public class AdminDiscountController : Controller
{
    private readonly ApplicationDbContext _context;
    public AdminDiscountController(ApplicationDbContext context) => _context = context;

    [HttpGet("")]
    public async Task<IActionResult> Index(bool showCategories = false)
    {
        var vm = new DiscountViewModel
        {
            Products = await _context.Products.Include(p => p.ProductPictures).ToListAsync(),
            Categories = await _context.Categories.ToListAsync(),
            Discounts = await _context.ProductDiscounts.ToListAsync(),
            ShowCategoryView = showCategories
        };

        ViewData["Title"] = "Kampanjer";
        return View("~/Views/Admin/Discounts.cshtml", vm);
    }

    [HttpPost("UpdateDiscount")]
    public async Task<IActionResult> UpdateDiscount(int id, double rate, bool isCategory)
    {
        ProductDiscount? discount = isCategory
            ? await _context.ProductDiscounts.FirstOrDefaultAsync(d => d.CategoryId == id)
            : await _context.ProductDiscounts.FirstOrDefaultAsync(d => d.ProductId == id);

        if (discount == null)
        {
            discount = new ProductDiscount
            {
                DiscountRate = rate,
                IsCategoryDiscount = isCategory,
                ProductId = isCategory ? null : id,
                CategoryId = isCategory ? id : null
            };
            _context.ProductDiscounts.Add(discount);
        }
        else
        {
            discount.DiscountRate = rate;
        }

        await _context.SaveChangesAsync();
        return Ok(new { success = true });
    }
}