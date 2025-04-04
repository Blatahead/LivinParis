using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParis.Pages
{
    public class ChoixPeModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }
        public void OnGet()
        {
        }

        public IActionResult OnPostCreateParticulier()
        {
            TempData.Keep("Email");
            return RedirectToPage("/CreateParticulier");
        }
        public IActionResult OnPostCreateEntreprise()
        {
            TempData.Keep("Email");
            return RedirectToPage("/CreateEntreprise");
        }
        public IActionResult OnPostChoixCC()
        {
            return RedirectToPage("/ChoixCC");
        }
    }
}
