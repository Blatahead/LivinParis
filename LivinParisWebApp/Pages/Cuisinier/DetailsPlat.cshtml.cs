using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class DetailsPlatModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPostCuisinierPanel()
        {
            return RedirectToPage("/CuisinierPanel");
        }
    }
}
