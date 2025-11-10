namespace VitalHelse.Models;

/// <summary>
/// Represents a product retrieved from Tripletex.
/// </summary>
public class TripletexProduct
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public double PriceExVat { get; set; }
    public double PriceInVat { get; set; }
    public double StockCount { get; set; }
}