/*using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace VitalHelse.Models;

public class OLDDiscountCode
{
    public OLDDiscountCode(){}

    [StringLength(500)]
    public string DiscountCodeId { get; set; }
    
    public double Rate { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public ICollection<Order> Orders { get; } = new List<Order>();
    public ICollection<OLDDiscountUsage> DiscountUsages { get; } = new List<OLDDiscountUsage>();
}*/