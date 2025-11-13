using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalHelse.Data;
using VitalHelse.Models;

namespace VitalHelse.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Discounts")]
    public class AdminDiscountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminDiscountController(ApplicationDbContext context) => _context = context;

        [HttpGet("")]
        public async Task<IActionResult> Index(List<int>? categoryIds, int page = 1, int pageSize = 25)
        {
            var query = _context.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .AsQueryable();

            if (categoryIds != null && categoryIds.Any())
            {
                query = query.Where(p => p.ProductCategories
                    .Any(pc => categoryIds.Contains(pc.CategoryId)));
            }

            var totalProducts = await query.CountAsync();

            var products = await query
                .OrderBy(p => p.ProductName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vm = new DiscountViewModel
            {
                Products = products,
                Categories = await _context.Categories.ToListAsync(),
                Discounts = await _context.ProductDiscounts.ToListAsync(),
                SelectedCategoryIds = categoryIds ?? new(),
                TotalProducts = totalProducts,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize)
            };

            return View("~/Views/Admin/Discounts.cshtml", vm);
        }



        // ===== SAVE MANY =====
        public record DiscountUpdateDto(int Id, double Rate, bool IsCategory);

        [HttpPost("UpdateMany")]
        public async Task<IActionResult> UpdateMany([FromBody] List<DiscountUpdateDto> updates)
        {
            foreach (var u in updates)
            {
                if (u.IsCategory)
                {
                    var cat = await _context.ProductDiscounts
                        .FirstOrDefaultAsync(d => d.CategoryId == u.Id && d.IsCategoryDiscount);

                    if (cat == null)
                    {
                        _context.ProductDiscounts.Add(new ProductDiscount
                        {
                            CategoryId = u.Id,
                            IsCategoryDiscount = true,
                            DiscountRate = u.Rate,
                            IsActive = u.Rate > 0
                        });
                    }
                    else
                    {
                        cat.DiscountRate = u.Rate;
                        cat.IsActive = u.Rate > 0;
                    }

                    continue;
                }

                var prod = await _context.ProductDiscounts
                    .FirstOrDefaultAsync(d => d.ProductId == u.Id && !d.IsCategoryDiscount);

                if (prod == null)
                {
                    _context.ProductDiscounts.Add(new ProductDiscount
                    {
                        ProductId = u.Id,
                        IsCategoryDiscount = false,
                        DiscountRate = u.Rate,
                        IsActive = u.Rate > 0
                    });
                }
                else
                {
                    prod.DiscountRate = u.Rate;
                    prod.IsActive = u.Rate > 0;
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
