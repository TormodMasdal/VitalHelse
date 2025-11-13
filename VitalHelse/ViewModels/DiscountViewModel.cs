namespace VitalHelse.Models
{
    public class DiscountViewModel
    {
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<ProductDiscount> Discounts { get; set; } = new List<ProductDiscount>();

        public List<int> SelectedCategoryIds { get; set; } = new();

        public int TotalProducts { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}