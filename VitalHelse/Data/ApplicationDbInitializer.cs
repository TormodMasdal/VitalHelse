using Microsoft.AspNetCore.Identity;
using VitalHelse.Models;

namespace VitalHelse.Data;

public class ApplicationDbInitializer
{
    public static void Initialize(ApplicationDbContext db, UserManager<AspNetUsers> um, RoleManager<IdentityRole> rm)
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        // Roles for the website. No role = private customer
        rm.CreateAsync(new IdentityRole("Admin")).Wait();
        rm.CreateAsync(new IdentityRole("Staff")).Wait();
        rm.CreateAsync(new IdentityRole("BusinessCustomer")).Wait();

        // Test users for all roles
        var adminUser = new AspNetUsers
            { UserName = "admin@VitalHelse.no", Email = "admin@VitalHelse.no", EmailConfirmed = true };
        um.CreateAsync(adminUser, "Password1.").Wait();
        um.AddToRoleAsync(adminUser, "Admin").Wait();

        var staffUser = new AspNetUsers
            { UserName = "staff@VitalHelse.no", Email = "staff@VitalHelse.no", EmailConfirmed = true };
        um.CreateAsync(staffUser, "Password.1");
        um.AddToRoleAsync(staffUser, "Staff").Wait();
        
        var businessUser = new AspNetUsers
            { UserName = "business@VitalHelse.no", Email = "business@VitalHelse.no", EmailConfirmed = true };
        um.CreateAsync(businessUser, "Password.1");
        um.AddToRoleAsync(businessUser, "BusinessCustomer").Wait();

        var customerUser = new AspNetUsers
            { UserName = "business@VitalHelse.no", Email = "customer@VitalHelse.no", EmailConfirmed = true };
        um.CreateAsync(customerUser, "Password.1");
        
        // Seed Products - Add this section ↓
        SeedProducts(db);
        
        db.SaveChanges();
    }
    
    // Temporary Db Set for products
    //used for testing purposes
    private static void SeedProducts(ApplicationDbContext db)
    {
        /*var products = new List<Product>
        {
            new Product
            {
                ProductName = "Vitamin C 1000mg",
                ProductDescription = "High-potency vitamin C tablets to support immune system health and boost antioxidant protection.",
                ProductIngredients = "Ascorbic Acid, Cellulose, Magnesium Stearate",
                ProductPrice = 19.99,
                StockCount = 150,
                LabelDescription = "Bestseller"
            },
            new Product
            {
                ProductName = "Omega-3 Fish Oil",
                ProductDescription = "Premium quality fish oil capsules rich in EPA and DHA omega-3 fatty acids for cardiovascular and brain health.",
                ProductIngredients = "Fish Oil, Gelatin, Glycerin, Vitamin E",
                ProductPrice = 29.99,
                ProductCampaignPrice = 24.99,
                StockCount = 80,
                LabelDescription = "Sale"
            },
            new Product
            {
                ProductName = "Multivitamin Complex",
                ProductDescription = "Complete daily multivitamin formula with essential vitamins and minerals for overall health and wellbeing.",
                ProductIngredients = "Vitamins A, C, D, E, B-Complex, Zinc, Selenium, Iron",
                ProductPrice = 34.99,
                StockCount = 200
            },
            new Product
            {
                ProductName = "Whey Protein Isolate",
                ProductDescription = "Pure whey protein isolate powder for muscle recovery, growth, and post-workout nutrition. Chocolate flavor.",
                ProductIngredients = "Whey Protein Isolate, Cocoa Powder, Natural Flavors, Stevia",
                ProductPrice = 49.99,
                StockCount = 45,
                LabelDescription = "New"
            },
            new Product
            {
                ProductName = "Magnesium Citrate 400mg",
                ProductDescription = "Highly absorbable magnesium citrate supplement to support muscle relaxation, sleep quality, and bone health.",
                ProductIngredients = "Magnesium Citrate, Cellulose, Silicon Dioxide",
                ProductPrice = 15.99,
                StockCount = 120
            },
            new Product
            {
                ProductName = "Probiotic Complex",
                ProductDescription = "Advanced probiotic formula with 10 billion CFU per capsule to support digestive health and immune function.",
                ProductIngredients = "Lactobacillus, Bifidobacterium, Prebiotic Fiber",
                ProductPrice = 27.99,
                StockCount = 90
            },
            new Product
            {
                ProductName = "Collagen Peptides",
                ProductDescription = "Hydrolyzed collagen peptides powder to support skin elasticity, joint health, and hair strength.",
                ProductIngredients = "Bovine Collagen Peptides, Vitamin C",
                ProductPrice = 39.99,
                ProductCampaignPrice = 34.99,
                StockCount = 60,
                LabelDescription = "Sale"
            },
            new Product
            {
                ProductName = "Zinc 50mg",
                ProductDescription = "Essential zinc supplement to boost immune system, support wound healing, and maintain healthy skin.",
                ProductIngredients = "Zinc Gluconate, Microcrystalline Cellulose",
                ProductPrice = 12.99,
                StockCount = 180
            },
            new Product
            {
                ProductName = "Turmeric Curcumin",
                ProductDescription = "High-strength turmeric extract with black pepper for enhanced absorption. Natural anti-inflammatory support.",
                ProductIngredients = "Turmeric Extract (95% Curcuminoids), Black Pepper Extract",
                ProductPrice = 24.99,
                StockCount = 100
            },
            new Product
            {
                ProductName = "Vitamin D3 5000 IU",
                ProductDescription = "High-dose vitamin D3 supplement for bone health, immune support, and mood regulation.",
                ProductIngredients = "Cholecalciferol (Vitamin D3), Olive Oil, Gelatin",
                ProductPrice = 16.99,
                StockCount = 140,
                LabelDescription = "Bestseller"
            }
        };

        db.Products.AddRange(products);*/
    }
}