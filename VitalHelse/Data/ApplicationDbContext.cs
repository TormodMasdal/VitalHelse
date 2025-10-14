using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VitalHelse.Models;

namespace VitalHelse.Data;

public class ApplicationDbContext : IdentityDbContext<AspNetUsers>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<ProductPicture> ProductPictures => Set<ProductPicture>();
    public DbSet<ProductTags> ProductTags => Set<ProductTags>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<PhysicalAttribute> PhysicalAttributes => Set<PhysicalAttribute>();
    public DbSet<PhysicalProductAttribute> PhysicalProductAttributes => Set<PhysicalProductAttribute>();
    public DbSet<ProductReview> ProductReviews => Set<ProductReview>();
    public DbSet<DiscountCode> DiscountCodes => Set<DiscountCode>();
    public DbSet<DiscountUsage> DiscountUsages => Set<DiscountUsage>();
    public DbSet<FavoriteProduct> FavoriteProducts => Set<FavoriteProduct>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderProduct> OrderProducts => Set<OrderProduct>();
    public DbSet<CartProduct> CartProducts => Set<CartProduct>();
    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
    
}