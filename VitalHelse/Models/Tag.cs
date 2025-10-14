using System.ComponentModel.DataAnnotations;

namespace VitalHelse.Models;

public class Tag
{
    Tag(){}
    
    public int TagId { get; set; }
    [Required][StringLength(50)] public string Tags { get; set; }

    public ICollection<ProductTags> ProductTags { get; } = new List<ProductTags>();
}