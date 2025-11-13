public class DiscountCode
{
    public int Id { get; set; }

    public string Code { get; set; } = "";

    public int DiscountPercent { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiresAt { get; set; } // null = ingen utløp

    public bool IsActive { get; set; } = true;

    public int TimesUsed { get; set; } = 0;
}