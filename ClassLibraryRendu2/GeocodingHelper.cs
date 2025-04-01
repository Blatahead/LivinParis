using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

public static class GeocodingHelper
{
    private const string ApiKey = "VOTRE_CLE_API"; // Remplacez par votre clé Google Maps

    public static async Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string address)
    {
        using HttpClient client = new();
        string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={ApiKey}";

        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Erreur lors de la récupération des coordonnées");

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.GetProperty("status").GetString() != "OK")
            throw new Exception("Adresse introuvable");

        var location = root.GetProperty("results")[0].GetProperty("geometry").GetProperty("location");
        double lat = location.GetProperty("lat").GetDouble();
        double lng = location.GetProperty("lng").GetDouble();

        return (lat, lng);
    }
}