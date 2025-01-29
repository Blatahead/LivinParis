using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryRendu1;

namespace ClassLibraryRendu1
{
    public class Graphe
    {
        #region Attributs
        List<Noeud> noeuds;
        List<Lien> liens;
        #endregion

        #region Propriétés
        public List<Noeud> Noeuds { get { return this.noeuds; } set {this.noeuds=value; } }
        public List<Lien> Liens { get{ return this.liens; } set { this.liens = value; } }
        #endregion

        #region Constructeurs
        public Graphe()
        {
            this.noeuds = new List<Noeud>();
            this.liens = new List<Lien>();
        }
        #endregion

        #region Methodes
        /// <summary>
        /// Créer un lien entre un noeud source et un noeud destination
        /// </summary>
        /// <param name="idSource"></param>
        /// <param name="idDestination"></param>
        public void NouveauLien(int idSource, int idDestination)
        {
            Noeud source = this.noeuds.FirstOrDefault(n => n.Id == idSource);
            Noeud destination = this.noeuds.FirstOrDefault(n => n.Id == idDestination);

            if (source == null)
            {
                source = new Noeud(idSource);
                this.noeuds.Add(source);
            }
            if (destination == null)
            {
                destination = new Noeud(idDestination);
                this.noeuds.Add(destination);
            }

            // Ajouter les voisins si ce n'est pas déjà fait
            if (!source.Voisins.Contains(destination))
                source.Voisins.Add(destination);

            if (!destination.Voisins.Contains(source))
                destination.Voisins.Add(source);

            this.liens.Add(new Lien(source, destination));
        }


        /// <summary>
        /// Permet de lire un fichier .mtx
        /// </summary>
        /// <param name="chemin"></param>
        /// <returns></returns>
        public static Graphe LectureMTX(string chemin)
        {
            Graphe graphe = new Graphe();

            foreach (string ligne in File.ReadLines(chemin))
            {
                if (ligne.StartsWith("%") || string.IsNullOrWhiteSpace(ligne))
                {
                    continue;
                }
                char separateur = ' ';
                string[] parties = ligne.Split(separateur);
                if (parties.Length == 2)
                {
                    int sommet1 = int.Parse(parties[0]);
                    int sommet2 = int.Parse(parties[1]);

                    graphe.NouveauLien(sommet1, sommet2);
                }
            }
            return graphe;
        }

        /// <summary>
        /// Permet de visualiser les voisins de chaque noeud
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, List<int>> ListeAdjacence()
        {
            Dictionary<int, List<int>> listeAdjacence = new Dictionary<int, List<int>>();
            foreach (Noeud noeud in this.noeuds)
            {
                listeAdjacence[noeud.Id] = noeud.Voisins.Select(v => v.Id).ToList();
            }
            return listeAdjacence;
        }

        /// <summary>
        /// Permet de représenter les liens entre les noeuds par une matrice
        /// </summary>
        /// <returns></returns>
        public int[,] MatriceAdjacence()
        {
            int[,] matrice = new int[this.noeuds.Count, this.noeuds.Count];
            foreach (Lien lien in this.liens)
            {
                //ptet un prblm ici de out of range entre count et ID si l'id est grand
                matrice[lien.Source.Id - 1, lien.Destination.Id - 1] = 1;
                matrice[lien.Destination.Id - 1, lien.Source.Id - 1] = 1;
            }
            return matrice;
        }

        public static void AfficherMatrice<T>(T[,] matrice)
        {
            int lignes = matrice.GetLength(0);
            int colonnes = matrice.GetLength(1);

            for (int i = 0; i < lignes; i++)
            {
                for (int j = 0; j < colonnes; j++)
                {
                    Console.Write(matrice[i, j] + "\t"); // Utilisation de tabulation pour aligner les colonnes
                }
                Console.WriteLine(); // Retour à la ligne après chaque ligne de la matrice
            }
        }


        #endregion
    }
}