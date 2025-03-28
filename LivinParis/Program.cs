using ClassLibraryRendu2;
namespace LivinParis
{
    public static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // MySQL en parallèle
            try
            {
                var databaseTask = Task.Run(() => ConnexionDB.ConnectToDatabase());
                MessageBox.Show("Connexion réussie !");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Application.Run(new Form1());
        }




    
    }
    
}