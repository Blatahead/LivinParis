# 📌 LivinParis - Problème Scientifique & Informatique
## _Rendu Intermédiaire n°1_
> **Projet réalisé en A2 - Collaborateurs : Mathieu & Antonin**

---

## 🚀 Description du Rendu 1
Ce rendu intermédiaire s'articule autour de **deux grandes parties** :
1. **Base de Données (BDD)** : Conception et exploitation d'une base de données.
2. **Graphe** : Implémentation et manipulation d'une structure de graphe.

L'idée est d'avoir une première version de notre BDD et de s'entraîner au parcours de graphes.

---

## 📂 Partie 1 - Base de Données (BDD)

### 🎯 Objectifs
- Création d'un **Modèle Entité/Association (E/A)**
- **Schéma Relationnel (MLD)** associé
- **Peuplement des tables** avec des données fictives
- Exécution de **requêtes SELECT** mono-tables et simples

### 📌 Contenu
- 📄 **Modèle E/A** : Représentation graphique du schéma conceptuel
- 📊 **Schéma Relationnel** : Tables et relations normalisées
- 📜 **Script SQL** : Structure des tables + insertion de données
- 🔎 **Requêtes SELECT** : Interrogation de la BDD

### 🛠 Technologies Utilisées
- **MySQL**
- **SQL** : manipulation des données et requêtes

---

## 📂 Partie 2 - Structure de Graphe

### 🎯 Objectifs
- Instanciation d'une **structure de graphe** minimale :
  - Classe `Noeud`
  - Classe `Lien`
  - Classe `Graphe`
- Implémentation de **parcours en profondeur (DFS) et largeur (BFS)**
- Détection automatique :
  - Graphe **connexe** : oui ou non
  - Présence d'un **circuit** : oui ou non
- **Visualisation** du graphe

### 📌 Contenu
- 📜 **Implémentation des classes** (Noeud, Lien, Graphe)
- 🔄 **Algorithmes de parcours DFS & BFS**
- 🔍 **Détection de connexions et circuits**
- 🖥 **Affichage graphique du graphe**

### 🛠 Technologies Utilisées
- **C#** pour l'implémentation
- **System.Drawing** pour le dessin du graphe
- **Windows Forms** pour la visualisation des données à l'écran

---

## 📂 Partie 3 - Tests unitaires
### 🎯 Objectifs
- Tester de manière simple chacune des fonctions présentes
- Permet de débuguer facilement
- Prévoir un certain nombre d'erreurs sur des cas plus ou moins simples

---

## 📋 Organisation du Projet

📁 `bdd/` → Scripts SQL, modèles, requêtes

📁 `ClassLibraryRendu1/` → Implémentation des classes et visualisation

📁 `WinFormsRendu1/` → Racine de la solution et classes Windows Forms

📁 `TestProjectRendu1/` → Tests unitaires associés au rendu intermédiaire n°1

📁 `docs/` → Schéma E/A, rapports, et captures d'écran

---

## 📅 Rendu 1 (1er Mars) - Livrables

✅ **Partie BDD** :
- Schéma Entité-Association (E/A)
- Schéma Relationnel
- Script SQL de création des tables
- Peuplement des tables avec les fichiers `Clients.csv`, `Cuisiniers.csv`, `Commandes.csv`
- Quelques requêtes SQL mono-tables et simples

✅ **Partie Graphe** :
- Classes `Noeud`, `Lien`, `Graphe` en C#
- Instanciation du graphe à partir du fichier d'exemple
- Deux méthodes de stockage : Liste et Matrice d'adjacence
- Implémentation DFS & BFS avec affichage des sommets
- Détection de connexions et circuits
- Visualisation graphique du graphe

✅ **Rapport** :
- Explications sur la conception
- Détails sur l'implémentation des graphes
- Screenshots de la visualisation
- Prompts d'IA utilisés pour aider à la visualisation (si applicable)

---

## 👥 Auteurs
👨‍💻 **[Mathieu]** - [Github](https://github.com/Thieuthieu77)

👩‍💻 **[Antonin]** - [Github](https://github.com/Blatahead)

---