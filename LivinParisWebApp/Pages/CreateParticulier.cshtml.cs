using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParis.Pages
{
    public class CreateParticulierModel : PageModel
    {
        [BindProperty]
        public string FirstName { get; set; }

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Arrondissement { get; set; }

        [BindProperty]
        public string Voirie { get; set; }

        [BindProperty]
        public string Numéro { get; set; }

        public string Message { get; set; }

        //quand on clique sur "C'est parti"

        public IActionResult OnPostCestParti()
        {
            return RedirectToPage();
        }

        // Handler pour le bouton "Retour"
        public IActionResult OnPostChoixPe()
        {
            return RedirectToPage("/ChoixPe");
        }
    }
}
