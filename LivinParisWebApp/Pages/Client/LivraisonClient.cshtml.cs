using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages.Client
{
    public class LivraisonClientModel : PageModel
    {

        public IActionResult OnPostComandeDetails()
        {
            return RedirectToPage("/Client/DetailsCommande");
        }

        public IActionResult OnPostConfirm()
        {
            return RedirectToPage("/ClientPanel");
        }
    }
}
