namespace VitalHelse.Models
{
    public class DiscountFilterViewModel
    {
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public List<int> SelectedCategoryIds { get; set; } = new();
    }
}