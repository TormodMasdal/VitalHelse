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

    public ProductController(ApplicationDbContext db)
    {
        _db = db;
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
}   