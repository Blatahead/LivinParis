using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class DetailsCommandeModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPostRetour()
        {
            return RedirectToPage("/Cuisinier/SeeCurrentCommand");
        }

        public IActionResult OnPostLivrerCommande()
        {
            //pas de redirection ici (le valider ajoute juste à la liste de commandes pretes)
            //ici test avec seecurrentcommand
            return RedirectToPage("/Cuisinier/SeeCurrentCommand");
        }
    }
}