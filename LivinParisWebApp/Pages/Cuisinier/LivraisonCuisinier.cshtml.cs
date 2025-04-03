using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class LivraisonCuisinierModel : PageModel
    {
        //chargement du graphe
        public void OnGet()
        {
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
