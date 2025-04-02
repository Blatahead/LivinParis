using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages.Client
{
    public class DetailsCommandeModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPostRetour()
        {
            return RedirectToPage("/Client/ClientPanel");
        }

        public IActionResult OnPostLivrerCommande()
        {
            return RedirectToPage("/Client/LivrerCommande");
        }
    }
}
