using ClassLibraryRendu1;
using System;
using System.Collections.Generic;
using System.Drawing;

public class GrapheDrawer
{
    #region Attributs
    Graphe graphe;
    int largeurImage;
    int hauteurImage;
    int rayonSommet = 25;
    int minEspacement = 60;
    string cheminImage = "graphe.png";
    Random r = new Random();
    #endregion

    #region Constructeur
    public GrapheDrawer(Graphe graphe1, int largeur1, int hauteur1)
    {
        this.graphe = graphe1;
        this.largeurImage = largeur1;
        this.hauteurImage = hauteur1;
    }
    #endregion

    #region Methodes
    /// <summary>
    /// Trace un graphe à partir de ses liens et ses noeuds
    /// </summary>
    public void DessinerGraphe()
    {
        Bitmap bitmap = new Bitmap(largeurImage, hauteurImage);
        Graphics g = Graphics.FromImage(bitmap);
        // meilleure qualité de rendu
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        // éviter les superpositions
        Dictionary<int, PointF> positions = GenererPositionsSansSuperposition();

        // liens
        Pen penLien = new Pen(Color.Black, 2);
        foreach (var lien in graphe.Liens)
        {
            PointF p1 = positions[lien.Source.Id];
            PointF p2 = positions[lien.Destination.Id];
            g.DrawLine(penLien, p1, p2);
        }

        // sommets
        Brush brushSommet = Brushes.LightBlue;
        Brush brushTexte = Brushes.Black;
        Font font = new Font("Arial", 10, FontStyle.Bold);
        foreach (var noeud in graphe.Noeuds)
        {
            PointF p = positions[noeud.Id];
            RectangleF cercle = new RectangleF(p.X - rayonSommet, p.Y - rayonSommet, rayonSommet * 2, rayonSommet * 2);

            g.FillEllipse(brushSommet, cercle);
            // Id du sommet
            g.DrawString(noeud.Id.ToString(), font, brushTexte, p.X - 10, p.Y - 10); 
        }

        bitmap.Save(cheminImage, System.Drawing.Imaging.ImageFormat.Png);
        g.Dispose();
        bitmap.Dispose();
    }

    /// <summary>
    /// Génère un Dictionnaire de coordonnées non superposées
    /// </summary>
    /// <returns></returns>
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
                float x = r.Next(rayonSommet, largeurImage - rayonSommet);
                float y = r.Next(rayonSommet, hauteurImage - rayonSommet);
                nouvellePosition = new PointF(x, y);

                // test de distance
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

            positions[noeud.Id] = nouvellePosition;
            pointsUtilises.Add(nouvellePosition);
        }
        return positions;
    }

    /// <summary>
    /// Calcul la distance entre deux points
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    private float Distance(PointF p1, PointF p2)
    {
        return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
    }
    #endregion
}
