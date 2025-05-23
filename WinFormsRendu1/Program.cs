using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryRendu1;

namespace WinFormsRendu1
{
    internal static class Program
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Nous avons mis la possibilt� d'afficher certaines donn�es dans la console           ///
        /// Pour y parvenir, il faut d�commenter les lignes avec 4 "/" ("regions � d�commenter")///
        /// Et passer en commentaire les lignes se terminant par 4 "/" ("regions � commenter")  ///
        ///////////////////////////////////////////////////////////////////////////////////////////

        #region A decommenter pour affichage console
        ////[System.Runtime.InteropServices.DllImport("kernel32.dll")]
        ////private static extern bool AllocConsole();
        #endregion

        /// <summary>
        /// Entree principale de l'app
        /// </summary>
        [STAThread]
        static void Main()
        {
            #region A commenter pour affichage console
            ApplicationConfiguration.Initialize();////
            Application.Run(new Form1());////
            #endregion

            #region A decommenter pour affichage console
            ////AllocConsole();

            ////try
            ////{
            ////    Graphe graphe = Graphe.LectureMTX("./../../../../ClassLibraryRendu1/soc-karate.mtx");

            ////    Console.WriteLine("Matrice d'adjacence :");
            ////    Graphe.AfficherMatrice(graphe.MatriceAdjacence());

            ////    Console.WriteLine("\nListe d'adjacence :");
            ////    Graphe.AfficherListeAdjacence(graphe.ListeAdjacence());

            ////    graphe.AfficherLiensGraphe();

            ////
            ////    List<int> parcoursLargeur = graphe.ParcoursLargeur(depart);
            ////    graphe.AfficherParcours("Parcours en Largeur", parcoursLargeur);

            ////    List<int> parcoursProfondeur = graphe.ParcoursProfondeur(depart);
            ////    graphe.AfficherParcours("Parcours en Profondeur", parcoursProfondeur);

            ////    Console.WriteLine($"Le graphe est orient� : {graphe.EstOriente()}");
            ////    Console.WriteLine("Le graphe n'est pas pond�r�.");
            ////    graphe.AfficherDegresSommets();

            ////}
            ////catch (Exception e)
            ////{
            ////    Console.WriteLine($"Erreur : {e.Message}");
            ////}

            ////Console.ReadLine();
            #endregion
        }
    }
}