using ClassLibraryRendu1;
using System;
using System.Collections.Generic;
using System.Drawing;

public class GrapheDrawer
{
    private Graphe graphe;
    private int largeurImage;
    private int hauteurImage;
    private int rayonSommet = 25; // Taille des cercles des sommets
    private int minEspacement = 60; // Distance minimale entre les sommets
    private string cheminImage = "graphe.png";
    private Random rand = new Random();

    public GrapheDrawer(Graphe graphe, int largeur, int hauteur)
    {
        this.graphe = graphe;
        this.largeurImage = largeur;
        this.hauteurImage = hauteur;
    }

    public void DessinerGraphe()
    {
        Bitmap bitmap = new Bitmap(largeurImage, hauteurImage);
        Graphics g = Graphics.FromImage(bitmap);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        // Générer les positions des sommets en évitant les superpositions
        Dictionary<int, PointF> positions = GenererPositionsSansSuperposition();

        // Dessiner les liens
        Pen penLien = new Pen(Color.Black, 2);
        foreach (var lien in graphe.Liens)
        {
            PointF p1 = positions[lien.Source.Id];
            PointF p2 = positions[lien.Destination.Id];
            g.DrawLine(penLien, p1, p2);
        }

        // Dessiner les sommets
        Brush brushSommet = Brushes.LightBlue;
        Brush brushTexte = Brushes.Black;
        Font font = new Font("Arial", 10, FontStyle.Bold);
        foreach (var noeud in graphe.Noeuds)
        {
            PointF p = positions[noeud.Id];
            RectangleF cercle = new RectangleF(p.X - rayonSommet, p.Y - rayonSommet, rayonSommet * 2, rayonSommet * 2);

            g.FillEllipse(brushSommet, cercle); // Remplissage du cercle
            g.DrawEllipse(Pens.Black, cercle);  // Contour du cercle
            g.DrawString(noeud.Id.ToString(), font, brushTexte, p.X - 10, p.Y - 10); // Numéro du sommet
        }

        // Sauvegarde de l'image
        bitmap.Save(cheminImage, System.Drawing.Imaging.ImageFormat.Png);
        g.Dispose();
        bitmap.Dispose();
    }

    private Dictionary<int, PointF> GenererPositionsSansSuperposition()
    {
        Dictionary<int, PointF> positions = new Dictionary<int, PointF>();
        List<PointF> pointsUtilises = new List<PointF>();
        int essaisMax = 1000;

        foreach (var noeud in graphe.Noeuds)
        {
            PointF nouvellePosition;
            int essais = 0;
            bool positionValide;

            do
            {
                // Générer une position aléatoire en utilisant toute la largeur et hauteur
                float x = rand.Next(rayonSommet, largeurImage - rayonSommet);
                float y = rand.Next(rayonSommet, hauteurImage - rayonSommet);
                nouvellePosition = new PointF(x, y);

                // Vérifier si elle est trop proche d'une autre
                positionValide = true;
                foreach (var point in pointsUtilises)
                {
                    if (Distance(nouvellePosition, point) < minEspacement)
                    {
                        positionValide = false;
                        break;
                    }
                }

                essais++;
            }
            while (!positionValide && essais < essaisMax);

            // Ajouter la position validée
            positions[noeud.Id] = nouvellePosition;
            pointsUtilises.Add(nouvellePosition);
        }

        return positions;
    }

    private float Distance(PointF p1, PointF p2)
    {
        return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
    }
}
