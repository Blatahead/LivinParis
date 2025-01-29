namespace ClassLibraryRendu1
{
    public class Noeud
    {
        int id;
        List<Noeud> voisins;
        public int Id { get; set; }
        public List<Noeud> Voisins { get; set; }

        public Noeud(int id1)
        {
            this.id = id1;
            this.voisins = new List<Noeud>();
        }
    }
}
