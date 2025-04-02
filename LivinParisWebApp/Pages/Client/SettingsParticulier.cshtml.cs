using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages.Client
{
    public class SettingsParticulierModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string Prenom { get; set; }

        [BindProperty]
        public string Nom { get; set; }
        [BindProperty]
        public string Arrondissement { get; set; }
        [BindProperty]
        public string Voirie { get; set; }

        [BindProperty]
        public string Numero { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPostActionPage()
        {
            //faire le tri
            return Page();
        }

        public IActionResult OnPostCuisinierPanel()
        {
            return RedirectToPage("/CuisinierPanel");
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
            return RedirectToPage("/Cuisinier/SupprimerCuisinier");
        }
    }
}
