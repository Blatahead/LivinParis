using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class ProgramGeo
    {
        public static async Task Main()
        {
            try
            {
                var (latitude, longitude) = await GeocodingHelper.GetCoordinatesAsync("15 rue de la Paix, Paris, 75002");
                Console.WriteLine($"Latitude: {latitude}, Longitude: {longitude}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }
        }
    }
}
