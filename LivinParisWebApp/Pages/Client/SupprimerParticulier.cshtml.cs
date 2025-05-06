using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages.Client
{
    public class SupprimerParticulierModel : PageModel
    {
        public void OnGet()
        {
        }
        /// <summary>
        /// clic sur non pour suppression
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostNo()
        {
            return RedirectToPage("/Client/SettingsClient");
        }

        /// <summary>
        /// clic sur oui pour suppression
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostYes()
        {
            return RedirectToPage("/Login");
        }
    }
}