using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace VitalHelse.Models;


public class PostalCode
{
    public PostalCode(){}

    [StringLength(4)] public string PostalCodeId { get; set; }
    [StringLength(100)] public string Area { get; set; }

    public ICollection<AspNetUsers> AspNetUsers { get; } = new List<AspNetUsers>();
}