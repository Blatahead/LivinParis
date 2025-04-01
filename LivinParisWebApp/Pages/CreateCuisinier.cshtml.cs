using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParis.Pages
{
    public class CreateCuisinierModel : PageModel
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

        //quand on clique sur "Se connecter"
        public IActionResult OnPostLogin()
        {
            //création du cuisinier

            //puis rediriger
            return RedirectToPage("/Cuisinier/Accueil");
        }

        public IActionResult OnPostCestParti()
        {
            return RedirectToPage();
        }

        // Handler pour le bouton "Retour"
        public IActionResult OnPostChoixCC()
        {
            return RedirectToPage("/ChoixCC");
        }
    }
}
