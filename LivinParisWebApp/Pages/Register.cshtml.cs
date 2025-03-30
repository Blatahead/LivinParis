using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;

namespace LivinParis.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _config;
        public string Message { get; set; }
        [Required]
        [BindProperty]
        public string Email { get; set; }
        [Required]
        [BindProperty]
        public string Password { get; set; }

        public RegisterModel(IConfiguration config)
        {
            _config = config;
        }

        public void OnGet() { }
        public async Task<IActionResult> OnPostRegisterAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // recharge la page avec les erreurs
            }


            // Test à ne pas oublier !
            if (Email == "existe@déjà.fr")
            {
                Message = "Cet email est déjà utilisé.";
                return Page(); // Reste sur la page
            }
            string connStr = _config.GetConnectionString("MyDb");


            using var conn = new MySqlConnection(connStr);
            conn.Open();


            //ne pas insert into tout de suite !!! car si un user fait retour après le bouton enregistrer, l'utilisateur sera crée et ne pourra
            // pas choisir ensuite car bloquer dans la page login
            // ou alors le drop quand clique sur retour à la page register
            var cmd = new MySqlCommand("INSERT INTO Utilisateur (Mail_Utilisateur, Mdp) VALUES (@Email, @Pwd)", conn);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@Pwd", Password);

            try
            {
                cmd.ExecuteNonQuery();
                Message = "Compte créé avec succès !";
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                    RedirectToPage();
                //Message = "Cet email est déjà utilisé.";
                else
                    RedirectToPage();
            }
            return RedirectToPage("/ChoixCC");
        }
    }
}