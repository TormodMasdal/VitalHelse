using Microsoft.AspNetCore.Identity;

namespace VitalHelse.Models;

// Test comment
public class AspNetUsers : IdentityUser
{
    public ICollection<ProductReview> ProductReviews { get; } = new List<ProductReview>();
    public ICollection<FavoriteProduct> FavoriteProducts { get; } = new List<FavoriteProduct>();
    public ICollection<Order> Orders { get; } = new List<Order>();
    public ICollection<ShoppingCart> ShoppingCarts { get; } = new List<ShoppingCart>();
    public ICollection<DiscountUsage> DiscountUsages { get; } = new List<DiscountUsage>();
}