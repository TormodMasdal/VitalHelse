namespace VitalHelse.Models;

public class StockViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int? StockCount { get; set; }
    public double ProductPriceInVAT { get; set; }
    public string? FirstImagePath { get; set; }  
    public List<string> CategoryNames { get; set; } = new List<string>(); 
}