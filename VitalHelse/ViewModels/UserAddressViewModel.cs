namespace VitalHelse.Models;

public class UserAddressViewModel
{
    public List<UserAddress> Existing { get; set; } = new();
    public UserAddress NewAddress { get; set; } = new();   // bindes fra skjema
    public int? SelectedAddressId { get; set; }            // om du lar bruker velge
}