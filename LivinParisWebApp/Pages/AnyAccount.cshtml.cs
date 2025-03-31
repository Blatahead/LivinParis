using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages
{
    public class AnyAccountModel : PageModel
    {
        public void OnGet()
        {
        }

        //penser à rediriger vers le panel (login pour test)
        public IActionResult OnPostLogin()
        {
            return RedirectToPage("/Login");
        }

        // pas besoin de récup les valeurs mises dans login : on redirige vers register directement
        public IActionResult OnPostRegister()
        {
            return RedirectToPage("/Register");
        }
    }
}
