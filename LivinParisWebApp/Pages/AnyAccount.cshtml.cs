using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages
{
    public class AnyAccountModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPostLogin()
        {
            return RedirectToPage("/Login");
        }

        public IActionResult OnPostRegister()
        {
            return RedirectToPage("/Register");
        }
    }
}
