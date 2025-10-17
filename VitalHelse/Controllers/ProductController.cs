using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalHelse.Data;
using VitalHelse.Models;

namespace VitalHelse.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db, ILogger<ProductController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [Route("produkt/{category}/{subcategory?}/{subsubcategory?}")]
        public IActionResult Category(string category, string? subcategory, string? subsubcategory)
        {
            var products = _db.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.ProductPictures)
                .Include(p => p.ProductTags)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
                products = products.Where(p => p.ProductCategories.Any(c => c.CategoryName == category));

            if (!string.IsNullOrEmpty(subcategory))
                products = products.Where(p => p.ProductCategories.Any(c => c.SubCategoryName == subcategory));

            if (!string.IsNullOrEmpty(subsubcategory))
                products = products.Where(p => p.ProductCategories.Any(c => c.SubSubCategoryName == subsubcategory));

            return View("CategoryTemplate", products.ToList());
        }
    }
}