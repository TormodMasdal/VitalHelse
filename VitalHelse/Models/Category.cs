using System.ComponentModel.DataAnnotations;

namespace VitalHelse.Models;

public class Category
{
    Category() {}
    
    public int CategoryId { get; set; }
    
    [StringLength(400)]
    [Required] 
    public string CategoryName { get; set; }

    public ICollection<ProductCategory> ProductCategories { get; } = new List<ProductCategory>();
}