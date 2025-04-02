using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class ChangeTodaysPlatModel : PageModel
    {
        [Required]
        [BindProperty]
        public string NomDuPlat { get; set; }

        [Required]
        [BindProperty]
        public string Prix { get; set; }

        [Required]
        [BindProperty]
        public string NbDePersonnes { get; set; }

        [Required]
        [BindProperty]
        public string Type { get; set; }
        [Required]
        [BindProperty]
        public string JourCreation { get; set; }

        [Required]
        [BindProperty]
        public string MoisCreation { get; set; }
        [Required]
        [BindProperty]
        public string AnneeCreation { get; set; }

        [Required]
        [BindProperty]
        public string JourPerem { get; set; }
        [Required]
        [BindProperty]
        public string MoisPerem { get; set; }

        [Required]
        [BindProperty]
        public string AnneePerem { get; set; }
        [Required]
        [BindProperty]
        public string Ingredients { get; set; }
        public void OnGet()
        {
        }
        public IActionResult OnPostCuisinierPanelRetour()
        {
            return RedirectToPage("/CuisinierPanel");
        }

        public IActionResult OnPostCuisinierPanelConfirm()
        {
            return RedirectToPage("/CuisinierPanel");
        }
    }
}