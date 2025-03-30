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


            // Test � ne pas oublier !
            if (Email == "existe@d�j�.fr")
            {
                Message = "Cet email est d�j� utilis�.";
                return Page(); // Reste sur la page
            }
            string connStr = _config.GetConnectionString("MyDb");


            using var conn = new MySqlConnection(connStr);
            conn.Open();


            //ne pas insert into tout de suite !!! car si un user fait retour apr�s le bouton enregistrer, l'utilisateur sera cr�e et ne pourra
            // pas choisir ensuite car bloquer dans la page login
            // ou alors le drop quand clique sur retour � la page register
            var cmd = new MySqlCommand("INSERT INTO Utilisateur (Mail_Utilisateur, Mdp) VALUES (@Email, @Pwd)", conn);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@Pwd", Password);

            try
            {
                cmd.ExecuteNonQuery();
                Message = "Compte cr�� avec succ�s !";
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                    RedirectToPage();
                //Message = "Cet email est d�j� utilis�.";
                else
                    RedirectToPage();
            }
            return RedirectToPage("/ChoixCC");
        }
    }
}