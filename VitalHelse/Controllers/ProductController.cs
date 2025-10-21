using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalHelse.Data;
using VitalHelse.Helpers;
using VitalHelse.Models;

namespace VitalHelse.Controllers;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<ProductController> _logger;

    public ProductController(ApplicationDbContext db, ILogger<ProductController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // Viser et spesifikt produkt via ID (fallback / direkte lenke)
    public IActionResult Index(int? id)
    {
        if (id == null)
            return RedirectToAction("Index", "Home");

        var product = _db.Products
            .Include(p => p.ProductPictures)
            .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                    .ThenInclude(c => c.ParentCategory)
            .FirstOrDefault(p => p.ProductId == id);

        if (product == null)
            return NotFound();

        return View("ProductDetails", product);
    }

    // Hovedrute som håndterer både kategori- og produkt-URL-er
    [Route("produkt/{*fullPath}")]
    public IActionResult ProductOrCategory(string? fullPath)
    {
        if (string.IsNullOrEmpty(fullPath))
            return NotFound();

        var parts = fullPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var lastPart = parts.Last().ToLowerInvariant();

        // 🔹 Sjekk om siste del matcher et produkt (via slug)
        var product = _db.Products
            .Include(p => p.ProductPictures)
            .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                    .ThenInclude(c => c.ParentCategory)
            .AsEnumerable() // kreves for SlugHelper i minne
            .FirstOrDefault(p => SlugHelper.Slugify(p.ProductName) == lastPart);

        if (product != null)
            return View("ProductDetails", product);

        // 🔹 Ellers: behandle som kategori
        var currentCategory = _db.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.ChildCategories)
            .AsEnumerable()
            .FirstOrDefault(c =>
                SlugHelper.Slugify(GetCategoryFullPath(c)) == SlugHelper.Slugify(fullPath)
            );

        if (currentCategory == null)
            return NotFound();

        var allCategoryIds = GetAllCategoryIds(currentCategory);

        var products = _db.Products
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

    // 📂 Lager full kategori-sti som "hudpleie/krem/ansiktskrem"
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

    // 🔁 Henter alle underkategorier rekursivt
    private List<int> GetAllCategoryIds(Category category)
    {
        var ids = new List<int> { category.CategoryId };

        var children = _db.Categories
            .Where(c => c.ParentCategoryId == category.CategoryId)
            .ToList();

        foreach (var child in children)
            ids.AddRange(GetAllCategoryIds(child));

        return ids;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
