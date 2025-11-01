using System.ComponentModel.DataAnnotations;

namespace VitalHelse.Models;

public class Address
{
    Address() {}
    
    public int AddressId { get; set; }
    [Required] public AspNetUsers? AspNetUsers { get; set; }
    [StringLength(450)] public string AspNetUsersId { get; set; } = null!;
    
    [Required] 
    [StringLength(100)]
    [Display(Name = "Street")]
    public string Street { get; set; } = string.Empty;
    
    [Required] 
    [StringLength(100)]
    [Display(Name = "City")]
    public string City { get; set; } = string.Empty;
    
    [Required] 
    [MinLength(4), MaxLength(4)]
    [Display(Name = "Postal Code")]
    public int Postalcode { get; set; }
}