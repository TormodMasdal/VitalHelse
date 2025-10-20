using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalHelse.Data;
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
    /*
    public IActionResult Index(int? id) 
    {
        if (id == null)
        {
            return RedirectToAction("Index","Home");
        }

        var products = _db.Products
            .Include(p => p.ProductPictures)
            .FirstOrDefault(p => p.ProductId == id); 

        if (products == null) 
            return NotFound();
        
        return View(products);
    } 
    */
    public IActionResult Index(int? id)
    {
        // Oppretter dummy produkt
        var product = new VitalHelse.Models.Product
        {
            ProductId = 1,
            ProductName = "AloeV Hyaluronic Acid Night Cream",
            ProductPrice = 199.89,
            ProductDescription = "AloeV nattkrem inneholder Aloe Vera, Hyaluronic Acid, Glycosaminoglycans, Grønn te,\n\nRetinol (vitamin A), Vitamin E, Panthenol, og har et høyt innhold av aktive lipider.",
            ProductIngredients = "Aloe Barbadensis, Aqua, Carbomer, C12-15 Alkyl Benzoate, Glycolic Acid, Camellia Sinensis, Caprylic/Capric Triglyceride, Cetearyl Alcohol, Sorbitol,Cyclomethicone, Ceteareth 20, Glyceryl Stearate, Echinacea Angustifolia, PEG-100 Stearate, Cetyl Alcohol, Hyaluronic Acid, Glycosaminoglycans, Biosaccharide Gum-1, Retinyl Palmitate, Cholecalciferol,Tocopheryl Acetate, Ascorbic Acid, Allantoin, Panthenol, TetrasodiumEDTA, Sodium Hydroxymethylglycinate, Sodium Hydroxide, Parfum, Rosmarinus Officinalis, Symphytum Officinale, Citrus Grandis."
        };

// Oppretter dummy bilder
        var pic1 = new VitalHelse.Models.ProductPicture
        {
            ProductPictureId = 1,
            PicturePath = "https://static.wixstatic.com/media/94c78e_e44626f5e8974dfeba2cb5fa19c3fc8b~mv2.jpg/v1/fill/w_1160,h_840,al_c,q_85,usm_0.66_1.00_0.01,enc_avif,quality_auto/94c78e_e44626f5e8974dfeba2cb5fa19c3fc8b~mv2.jpg",
            Product = product, // kobler tilbake til produktet
            ProductId = product.ProductId
        };

        var pic2 = new VitalHelse.Models.ProductPicture
        {
            ProductPictureId = 2,
            PicturePath = "https://static.wixstatic.com/media/94c78e_ea4520d3ac284d0c829fd57f3b1ef859~mv2.jpeg/v1/fill/w_1160,h_840,al_c,q_85,usm_0.66_1.00_0.01,enc_avif,quality_auto/94c78e_ea4520d3ac284d0c829fd57f3b1ef859~mv2.jpeg",
            Product = product,
            ProductId = product.ProductId
        };

// Legger bildene til produktet
        product.ProductPictures.Add(pic1);
        product.ProductPictures.Add(pic2);

        return View(product);
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    

    [Route("produkt/{*categoryPath}")]
    public IActionResult Category(string? categoryPath)
    {
        if (string.IsNullOrEmpty(categoryPath))
            return NotFound();

        var pathParts = categoryPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var categoryName = pathParts.Last();

        // Finn kategorien basert på hele hierarkiet
        var currentCategory = _db.Categories
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

        var children = _db.Categories
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
        var product = _db.Products
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
