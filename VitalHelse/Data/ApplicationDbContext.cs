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

    #region Product entities
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<ProductTag> ProductTags => Set<ProductTag>();
    public DbSet<ProductPicture> ProductPictures => Set<ProductPicture>();
    public DbSet<Tag> Tags => Set<Tag>();
    #endregion

    #region Commerce
    public DbSet<CartProduct> CartProducts => Set<CartProduct>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderProduct> OrderProducts => Set<OrderProduct>();
    public DbSet<DiscountCode> DiscountCodes => Set<DiscountCode>();
    public DbSet<DiscountUsage> DiscountUsages => Set<DiscountUsage>();
    #endregion

    #region User and favorites
    public DbSet<FavoriteProduct> FavoriteProducts => Set<FavoriteProduct>();
    public DbSet<Address> Addresses => Set<Address>();
    
    #endregion

    #region Misc
    public DbSet<PhysicalAttribute> PhysicalAttributes => Set<PhysicalAttribute>();
    public DbSet<PhysicalProductAttribute> PhysicalProductAttributes => Set<PhysicalProductAttribute>();
    public DbSet<ProductReview> ProductReviews => Set<ProductReview>();
    #endregion
}