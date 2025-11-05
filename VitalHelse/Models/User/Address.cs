using System.ComponentModel.DataAnnotations;

namespace VitalHelse.Models;

public class Address
{
    public Address() {}
    
    public int? AddressId { get; set; }
    public AspNetUsers? AspNetUsers { get; set; }
    
    // Gjorde den om slik at den kan være NULL foreløpig siden det funket ikke ellers
    [StringLength(450)] public string? AspNetUsersId { get; set; } = null!;
    
    [Required] 
    [StringLength(100)]
    [Display(Name = "Street")]
    public string Street { get; set; } = string.Empty;
    
    [Required] 
    [StringLength(100)]
    [Display(Name = "City")]
    public string City { get; set; } = string.Empty;
    
    [Required] 
    [StringLength(4, MinimumLength = 4)]
    [Display(Name = "Postal Code")]
    public string PostalCode { get; set; } = string.Empty;
}