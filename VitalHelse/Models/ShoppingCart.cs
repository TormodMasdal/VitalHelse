using System.ComponentModel.DataAnnotations;

namespace VitalHelse.Models;

public class ShoppingCart
{
    ShoppingCart(){}
    
    public int ShoppingCartId { get; set; }

    [Required] public AspNetUsers AspNetUsers { get; set; } = null!;
    [StringLength(450)] public string AspNetUsersId { get; set; } = null!;

    public ICollection<CartProduct> CartProducts { get; } = new List<CartProduct>();
}