using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class DeletePlatModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPostCuisinierPanelNo()
        {
            return RedirectToPage("/CuisinierPanel");
        }

        public IActionResult OnPostCuisinierPanelYes()
        {
            return RedirectToPage("/CuisinierPanel");
        }
    }
}
