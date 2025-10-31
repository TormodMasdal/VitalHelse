using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace VitalHelse.Services;

public class TripletexService
{
    private readonly HttpClient _client;
    private readonly IConfiguration _config;

    public TripletexService(IConfiguration config)
    {
        _config = config;
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://api-test.tripletex.tech/v2/"); // TEST-miljø
    }

    // 1️⃣ Hent session token fra dine to tokens
    public async Task<string> GetSessionTokenAsync()
    {
        // 1. Hent tokens fra appsettings.Development.json
        var consumerToken = Uri.EscapeDataString(_config["Tripletex:ConsumerToken"] ?? "");
        var employeeToken = Uri.EscapeDataString(_config["Tripletex:EmployeeToken"] ?? "");

        // 2. Legg til utløpsdato (obligatorisk i Tripletex testmiljø)
        var expirationDate = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd");

        // 3. Sett sammen URL-en riktig
        var url = $"token/session/:create?consumerToken={consumerToken}&employeeToken={employeeToken}&expirationDate={expirationDate}";

        // 4. Send PUT-forespørsel
        var response = await _client.PutAsync(url, null);

        // 5. Hvis noe går galt, skriv ut detaljert feilmelding i konsollen
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Kunne ikke hente session token ({response.StatusCode}):\n{error}");
        }

        // 6. Hent token fra JSON
        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("value").GetProperty("token").GetString()!;
    }


    // 2️⃣ Bruk session token for å hente produkter
    public async Task<List<TripletexProduct>> GetProductsAsync(string sessionToken)
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"0:{sessionToken}")));

        var response = await _client.GetAsync("product");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        var values = doc.RootElement.GetProperty("values");
        var products = new List<TripletexProduct>();

        foreach (var item in values.EnumerateArray())
        {
            products.Add(new TripletexProduct
            {
                Id = item.GetProperty("id").GetInt32(),
                Name = item.GetProperty("name").GetString() ?? "",
                Price = item.TryGetProperty("costPrice", out var p) ? p.GetDouble() : 0,
                StockCount = item.TryGetProperty("stock", out var s) ? s.GetInt32() : 0
            });
        }

        return products;
    }
}

public class TripletexProduct
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public double Price { get; set; }
    public int StockCount { get; set; }
}
