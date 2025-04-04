using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages
{
    public class ChoixCCModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }
        public void OnGet()
        {
            var UserId = HttpContext.Session.GetInt32("UserId");
            var email = TempData["Email"] as string;

            Console.WriteLine($"SESSION ID: {UserId}");
            Console.WriteLine($"TEMP EMAIL: {email}");

            TempData.Keep("Email"); // à garder si on redirige encore
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