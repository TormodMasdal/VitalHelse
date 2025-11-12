namespace VitalHelse.Models;

public class DiscountViewModel
{
    public IEnumerable<Product> Products { get; set; } = new List<Product>();
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public IEnumerable<ProductDiscount> Discounts { get; set; } = new List<ProductDiscount>();
    public bool ShowCategoryView { get; set; } = false;
}