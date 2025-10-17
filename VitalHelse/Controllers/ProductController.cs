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

    [Route("produkt/{categoryName}")]
    public IActionResult Category(string categoryName)
    {
        // Hent hovedkategori med subkategorier
        var currentCategory = _context.Categories
            .Include(c => c.ChildCategories)
            .Include(c => c.ProductCategories)
            .ThenInclude(pc => pc.Product)
            .ThenInclude(p => p.ProductPictures)
            .Include(c => c.ProductCategories)
            .ThenInclude(pc => pc.Product)
            .ThenInclude(p => p.ProductTags)
            .ThenInclude(pt => pt.Tag)
            .FirstOrDefault(c => c.CategoryName == categoryName);

        if (currentCategory == null) return NotFound();

        // Alle produkter direkte under denne kategorien
        var products = currentCategory.ProductCategories
            .Select(pc => pc.Product)
            .ToList();

        // Subkategorier (child)
        var subcategories = currentCategory.ChildCategories.ToList();

        var viewModel = new CategoryViewModel
        {
            CurrentCategory = currentCategory,
            ParentCategoryName = currentCategory.ParentCategory?.CategoryName,
            SubCategories = subcategories,
            Products = products
        };

        return View("CategoryTemplate", viewModel);
    }



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
