namespace VitalHelse.Services;

using Microsoft.Extensions.Configuration;

public class TripletexService
{
    private readonly HttpClient _client;
    private readonly string _consumerToken;
    private readonly string _employeeToken;

    public TripletexService(IConfiguration config)
    {
        _client = new HttpClient();
        _consumerToken = config["Tripletex:ConsumerToken"];
        _employeeToken = config["Tripletex:EmployeeToken"];
    }

    // Eksempel: hent session token
    public async Task<string> GetSessionTokenAsync()
    {
        var url = $"https://tripletex.no/v2/token/session/:create?consumerToken={_consumerToken}&employeeToken={_employeeToken}";
        var response = await _client.PutAsync(url, null);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
