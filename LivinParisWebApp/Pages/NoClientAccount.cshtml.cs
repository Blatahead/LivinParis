using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages
{
    public class NoClientAccountModel : PageModel
    {
        public void OnGet()
        {
        }

        //penser � rediriger vers le panel (login pour test)
        public IActionResult OnPostCuisinierPanel()
        {
            return RedirectToPage("/Login");
        }

        // bien penser au fait que ce passage skip la page register (donc r�cup les valeurs mises dans login)
        public IActionResult OnPostChoixPe()
        {
            return RedirectToPage("/ChoixPe");
        }
    }
}
