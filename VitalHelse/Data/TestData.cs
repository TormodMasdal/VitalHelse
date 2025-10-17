using Microsoft.EntityFrameworkCore;
using VitalHelse.Models;

namespace VitalHelse.Data;

public static class TestData
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        // --- 1. Kategorier med hierarki ---
        if (!context.Categories.Any())
        {
            // Top-level kategorier
            var hudpleie = new Category { CategoryName = "Hudpleie" };
            var kjoleprodukter = new Category { CategoryName = "Kjøleprodukter" };

            context.Categories.AddRange(hudpleie, kjoleprodukter);
            context.SaveChanges();

            // Underkategorier
            var krem = new Category { CategoryName = "Krem", ParentCategoryId = hudpleie.CategoryId };
            var serum = new Category { CategoryName = "Serum", ParentCategoryId = hudpleie.CategoryId };
            var aloe = new Category { CategoryName = "Aloe Vera", ParentCategoryId = hudpleie.CategoryId };

            var kremKjol = new Category { CategoryName = "Krem", ParentCategoryId = kjoleprodukter.CategoryId };
            var spray = new Category { CategoryName = "Spray", ParentCategoryId = kjoleprodukter.CategoryId };

            context.Categories.AddRange(krem, serum, aloe, kremKjol, spray);
            context.SaveChanges();

            // Sub-subkategorier
            var ansiktskrem = new Category { CategoryName = "Ansiktskrem", ParentCategoryId = krem.CategoryId };
            var handFotKrem = new Category { CategoryName = "Hånd- og fotkrem", ParentCategoryId = krem.CategoryId };
            var kuldekrem = new Category { CategoryName = "Kuldekrem", ParentCategoryId = kremKjol.CategoryId };
            var kuldespray = new Category { CategoryName = "Kuldespray", ParentCategoryId = spray.CategoryId };

            context.Categories.AddRange(ansiktskrem, handFotKrem, kuldekrem, kuldespray);
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

            var kuldekremProd = new Product
            {
                ProductName = "CoolFace 50ml Kuldekrem",
                ProductPrice = 279.0,
                ProductDescription = "Avkjølende krem for ansiktet."
            };

            context.Products.AddRange(hudkrem, fotkrem, nattkrem, kuldekremProd);
            context.SaveChanges();

            // --- 4. Legg til ProductPictures ---
            var pictures = new List<ProductPicture>
            {
                new ProductPicture { ProductId = hudkrem.ProductId, PicturePath = "/images/products/daycream.png" },
                new ProductPicture { ProductId = fotkrem.ProductId, PicturePath = "/images/products/footcream.png" },
                new ProductPicture { ProductId = nattkrem.ProductId, PicturePath = "/images/products/nightcream.png" },
                new ProductPicture { ProductId = kuldekremProd.ProductId, PicturePath = "/images/products/coldcream.png" }
            };
            context.ProductPictures.AddRange(pictures);
            context.SaveChanges();

            // --- 5. Koble produkter til kategorier ---
            var krem = context.Categories.First(c => c.CategoryName == "Krem" && c.ParentCategory.CategoryName == "Hudpleie");
            var ansiktskrem = context.Categories.First(c => c.CategoryName == "Ansiktskrem");
            var handFotKrem = context.Categories.First(c => c.CategoryName == "Hånd- og fotkrem");
            var kremKjol = context.Categories.First(c => c.CategoryName == "Krem" && c.ParentCategory.CategoryName == "Kjøleprodukter");
            var kuldekrem = context.Categories.First(c => c.CategoryName == "Kuldekrem");

            context.ProductCategories.AddRange(
                new ProductCategory { ProductId = hudkrem.ProductId, CategoryId = ansiktskrem.CategoryId },
                new ProductCategory { ProductId = fotkrem.ProductId, CategoryId = handFotKrem.CategoryId },
                new ProductCategory { ProductId = nattkrem.ProductId, CategoryId = ansiktskrem.CategoryId },
                new ProductCategory { ProductId = kuldekremProd.ProductId, CategoryId = kuldekrem.CategoryId }
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
                new ProductTags { ProductId = fotkrem.ProductId, TagId = økologisk.TagId },
                new ProductTags { ProductId = kuldekremProd.ProductId, TagId = nyhet.TagId }
            );
            context.SaveChanges();
        }
    }
}
