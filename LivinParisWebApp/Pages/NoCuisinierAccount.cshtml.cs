using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages
{
    public class NoCuisinierAccountModel : PageModel
    {
        public void OnGet()
        {
        }
        /// <summary>
        /// Redirection vers le panel Client
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostClientPanel()
        {
            return RedirectToPage("/ClientPanel");
        }

        /// <summary>
        /// Redirection vers la création Cuisinier
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostCreateCuisinier()
        {
            return RedirectToPage("/CreateCuisinier");
        }
    }
}
