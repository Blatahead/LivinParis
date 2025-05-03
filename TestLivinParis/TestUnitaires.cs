using Org.BouncyCastle.Crypto.Utilities;
using System.Text.Json;
using System.Xml.Serialization;

namespace ClassLibrary
{
    [TestClass]
    public sealed class TestUnitaires
    {
        [TestMethod]
        public void Graphe_Triangle()
        {

            var g = new Graphe2();
            var a = new Noeud("A", "Test");
            var b = new Noeud("B", "Test");
            var c = new Noeud("C", "Test");

            g.AjouterNoeud(a);
            g.AjouterNoeud(b);
            g.AjouterNoeud(c);

            g.AjouterLien(a, b, "x");
            g.AjouterLien(b, c, "x");
            g.AjouterLien(c, a, "x");


            var couleurs = WelshPowell.ColorierGraphe(g);
            var nbCouleurs = couleurs.Values.Distinct().Count();


            Assert.AreEqual(3, nbCouleurs);
        }

        [TestMethod]
        public void Graphe_Ligne()
        {

            var g = new Graphe2();
            var a = new Noeud("A", "Test");
            var b = new Noeud("B", "Test");
            var c = new Noeud("C", "Test");
            var d = new Noeud("D", "Test");

            g.AjouterNoeud(a);
            g.AjouterNoeud(b);
            g.AjouterNoeud(c);
            g.AjouterNoeud(d);

            g.AjouterLien(a, b, "x");
            g.AjouterLien(b, c, "x");
            g.AjouterLien(c, d, "x");


            var couleurs = WelshPowell.ColorierGraphe(g);
            var nbCouleurs = couleurs.Values.Distinct().Count();


            Assert.AreEqual(2, nbCouleurs);
        }

        [TestClass]
        public class BellmanFordTests
        {
            //[TestMethod]
            //public void BellmanFord_CalculDistanceCorrecte()
            //{
            //    // Arrange : création d’un graphe simple
            //    var noeuds = new List<StationNoeud>
            //{
            //    new StationNoeud(1,"A",0,0,"1",1,2,2),
            //    new StationNoeud(2,"B",0,0,"1",2,3,2),
            //    new StationNoeud(3,"C",0,0,"1",3,2,2)

            //};

            //    var stations = noeuds.ConvertAll(StationConvertisseurs.NoeudToStation);

            //    // Act
            //    var result = Parcours.BellmanFord(1, stations);

            //    // Assert
            //    Assert.AreEqual(1, result[0].Id);  // Station A
            //    Assert.AreEqual(2, result[1].Id);  // Station B
            //    Assert.AreEqual(3, result[2].Id);  // Station C
            //}

            //[TestMethod]
            //public void 
        }
    }

    [TestClass]
    public class AStarTests
    {
        [TestMethod]
        public void AStar_Trouve_Chemin_Correct()
        {
            // Création du graphe
            var graphe = new Graphe();

            // Ajout des stations (id, nom, lat, lon, ligne)
            var s1 = new StationNoeud(1, "A", 0, 0, "L1");
            var s2 = new StationNoeud(2, "B", 0, 1, "L1");
            var s3 = new StationNoeud(3, "C", 1, 1, "L1");
            var s4 = new StationNoeud(4, "D", 1, 2, "L1");

            graphe.AjouterStation(s1);
            graphe.AjouterStation(s2);
            graphe.AjouterStation(s3);
            graphe.AjouterStation(s4);

            // Connexions (poids = distance)
            graphe.AjouterArc(1, 2, 1, "L1");
            graphe.AjouterArc(2, 3, 1, "L1");
            graphe.AjouterArc(3, 4, 1, "L1");

            // Lancement de A*
            var aStar = new AStar();
            var chemin = aStar.TrouverChemin(graphe, 1, 4);

            // Validation
            Assert.AreEqual(4, chemin.Count);
            Assert.AreEqual("A", chemin[0].Nom);
            Assert.AreEqual("D", chemin[3].Nom);
        }
    }

    [TestClass]
    public class SerializationTests
    {
        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [TestMethod]
        public void DeserializeFromJson_ShouldReturnCorrectObject()
        {
            // Arrange
            var json = "{\"Name\":\"Alice\",\"Age\":30}";
            var expected = new Person { Name = "Alice", Age = 30 };

            // Act
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<Person>(json, options);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.Age, result.Age);
        }
    }


    [TestClass]
    public class SerializationTests2
    {
        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [TestMethod]
        public void DeserializeFromXml_ShouldReturnCorrectObject()
        {
            // Arrange
            var xml = "<Person><Name>Bob</Name><Age>40</Age></Person>";
            var expected = new Person { Name = "Bob", Age = 40 };

            // Act
            var serializer = new XmlSerializer(typeof(Person));
            Person result;
            using (var reader = new StringReader(xml))
            {
                result = (Person)serializer.Deserialize(reader);
            }

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.Age, result.Age);
        }
    }
}



    
