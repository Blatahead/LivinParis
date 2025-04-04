using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLibraryRendu2;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class LivraisonCuisinierModel : PageModel
    {
        //chargement du graphe
        private readonly IConfiguration _config;

        public LivraisonCuisinierModel(IConfiguration config)
        {
            _config = config;
        }

        public void OnGet()
        {
            var graphe = new Graphe();
            string connStr = _config.GetConnectionString("MyDb");
            graphe.ChargerDepuisBDD(connStr);

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            ViewData["Stations"] = JsonConvert.SerializeObject(graphe.Stations, settings);
            ViewData["Arcs"] = JsonConvert.SerializeObject(graphe.Arcs, settings);
        }
        public IActionResult OnPostComandeDetails()
        {
            return RedirectToPage("/Cuisinier/DetailsCommande");
        }

        public IActionResult OnPostConfirm()
        {
            return RedirectToPage("/CuisinierPanel");
        }
    }
}
