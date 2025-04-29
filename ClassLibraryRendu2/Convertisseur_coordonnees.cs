using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using DotNetEnv;
using System.Configuration;

public static class Convertisseur_coordonnees
{
    private static string cle_API; 

    public static async Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string address)
    {
        DotNetEnv.Env.Load("../.env");
        cle_API = Environment.GetEnvironmentVariable("CLEMAPG2");

        using HttpClient client = new();
        string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={cle_API}";

        var reponse = await client.GetAsync(url);
        if (!reponse.IsSuccessStatusCode)
            throw new Exception("Erreur lors de la récupération des coordonnées");

        var json = await reponse.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var status = root.GetProperty("status").GetString();
        System.Diagnostics.Debug.WriteLine($"Status retourné par Google Maps: {status}");

        if (status != "OK")
        {
            throw new Exception($"Adresse erronée : Status = {status}");
        }

        var lieu = root.GetProperty("results")[0].GetProperty("geometry").GetProperty("location");
        double latitude = lieu.GetProperty("lat").GetDouble();
        double longitude = lieu.GetProperty("lng").GetDouble();

        return (latitude, longitude);
    }
}