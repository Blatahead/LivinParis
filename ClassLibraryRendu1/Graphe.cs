using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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

            // ajouter les voisins si ce n'est pas déjà fait
            if (!source.Voisins.Contains(destination))
            {
                source.Voisins.Add(destination);
            }

            if (!destination.Voisins.Contains(source))
            {
                destination.Voisins.Add(source);
            }

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
                    int sommet1 = Convert.ToInt32(parties[0]);
                    int sommet2 = Convert.ToInt32(parties[1]);

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
        /// Affichage d'un dictionnaire dans la console
        /// </summary>
        /// <param name="listeAdjacence"></param>
        public static void AfficherListeAdjacence(Dictionary<int, List<int>> listeAdjacence)
        {
            foreach (KeyValuePair<int,List<int>> noeud in listeAdjacence)
            {
                Console.Write($"Noeud {noeud.Key} : ");
                if (noeud.Value.Count > 0)
                {
                    Console.WriteLine(string.Join(", ", noeud.Value));
                }
                else
                {
                    Console.WriteLine("Aucun voisin");
                }
            }
        }

        /// <summary>
        /// Permet de représenter les liens entre les noeuds par une matrice
        /// </summary>
        /// <returns></returns>
        public int[,] MatriceAdjacence()
        {
            int taille = this.noeuds.Count;
            int[,] matrice = new int[taille, taille];

            foreach (Lien lien in this.liens)
            {
                int source = lien.Source.Id - 1;
                int destination = lien.Destination.Id - 1;

                if (source >= 0 && source < taille && destination >= 0 && destination < taille)
                {
                    matrice[source, destination] = 1;
                    matrice[destination, source] = 1;
                }
                else
                {
                    Console.WriteLine($"ID hors de la matrice ({lien.Source.Id-1}, {lien.Destination.Id-1})");
                }
            }
            return matrice;
        }

        /// <summary>
        /// Affichage d'une matrice dans la console
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrice"></param>
        public static void AfficherMatrice<T>(T[,] matrice)
        {
            int lignes = matrice.GetLength(0);
            int colonnes = matrice.GetLength(1);

            for (int i = 0; i < lignes; i++)
            {
                for (int j = 0; j < colonnes; j++)
                {
                    Console.Write(matrice[i, j]);
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Permet le parcours d'un graphe en profondeur
        /// </summary>
        /// <param name="depart"></param>
        /// <returns></returns>
        public List<int> ParcoursProfondeur(Noeud depart)
        {
            List<int> resultat = new List<int>();
            Stack<Noeud> pile = new Stack<Noeud>();
            HashSet<int> visites = new HashSet<int>();

            pile.Push(depart);

            while (pile.Count > 0)
            {
                Noeud courant = pile.Pop();
                if (!visites.Contains(courant.Id))
                {
                    visites.Add(courant.Id);
                    resultat.Add(courant.Id);

                    foreach (Noeud voisin in courant.Voisins)
                    {
                        if (!visites.Contains(voisin.Id))
                        {
                            pile.Push(voisin);
                        }
                    }
                }
            }
            return resultat;
        }

        /// <summary>
        /// Permet le parcours d'un graphe en largeur
        /// </summary>
        /// <param name="depart"></param>
        /// <returns></returns>
        public List<int> ParcoursLargeur(Noeud depart)
        {
            List<int> resultat = new List<int>();
            Queue<Noeud> file = new Queue<Noeud>();
            HashSet<int> visites = new HashSet<int>();

            file.Enqueue(depart);
            visites.Add(depart.Id);

            while (file.Count > 0)
            {
                Noeud courant = file.Dequeue();
                resultat.Add(courant.Id);

                foreach (var voisin in courant.Voisins)
                {
                    if (!visites.Contains(voisin.Id))
                    {
                        visites.Add(voisin.Id);
                        file.Enqueue(voisin);
                    }
                }
            }
            return resultat;
        }

        /// <summary>
        /// Affiche les sommets parcourus selon le type de parcours
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parcours"></param>
        public void AfficherParcours(string type, List<int> parcours)
        {
            Console.WriteLine($"{type} : {string.Join(" -> ", parcours)}");
        }

        /// <summary>
        /// Renvoie un booleen si le graphe est connexe ou pas
        /// </summary>
        /// <returns></returns>
        public bool EstConnexe()
        {
            List<int> visites = ParcoursLargeur(this.noeuds[0]);
            return visites.Count == this.noeuds.Count;
        }

        /// <summary>
        /// Affichage de tous les liens d'un graphe
        /// </summary>
        public void AfficherLiensGraphe()
        {
            Console.WriteLine("Liste des liens dans le graphe :");
            foreach (Lien lien in this.liens)
            {
                Console.WriteLine($"{lien.Source.Id} - {lien.Destination.Id}");
            }
        }

        /// <summary>
        /// Renvoie un booleen pour savoir si le graph contient un ou des cycles.
        /// Vérifie la présence d'un cycle en lançant un parcours en profondeur sur chaque noeud connexe
        /// </summary>
        /// <returns></returns>
        public bool ContientCycle()
        {
            HashSet<int> visites = new HashSet<int>();

            foreach (var noeud in this.noeuds)
            {
                if (!visites.Contains(noeud.Id))
                {
                    if (DFS_DetectionCycle(noeud, null, visites))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Fonction récursive pour détecter un cycle via DFS
        /// <summary>
        /// Récursivité du parcours en profondeur jusqu'à treouver un cycle
        /// </summary>
        /// <param name="courant"></param>
        /// <param name="parent"></param>
        /// <param name="visites"></param>
        /// <returns></returns>
        private bool DFS_DetectionCycle(Noeud courant, Noeud parent, HashSet<int> visites)
        {
            visites.Add(courant.Id);

            foreach (var voisin in courant.Voisins)
            {
                // si voisin non visité alors appel récursif
                if (!visites.Contains(voisin.Id))
                {
                    if (DFS_DetectionCycle(voisin, courant, visites))
                    {
                        return true;  // Cycle détecté
                    }
                }
                // aussi un cycle si le voisin a déjà été visité mais n'est pas le parent direct
                else if (voisin != parent)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Regarder si un lien n'a pas de retour pour le considéré non orienté
        /// </summary>
        /// <returns></returns>
        public bool EstOriente()
        {
            foreach (var lien in this.Liens)
            {
                if (!lien.Destination.Voisins.Contains(lien.Source))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Affiche le degre des sommets d'un graphe
        /// </summary>
        public void AfficherDegresSommets()
        {
            Console.WriteLine("\nDegré de chaque sommet :");
            foreach (Noeud noeud in this.noeuds)
            {
                Console.WriteLine($"Sommet id : {noeud.Id} : {noeud.Voisins.Count} voisins");
            }
        }
        #endregion
    }
}