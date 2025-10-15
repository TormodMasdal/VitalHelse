using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace VitalHelse.Models;

public class AspNetUsers : IdentityUser
{
    [StringLength(200)] public string? Address { get; set; }
    public PostalCode? PostalCode { get; set; }
    
    [MinLength(4), MaxLength(4)]
    public string? PostalCodeId { get; set; }

    [MinLength(9), MaxLength(9)] public string? OrgNr { get; set; }
    
    public ICollection<ProductReview> ProductReviews { get; } = new List<ProductReview>();
    public ICollection<FavoriteProduct> FavoriteProducts { get; } = new List<FavoriteProduct>();
    public ICollection<Order> Orders { get; } = new List<Order>();
    public ICollection<ShoppingCart> ShoppingCarts { get; } = new List<ShoppingCart>();
    public ICollection<DiscountUsage> DiscountUsages { get; } = new List<DiscountUsage>();
}