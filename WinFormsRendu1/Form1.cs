using ClassLibraryRendu1;

namespace WinFormsRendu1
{
    public partial class Form1 : Form
    {
        #region Constructeur
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            ChargerGraphe();
        }
        #endregion

        #region Methodes
        /// <summary>
        /// Importation du graphe depuis le fichier matrice
        /// </summary>
        private void ChargerGraphe()
        {
            try
            {
                Graphe graphe = Graphe.LectureMTX("./../../../../ClassLibraryRendu1/soc-karate.mtx");

                labelOrdreGraphe.Text = $"{graphe.Noeuds.Count}";
                labelTailleGraphe.Text = $"{graphe.Liens.Count}";
                LabelEstOriente.Text = $"{graphe.EstOriente()}";
                labelEstConnexe.Text = $"{graphe.EstConnexe()}";
                labelEstPondere.Text = $"False";

                if (graphe.Noeuds.Count > 0)
                {
                    Noeud depart = graphe.Noeuds[0];
                    AfficherParcoursLargeur(depart, graphe);
                    AfficherParcoursProfondeur(depart, graphe);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Erreur lors du chargement du graphe : {e.Message}");
            }
        }

        /// <summary>
        /// Permet de charger l'image du graphe dans son emplacement lors du chargement de la page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            Graphe graphe = Graphe.LectureMTX("./../../../../ClassLibraryRendu1/soc-karate.mtx");

            int largeur = pictureBox1.Width;
            int hauteur = pictureBox1.Height;

            GrapheDrawer drawer = new GrapheDrawer(graphe, largeur, hauteur);
            drawer.DessinerGraphe();

            pictureBox1.Image = Image.FromFile("graphe.png");
        }

        /// <summary>
        /// Affiche la liste des sommets parcourus en largeur dans l'emplacement "listParcoursLargeur" de l'interface
        /// </summary>
        /// <param name="depart"></param>
        /// <param name="graphe"></param>
        private void AfficherParcoursLargeur(Noeud depart, Graphe graphe)
        {
            listParcoursLargeur.Items.Clear();
            List<int> parcours = graphe.ParcoursLargeur(depart);
            listParcoursLargeur.Items.AddRange(parcours.Select(p => $"{p}").ToArray());
        }

        /// <summary>
        /// Affiche la liste des sommets parcourus en profondeur dans l'emplacement "listParcoursLargeur" de l'interface
        /// </summary>
        /// <param name="depart"></param>
        /// <param name="graphe"></param>
        private void AfficherParcoursProfondeur(Noeud depart, Graphe graphe)
        {
            listParcoursProfondeur.Items.Clear();
            List<int> parcours = graphe.ParcoursProfondeur(depart);
            listParcoursProfondeur.Items.AddRange(parcours.Select(p => $"{p}").ToArray());
        }
        #endregion
    }
}