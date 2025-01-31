using ClassLibraryRendu1;

namespace WinFormsRendu1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            ChargerGraphe();
        }

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


                //// Afficher la matrice d'adjacence
                //AfficherMatriceAdjacence();

                //// Afficher la liste d'adjacence
                //AfficherListeAdjacence();

                // Effectuer les parcours et afficher les résultats
                if (graphe.Noeuds.Count > 0)
                {
                    Noeud depart = graphe.Noeuds[0];
                    AfficherParcoursLargeur(depart, graphe);
                    AfficherParcoursProfondeur(depart, graphe);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Erreur lors du chargement du graphe : {e.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void AfficherParcoursLargeur(Noeud depart, Graphe graphe)
        {
            listParcoursLargeur.Items.Clear();
            List<int> parcours = graphe.ParcoursLargeur(depart);
            listParcoursLargeur.Items.AddRange(parcours.Select(p => $"{p}").ToArray());
        }

        private void AfficherParcoursProfondeur(Noeud depart, Graphe graphe)
        {
            listParcoursProfondeur.Items.Clear();
            List<int> parcours = graphe.ParcoursProfondeur(depart);
            listParcoursProfondeur.Items.AddRange(parcours.Select(p => $"{p}").ToArray());
        }

        /// remettre le commentaire de l'affichage console
    }
}
