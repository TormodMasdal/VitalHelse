using System.ComponentModel.DataAnnotations;

namespace VitalHelse.Models;

public class DiscountUsage
{
    public DiscountUsage(){}
    
    public int DiscountUsageId { get; set; }

    [Required] public DiscountCode DiscountCode { get; set; } = null!;
    [StringLength(500)] public string DiscountCodeId { get; set; } = null!;

    [Required] public AspNetUsers AspNetUsers { get; set; } = null!;
    [Required][StringLength(450)] public string AspNetUsersId { get; set; } = null!;

    public DateOnly UsedAt { get; set; }
}