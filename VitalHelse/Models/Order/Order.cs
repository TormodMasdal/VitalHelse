using System.ComponentModel.DataAnnotations;

namespace VitalHelse.Models;

public class Order
{
    public Order(){}

    public int OrderId { get; set; }
    public string? StripePaymentIntentId { get; set; }

    
    public DateTime? OrderDate { get; set; }
    [StringLength(50)] public string Status { get; set; } = "Draft";
    public decimal TotalCost { get; set; }
    [StringLength(100)] public string? TrackingNumber { get; set; }
    [StringLength(50)] public string? ShippingProvider { get; set; }

    [StringLength(500)] public string? DiscountCodeId { get; set; }

    [Required] public AspNetUsers AspNetUsers { get; set; }
    [StringLength(450)] public string? AspNetUsersId { get; set; }
    
    public UserAddress? UserAddress { get; set; }

    public ICollection<OrderProduct> OrderProducts { get; } = new List<OrderProduct>();
}