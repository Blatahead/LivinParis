using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParis.Pages
{
    public class ChoixCCModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPostRegister()
        {
            return RedirectToPage("/Register");
        }

        public IActionResult OnPostCreateCuisinier()
        {
            return RedirectToPage("/CreateCuisinier");
        }

        public IActionResult OnPostChoixPe()
        {
            return RedirectToPage("/ChoixPe");
        }
    }
}