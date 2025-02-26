namespace ClassLibraryRendu1
{
    public class Noeud
    {
        #region Attributs
        int id;
        List<Noeud> voisins;
        #endregion

        #region Proprietes
        public int Id 
        { 
            get { return this.id; }
            set { this.id = value; } 
        }
        public List<Noeud> Voisins 
        {
            get { return this.voisins; } 
            set { this.voisins = value; }
        }
        #endregion

        #region Constructeur
        public Noeud(int id1)
        {
            this.id = id1;
            this.voisins = new List<Noeud>();
        }
        #endregion
    }
}