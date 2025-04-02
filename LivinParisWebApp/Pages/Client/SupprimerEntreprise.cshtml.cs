using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages.Client
{
    public class SupprimerEntrepriseModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPostNo()
        {
            return RedirectToPage("/Client/SettingsClient");
        }

        public IActionResult OnPostYes()
        {
            return RedirectToPage("/Login");
        }
    }
}
