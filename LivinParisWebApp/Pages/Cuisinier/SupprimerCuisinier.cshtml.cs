using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class SupprimerCuisinierModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPostNo()
        {
            return RedirectToPage("/Cuisinier/SettingsCuisinier");
        }

        public IActionResult OnPostYes()
        {
            return RedirectToPage("/Register");
        }
    }
}
