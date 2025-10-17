using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalHelse.Data;
using VitalHelse.Models;

namespace VitalHelse.Controllers;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductController> _logger;

    public ProductController(ApplicationDbContext context, ILogger<ProductController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Route med kategori, subkategori og subsubkategori
    [Route("produkt/{category}/{subcategory?}/{subsubcategory?}")]
    public IActionResult Category(string category, string? subcategory, string? subsubcategory)
    {
        // Hent produkter med relasjoner
        var products = _context.Products
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.ProductPictures)
            .Include(p => p.ProductTags)
                .ThenInclude(pt => pt.Tag)
            .AsQueryable();

        // Filtrer på top-level kategori
        if (!string.IsNullOrEmpty(category))
        {
            products = products.Where(p => p.ProductCategories
                .Any(pc => pc.Category.CategoryName == category ||
                           (pc.Category.ParentCategory != null && pc.Category.ParentCategory.CategoryName == category)));
        }

        // Filtrer på subkategori
        if (!string.IsNullOrEmpty(subcategory))
        {
            products = products.Where(p => p.ProductCategories
                .Any(pc => pc.Category.CategoryName == subcategory));
        }

        // Filtrer på subsubkategori
        if (!string.IsNullOrEmpty(subsubcategory))
        {
            products = products.Where(p => p.ProductCategories
                .Any(pc => pc.Category.CategoryName == subsubcategory));
        }

        var productList = products.ToList();

        if (!productList.Any())
        {
            ViewBag.Message = "Ingen produkter funnet i denne kategorien.";
        }

        return View("CategoryTemplate", productList);
    }

    // Eksempel på detaljside for ett produkt
    [Route("produkt/detaljer/{id}")]
    public IActionResult Details(int id)
    {
        var product = _context.Products
            .Include(p => p.ProductPictures)
            .Include(p => p.ProductTags)
                .ThenInclude(pt => pt.Tag)
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                    .ThenInclude(c => c.ParentCategory)
            .FirstOrDefault(p => p.ProductId == id);

        if (product == null) return NotFound();

        return View("ProductDetails", product);
    }
}