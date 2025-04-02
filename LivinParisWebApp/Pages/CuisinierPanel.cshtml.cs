using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LivinParisWebApp.Pages
{
    public class CuisinierPanelModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPostSettingsCuisinier()
        {
            return RedirectToPage("Cuisinier/SettingsCuisinier");
        }

        public IActionResult OnPostSeeCurrentCommands()
        {
            return RedirectToPage("Cuisinier/SeeCurrentCommand");

        }

        public IActionResult OnPostChangeTodaysPlat()
        {
            return RedirectToPage("Cuisinier/ChangeTodaysPlat");
        }

        public IActionResult OnPostDetailsPlat()
        {
            return RedirectToPage("/Cuisinier/DetailsPlat");

        }

        public IActionResult OnPostAddPlat()
        {
            return RedirectToPage("Cuisinier/AddPlat");
        }

        public IActionResult OnPostDeletePlat()
        {
            return RedirectToPage("Cuisinier/DeletePlat");
        }
    }
}
