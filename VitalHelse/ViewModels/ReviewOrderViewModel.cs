namespace VitalHelse.Models;

public class ReviewOrderViewModel
{
    public List<CartProduct> CartProducts { get; set; } = new();
    public UserAddress UserAddress { get; set; } = new();
}