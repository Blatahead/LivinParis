using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParis.Pages
{
    public class ChoixPeModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPostCreateParticulier()
        {
            return RedirectToPage("/CreateParticulier");
        }
        public IActionResult OnPostCreateEntreprise()
        {
            return RedirectToPage("/CreateEntreprise");
        }
        public IActionResult OnPostChoixCC()
        {
            return RedirectToPage("/ChoixCC");
        }
    }
}
