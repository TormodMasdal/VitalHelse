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
        var currentCategory = _context.Categories
            .Include(c => c.ChildCategories)
            .ThenInclude(c => c.ChildCategories)
            .Include(c => c.ProductCategories)
            .FirstOrDefault(c => c.CategoryName == categoryName);

        if (currentCategory == null)
            return NotFound();

// Hent alle ID-er nedover i hierarkiet
        var allCategoryIds = GetAllCategoryIds(currentCategory);

        var products = _context.Products
            .Include(p => p.ProductPictures)
            .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
            .Include(p => p.ProductCategories)
            .Where(p => p.ProductCategories.Any(pc => allCategoryIds.Contains(pc.CategoryId)))
            .ToList();

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


// Rekursiv metode for å hente alle produkter fra kategori + underkategorier
    private List<int> GetAllProductIds(Category category)
    {
        var ids = category.ProductCategories.Select(pc => pc.ProductId).ToList();
        foreach (var child in category.ChildCategories)
            ids.AddRange(GetAllProductIds(child));
        return ids;
    }
    
    private List<int> GetAllCategoryIds(Category category)
    {
        var ids = new List<int> { category.CategoryId };

        if (category.ChildCategories != null && category.ChildCategories.Any())
        {
            foreach (var child in category.ChildCategories)
            {
                ids.AddRange(GetAllCategoryIds(child));
            }
        }

        return ids;
    }



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

    // Finn kategori basert på hierarki
    Category currentCategory = null;

    if (!string.IsNullOrEmpty(subsubcategory))
    {
        currentCategory = _context.Categories
            .Include(c => c.ChildCategories)
            .Include(c => c.ParentCategory)
            .FirstOrDefault(c => c.CategoryName == subsubcategory);
    }
    else if (!string.IsNullOrEmpty(subcategory))
    {
        currentCategory = _context.Categories
            .Include(c => c.ChildCategories)
            .Include(c => c.ParentCategory)
            .FirstOrDefault(c => c.CategoryName == subcategory);
    }
    else
    {
        currentCategory = _context.Categories
            .Include(c => c.ChildCategories)
            .Include(c => c.ParentCategory)
            .FirstOrDefault(c => c.CategoryName == category);
    }

    if (currentCategory == null) return NotFound();

    // Hent alle produkter under denne kategorien inkl. child categories
    productsQuery = productsQuery.Where(p =>
        p.ProductCategories.Any(pc => IsCategoryInPath(pc.Category, currentCategory)));

    var products = productsQuery.ToList();

    // Hent child categories for subkategori-bar
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

// Hjelpefunksjon for å sjekke om produktets kategori er under currentCategory
private bool IsCategoryInPath(Category productCategory, Category currentCategory)
{
    var cat = productCategory;
    while (cat != null)
    {
        if (cat.CategoryId == currentCategory.CategoryId)
            return true;
        cat = cat.ParentCategory;
    }
    return false;
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
