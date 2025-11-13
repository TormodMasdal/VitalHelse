using VitalHelse.Models;

public class CategoryDiscount
{
    public int Id { get; set; }

    public int CategoryId { get; set; }
    public ProductCategory Category { get; set; }

    public int DiscountPercent { get; set; }
}