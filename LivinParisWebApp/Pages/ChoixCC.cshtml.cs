using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages
{
    public class ChoixCCModel : PageModel
    {
        public void OnGet()
        {
            TempData.Keep("Email");
            TempData.Keep("Password");
        }

        public IActionResult OnPostRegister()
        {
            return RedirectToPage("/Register");
        }

        public IActionResult OnPostCreateCuisinier()
        {
            TempData.Keep("Email");
            return RedirectToPage("/CreateCuisinier");
        }

        public IActionResult OnPostChoixPe()
        {
            TempData.Keep("Email");
            return RedirectToPage("/ChoixPe");
        }
    }
}