using Azure.Identity;
using Microsoft.EntityFrameworkCore;

namespace VitalHelse.Models;

[PrimaryKey(nameof(ProductId), nameof(ShoppingCartId))]
public class CartProduct
{
    public CartProduct(){}

    public Product Product { get; set; }
    public ShoppingCart ShoppingCart { get; set; }

    public int ProductId { get; set; }
    public int ShoppingCartId { get; set; }
    public int Quantity { get; set; }
}