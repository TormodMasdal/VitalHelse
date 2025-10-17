using Microsoft.EntityFrameworkCore;
using VitalHelse.Models;

namespace VitalHelse.Data;

public static class GenerateData
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        // --- 1. Kategorier ---
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category { CategoryName = "Hudpleie" },
                new Category { CategoryName = "Fotpleie" },
                new Category { CategoryName = "Kroppspleie" },
                new Category { CategoryName = "Dagkrem" },
                new Category { CategoryName = "Nattkrem" },
                new Category { CategoryName = "Fotkrem" }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        // --- 2. Tags ---
        if (!context.Tags.Any())
        {
            var tags = new List<Tag>
            {
                new Tag { Tags = "Økologisk" },
                new Tag { Tags = "Populær" },
                new Tag { Tags = "Nyhet" }
            };
            context.Tags.AddRange(tags);
            context.SaveChanges();
        }

        // --- 3. Produkter ---
        if (!context.Products.Any())
        {
            var hudkrem = new Product
            {
                ProductName = "BestBuy 50ml Day Cream",
                ProductPrice = 299.0,
                ProductDescription = "Fuktighetskrem for dagbruk."
            };

            var fotkrem = new Product
            {
                ProductName = "SoftFeet 75ml Foot Cream",
                ProductPrice = 199.0,
                ProductDescription = "Nærende krem for tørre føtter."
            };

            var nattkrem = new Product
            {
                ProductName = "NightGlow 30ml Night Cream",
                ProductPrice = 349.0,
                ProductDescription = "Nærende krem for natten."
            };

            context.Products.AddRange(hudkrem, fotkrem, nattkrem);
            context.SaveChanges(); // ProductId settes her

            // --- 4. Legg til ProductPictures med riktig ProductId ---
            var pictures = new List<ProductPicture>
            {
                new ProductPicture { ProductId = hudkrem.ProductId, PicturePath = "/images/products/daycream.png" },
                new ProductPicture { ProductId = fotkrem.ProductId, PicturePath = "/images/products/footcream.png" },
                new ProductPicture { ProductId = nattkrem.ProductId, PicturePath = "/images/products/nightcream.png" }
            };
            context.ProductPictures.AddRange(pictures);
            context.SaveChanges();

            // --- 5. Koble produkter til kategorier ---
            var hudpleie = context.Categories.First(c => c.CategoryName == "Hudpleie");
            var dagkrem = context.Categories.First(c => c.CategoryName == "Dagkrem");
            var nattkremCat = context.Categories.First(c => c.CategoryName == "Nattkrem");
            var fotpleie = context.Categories.First(c => c.CategoryName == "Fotpleie");
            var fotkremCat = context.Categories.First(c => c.CategoryName == "Fotkrem");

            context.ProductCategories.AddRange(
                new ProductCategory { ProductId = hudkrem.ProductId, CategoryId = hudpleie.CategoryId },
                new ProductCategory { ProductId = hudkrem.ProductId, CategoryId = dagkrem.CategoryId },
                new ProductCategory { ProductId = nattkrem.ProductId, CategoryId = hudpleie.CategoryId },
                new ProductCategory { ProductId = nattkrem.ProductId, CategoryId = nattkremCat.CategoryId },
                new ProductCategory { ProductId = fotkrem.ProductId, CategoryId = fotpleie.CategoryId },
                new ProductCategory { ProductId = fotkrem.ProductId, CategoryId = fotkremCat.CategoryId }
            );
            context.SaveChanges();

            // --- 6. Koble produkter til tags ---
            var økologisk = context.Tags.First(t => t.Tags == "Økologisk");
            var populær = context.Tags.First(t => t.Tags == "Populær");
            var nyhet = context.Tags.First(t => t.Tags == "Nyhet");

            context.ProductTags.AddRange(
                new ProductTags { ProductId = hudkrem.ProductId, TagId = populær.TagId },
                new ProductTags { ProductId = hudkrem.ProductId, TagId = økologisk.TagId },
                new ProductTags { ProductId = nattkrem.ProductId, TagId = nyhet.TagId },
                new ProductTags { ProductId = fotkrem.ProductId, TagId = økologisk.TagId }
            );
            context.SaveChanges();
        }
    }
}
