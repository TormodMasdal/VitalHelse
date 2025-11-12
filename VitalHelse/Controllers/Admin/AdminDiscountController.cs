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

    public AdminDiscountController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /AdminDiscount
    public async Task<IActionResult> Index(bool showCategories = false)
    {
        var vm = new DiscountViewModel
        {
            Products = await _context.Products
                .Include(p => p.ProductPictures)
                .AsNoTracking()
                .ToListAsync(),

            Categories = await _context.Categories
                .AsNoTracking()
                .ToListAsync(),

            Discounts = await _context.ProductDiscounts
                .AsNoTracking()
                .ToListAsync(),

            ShowCategoryView = showCategories
        };

        ViewData["Title"] = "Kampanjer";
        return View("~/Views/Admin/Discounts.cshtml", vm);
    }


    // POST: /AdminDiscount/UpdateDiscount
    [HttpPost]
    public async Task<IActionResult> UpdateDiscount(int id, double rate, bool isCategory)
    {
        ProductDiscount? discount;

        if (isCategory)
            discount = await _context.ProductDiscounts.FirstOrDefaultAsync(d => d.CategoryId == id);
        else
            discount = await _context.ProductDiscounts.FirstOrDefaultAsync(d => d.ProductId == id);

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