using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParis.Pages
{
    public class CreateEntrepriseModel : PageModel
    {
        [BindProperty] public string NomEntreprise { get; set; }
        [BindProperty] public string NumeroSiret { get; set; }
        [BindProperty] public string NomReferent { get; set; }
        [BindProperty] public string Arrondissement { get; set; }
        [BindProperty] public string Voirie { get; set; }
        [BindProperty] public string Numéro { get; set; }

        public IActionResult OnPostCestParti()
        {
            //enregistrer l'entreprise

            //puis redirection avec vue entreprise !
            return RedirectToPage("/Client/Accueil");
        }

        public IActionResult OnPostChoixPe()
        {
            return RedirectToPage("/ChoixPe");
        }
    }
}
