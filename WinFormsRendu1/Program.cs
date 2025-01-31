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
        //ouverture d'une console même dans un projet Winforms car GUI
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //ApplicationConfiguration.Initialize();
            //Application.Run(new Form1());
            AllocConsole();

            try
            {
                Graphe graphe = Graphe.LectureMTX("./../../../../ClassLibraryRendu1/soc-karate.mtx");

                Console.WriteLine("Matrice d'adjacence :");
                Graphe.AfficherMatrice(graphe.MatriceAdjacence());

                Console.WriteLine("\nListe d'adjacence :");
                Graphe.AfficherListeAdjacence(graphe.ListeAdjacence());

                graphe.AfficherLiensGraphe();

                Noeud depart = graphe.Noeuds[0];
                List<int> parcoursLargeur = graphe.ParcoursLargeur(depart);
                graphe.AfficherParcours("Parcours en Largeur", parcoursLargeur);

                List<int> parcoursProfondeur = graphe.ParcoursProfondeur(depart);
                graphe.AfficherParcours("Parcours en Profondeur", parcoursProfondeur);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erreur : {e.Message}");
            }

            Console.ReadLine();
        }
    }
}