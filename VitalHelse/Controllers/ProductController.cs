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

    [Route("produkt/{*categoryPath}")]
    public IActionResult Category(string? categoryPath)
    {
        if (string.IsNullOrEmpty(categoryPath))
            return NotFound();

        var pathParts = categoryPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var categoryName = pathParts.Last();

        // Finn kategorien basert på hele hierarkiet
        var currentCategory = _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.ChildCategories)
            .AsEnumerable() // nødvendig for å bygge full sti i minnet
            .FirstOrDefault(c =>
            {
                var fullPath = GetCategoryFullPath(c).ToLower();
                return fullPath == categoryPath.ToLower();
            });

        if (currentCategory == null)
            return NotFound();

        // 🔥 Hent ALLE produkter i denne kategorien + ALLE underkategorier (rekursivt)
        var allCategoryIds = GetAllCategoryIds(currentCategory);

        var products = _context.Products
            .Include(p => p.ProductPictures)
            .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
            .Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category)
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

    // 📂 Hjelpemetode for å bygge full sti (f.eks. hudpleie/krem/ansiktskrem)
    private string GetCategoryFullPath(Category category)
    {
        var names = new List<string> { category.CategoryName };
        var parent = category.ParentCategory;
        while (parent != null)
        {
            names.Insert(0, parent.CategoryName);
            parent = parent.ParentCategory;
        }
        return string.Join('/', names);
    }

    // 🔁 Hent ALLE underkategorier rekursivt
    private List<int> GetAllCategoryIds(Category category)
    {
        var ids = new List<int> { category.CategoryId };

        var children = _context.Categories
            .Where(c => c.ParentCategoryId == category.CategoryId)
            .ToList();

        foreach (var child in children)
        {
            ids.AddRange(GetAllCategoryIds(child));
        }

        return ids;
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

        if (product == null)
            return NotFound();

        return View("ProductDetails", product);
    }
}
