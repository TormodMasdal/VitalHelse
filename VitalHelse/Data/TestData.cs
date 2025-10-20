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
            // Toppnivå-kategorier
            var hudpleie = new Category { CategoryName = "hudpleie" };
            var kjøleprodukter = new Category { CategoryName = "kjøleprodukter" };

            context.Categories.AddRange(hudpleie, kjøleprodukter);
            context.SaveChanges();

            // Underkategorier
            var krem = new Category { CategoryName = "krem", ParentCategoryId = hudpleie.CategoryId };
            var serum = new Category { CategoryName = "serum", ParentCategoryId = hudpleie.CategoryId };
            var aloevera = new Category { CategoryName = "aloevera", ParentCategoryId = hudpleie.CategoryId };

            var kjolekrem = new Category { CategoryName = "krem", ParentCategoryId = kjøleprodukter.CategoryId };
            var spray = new Category { CategoryName = "spray", ParentCategoryId = kjøleprodukter.CategoryId };

            context.Categories.AddRange(krem, serum, aloevera, kjolekrem, spray);
            context.SaveChanges();

            // Sub-subkategorier
            var ansiktskrem = new Category { CategoryName = "ansiktskrem", ParentCategoryId = krem.CategoryId };
            var handogfotkrem = new Category { CategoryName = "handogfotkrem", ParentCategoryId = krem.CategoryId };
            var kuldekrem = new Category { CategoryName = "kuldekrem", ParentCategoryId = kjolekrem.CategoryId };
            var kuldespray = new Category { CategoryName = "kuldespray", ParentCategoryId = spray.CategoryId };

            context.Categories.AddRange(ansiktskrem, handogfotkrem, kuldekrem, kuldespray);
            context.SaveChanges();
        }

        // --- 2. Tags ---
        if (!context.Tags.Any())
        {
            var tags = new List<Tag>
            {
                new Tag { Tags = "økologisk" },
                new Tag { Tags = "populær" },
                new Tag { Tags = "nyhet" }
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

            // --- 4. Produktbilder ---
            var pictures = new List<ProductPicture>
            {
                new ProductPicture { ProductId = hudkrem.ProductId, PicturePath = "/images/products/daycream.png" },
                new ProductPicture { ProductId = fotkrem.ProductId, PicturePath = "/images/products/footcream.png" },
                new ProductPicture { ProductId = nattkrem.ProductId, PicturePath = "/images/products/nightcream.png" },
                new ProductPicture { ProductId = kuldekremProd.ProductId, PicturePath = "/images/products/coldcream.png" }
            };
            context.ProductPictures.AddRange(pictures);
            context.SaveChanges();

            // --- 5. Produkt–kategori koblinger ---
            var krem = context.Categories.First(c => c.CategoryName == "krem" && c.ParentCategory.CategoryName == "hudpleie");
            var ansiktskrem = context.Categories.First(c => c.CategoryName == "ansiktskrem");
            var handogfotkrem = context.Categories.First(c => c.CategoryName == "handogfotkrem");
            var kjolekrem = context.Categories.First(c => c.CategoryName == "krem" && c.ParentCategory.CategoryName == "kjøleprodukter");
            var kuldekrem = context.Categories.First(c => c.CategoryName == "kuldekrem");

            context.ProductCategories.AddRange(
                new ProductCategory { ProductId = hudkrem.ProductId, CategoryId = ansiktskrem.CategoryId },
                new ProductCategory { ProductId = fotkrem.ProductId, CategoryId = handogfotkrem.CategoryId },
                new ProductCategory { ProductId = nattkrem.ProductId, CategoryId = ansiktskrem.CategoryId },
                new ProductCategory { ProductId = kuldekremProd.ProductId, CategoryId = kuldekrem.CategoryId }
            );
            context.SaveChanges();

            // --- 6. Produkt–tag koblinger ---
            var okologisk = context.Tags.First(t => t.Tags == "økologisk");
            var popular = context.Tags.First(t => t.Tags == "populær");
            var nyhet = context.Tags.First(t => t.Tags == "nyhet");

            context.ProductTags.AddRange(
                new ProductTags { ProductId = hudkrem.ProductId, TagId = popular.TagId },
                new ProductTags { ProductId = hudkrem.ProductId, TagId = okologisk.TagId },
                new ProductTags { ProductId = nattkrem.ProductId, TagId = nyhet.TagId },
                new ProductTags { ProductId = fotkrem.ProductId, TagId = okologisk.TagId },
                new ProductTags { ProductId = kuldekremProd.ProductId, TagId = nyhet.TagId }
            );
            context.SaveChanges();
        }
    }
}
