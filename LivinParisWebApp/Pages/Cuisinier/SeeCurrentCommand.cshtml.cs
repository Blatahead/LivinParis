using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class SeeCurrentCommandModel : PageModel
    {
        public void OnGet()
        {

        }

        public IActionResult OnPostCuisinierPanel()
        {
            return RedirectToPage("/CuisinierPanel");
        }

        public IActionResult OnPostRefuseCommande()
        {
            return RedirectToPage("/Cuisinier/RefuseCommande");
        }

        public IActionResult OnPostDetailsCommande()
        {
            return RedirectToPage("/Cuisinier/DetailsCommande");
        }

        public IActionResult OnPostCancelCommande()
        {
            //pas besoin de rediriger, juste enlever de la liste
            return RedirectToPage();
        }

        public IActionResult OnPostLivrerCommande()
        {
            return RedirectToPage("/Cuisinier/LivraisonCuisinier");
        }
    }
}
