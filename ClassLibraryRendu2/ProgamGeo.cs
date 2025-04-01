using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class ProgamGeo
    {

        
            public static async Task Main()
            {
                try
                {
                    var (latitude, longitude) = await GeocodingHelper.GetCoordinatesAsync("1600 Amphitheatre Parkway, Mountain View, CA");
                    Console.WriteLine($"Latitude: {latitude}, Longitude: {longitude}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur : {ex.Message}");
                }
            }
    }

}
