using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages.Client
{
    public class SettingsEntrepriseModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string NomEntre { get; set; }

        [BindProperty]
        public string Siret { get; set; }
        [BindProperty]
        public string NomRef { get; set; }
        [BindProperty]
        public string Arrondissement { get; set; }
        [BindProperty]
        public string Voirie { get; set; }

        [BindProperty]
        public string Numero { get; set; }
        public IActionResult OnPostActionPage()
        {
            //faire le tri
            return Page();
        }

        public IActionResult OnPostClientPanel()
        {
            return RedirectToPage("/ClientPanel");
        }

        public IActionResult OnPostValidate()
        {
            //afficher une popup alerte en mm temps que la sauvegarde
            return Page();
        }
        public IActionResult OnPostDeconnexion()
        {
            return RedirectToPage("/Login");
        }
        public IActionResult OnPostSupprimer()
        {
            //faire le delete dans la bdd
            return RedirectToPage("/Client/SupprimerEntreprise");
        }
    }
}
