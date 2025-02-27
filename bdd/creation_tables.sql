CREATE DATABASE IF NOT EXISTS AntoMath;
USE AntoMath;
CREATE TABLE Utilisateur(
   Identifiant VARCHAR(50),
   Mdp VARCHAR(50) NOT NULL,
   Numéro_utilisateur INT,
   Adresse_mail_utilisateur VARCHAR(50),
   PRIMARY KEY(Identifiant)
);

CREATE TABLE Cuisinier(
   Numéro_cuisinier INT,
   Prénom_cuisinier VARCHAR(50),
   Nom_particulier VARCHAR(50),
   Adresse_cuisinier VARCHAR(50),
   Identifiant VARCHAR(50) NOT NULL,
   PRIMARY KEY(Numéro_cuisinier),
   UNIQUE(Identifiant),
   FOREIGN KEY(Identifiant) REFERENCES Utilisateur(Identifiant)
);

CREATE TABLE Client_(
   Numéro_client INT,
   Identifiant VARCHAR(50) NOT NULL,
   PRIMARY KEY(Numéro_client),
   UNIQUE(Identifiant),
   FOREIGN KEY(Identifiant) REFERENCES Utilisateur(Identifiant)
);

CREATE TABLE Commande(
   Numéro_commande INT,
   Prix_commande DECIMAL(15,2),
   Note_commande INT,
   Identifiant VARCHAR(50) NOT NULL,
   PRIMARY KEY(Numéro_commande),
   FOREIGN KEY(Identifiant) REFERENCES Utilisateur(Identifiant)
);

CREATE TABLE Plat(
   Numéro_plat INT,
   Nom_plat VARCHAR(50),
   Nombre_de_personne_plat INT,
   Type_plat ENUM('varchar'),
   Nationalité_plat ENUM('varchar'),
   Date_péremption_plat DATE,
   prix_plat DECIMAL(10,2),
   Ingrédients_plat VARCHAR(50),
   Régime_alimentaire_plat ENUM('varchar'),
   Photo_plat VARCHAR(50),
   Date_fabrication_plat DATE,
   Numéro_commande INT,
   Numéro_cuisinier INT NOT NULL,
   PRIMARY KEY(Numéro_plat),
   FOREIGN KEY(Numéro_commande) REFERENCES Commande(Numéro_commande),
   FOREIGN KEY(Numéro_cuisinier) REFERENCES Cuisinier(Numéro_cuisinier),
   FOREIGN KEY(Numéro_cuisinier) REFERENCES Cuisinier(Numéro_cuisinier)
);

CREATE TABLE Entreprise(
   Numéro_siret INT,
   Nom_entreprise VARCHAR(50) NOT NULL,
   Nom_référent VARCHAR(50),
   Adresse_entreprise VARCHAR(50),
   Numéro_client INT NOT NULL,
   PRIMARY KEY(Numéro_siret),
   UNIQUE(Numéro_client),
   FOREIGN KEY(Numéro_client) REFERENCES Client_(Numéro_client)
);

CREATE TABLE Particulier(
   Numéro_particulier INT,
   Prénom_particulier VARCHAR(50),
   Nom_particulier VARCHAR(50),
   Adresse_particulier VARCHAR(50),
   Numéro_client INT NOT NULL,
   PRIMARY KEY(Numéro_particulier),
   UNIQUE(Numéro_client),
   FOREIGN KEY(Numéro_client) REFERENCES Client_(Numéro_client)
);