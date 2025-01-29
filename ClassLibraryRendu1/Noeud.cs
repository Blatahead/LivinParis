namespace ClassLibraryRendu1
{
    public class Noeud
    {
        int id;
        List<Noeud> voisins;

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

        public Noeud(int id1)
        {
            this.id = id1;
            this.voisins = new List<Noeud>();
        }
    }
}
