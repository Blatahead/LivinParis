using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages.Client
{
    public class SupprimerEntrepriseModel : PageModel
    {
        public void OnGet()
        {
        }
        /// <summary>
        /// clic sur le oui pour suppression
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostNo()
        {
            return RedirectToPage("/Client/SettingsClient");
        }

        /// <summary>
        /// clic sur non pour suppression
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostYes()
        {
            return RedirectToPage("/Login");
        }
    }
}
