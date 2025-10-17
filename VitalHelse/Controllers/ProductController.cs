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
    
    
[Route("produkt/{category}/{subcategory?}/{subsubcategory?}")]
public IActionResult Category(string category, string? subcategory, string? subsubcategory)
{
    // Hent alle produkter med relasjoner
    var productsQuery = _context.Products
        .Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category)
        .Include(p => p.ProductPictures)
        .Include(p => p.ProductTags)
            .ThenInclude(pt => pt.Tag)
        .AsQueryable();

    var currentCategory = _context.Categories
        .Include(c => c.ChildCategories)
        .FirstOrDefault(c => c.CategoryName == category);


    if (currentCategory == null)
        return NotFound();

    // Filtrer produkter basert på kategori-hierarkiet
    if (!string.IsNullOrEmpty(category))
    {
        productsQuery = productsQuery.Where(p => p.ProductCategories
            .Any(pc => pc.CategoryId == currentCategory.CategoryId ||
                       (pc.Category.ParentCategoryId == currentCategory.CategoryId)));
    }

    if (!string.IsNullOrEmpty(subcategory))
    {
        var subCat = _context.Categories.FirstOrDefault(c => c.CategoryName == subcategory);
        if (subCat != null)
            productsQuery = productsQuery.Where(p => p.ProductCategories
                .Any(pc => pc.CategoryId == subCat.CategoryId));
    }

    if (!string.IsNullOrEmpty(subsubcategory))
    {
        var subSubCat = _context.Categories.FirstOrDefault(c => c.CategoryName == subsubcategory);
        if (subSubCat != null)
            productsQuery = productsQuery.Where(p => p.ProductCategories
                .Any(pc => pc.CategoryId == subSubCat.CategoryId));
    }

    var subcategories = _context.Categories
        .Where(c => c.ParentCategoryId == currentCategory.CategoryId)
        .ToList();

    var viewModel = new CategoryViewModel
    {
        CurrentCategory = currentCategory,
        ParentCategoryName = currentCategory.ParentCategory?.CategoryName, // <- viktig
        SubCategories = subcategories,
        Products = productsQuery.ToList()
    };

    return View("CategoryTemplate", viewModel);
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