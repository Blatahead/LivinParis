using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class RefuseCommandeModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPostSeeCurrentCommandNo()
        {
            return RedirectToPage("/Cuisinier/SeeCurrentCommand");
        }

        public IActionResult OnPostSeeCurrentCommandYes()
        {
            return RedirectToPage("/Cuisinier/SeeCurrentCommand");
        }
    }
}
