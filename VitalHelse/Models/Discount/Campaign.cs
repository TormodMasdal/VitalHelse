public class Campaign
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public bool IsActive { get; set; } = false;

    public List<int> CategoryIds { get; set; } = new();
}