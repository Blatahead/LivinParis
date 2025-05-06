using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages
{
    public class NoClientAccountModel : PageModel
    {
        public void OnGet()
        {
        }
        /// <summary>
        /// Redirection vers panel Cuisinier
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostCuisinierPanel()
        {
            return RedirectToPage("/CuisinierPanel");
        }

        /// <summary>
        /// Redirection vers choix Particulier/Entreprise
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostChoixPe()
        {
            return RedirectToPage("/ChoixPe");
        }
    }
}
