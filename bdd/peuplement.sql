INSERT INTO Utilisateur(Identifiant,Mdp) VALUES ('DDEDF','Abcd123');
INSERT INTO Utilisateur(Identifiant,Mdp) VALUES ('Olive','Afcd431');
INSERT INTO Utilisateur(Identifiant,Mdp) VALUES ('Charlou','zz31');
INSERT INTO Utilisateur(Identifiant,Mdp) VALUES ('Champion2018','19982018');
INSERT INTO Utilisateur(Identifiant,Mdp) VALUES ('Champion98','Ballondor');
INSERT INTO Utilisateur(Identifiant,Mdp) VALUES ('ENGIE92','Entreprise47');

INSERT INTO Client_(Numéro_client,Identifiant) VALUES ('1', 'DDEDF');
INSERT INTO Client_(Numéro_client,Identifiant) VALUES ('2', 'Olive');
INSERT INTO Client_(Numéro_client,Identifiant) VALUES ('3', 'Charlou');
INSERT INTO Client_(Numéro_client,Identifiant) VALUES ('4', 'Champion2018');
INSERT INTO Client_(Numéro_client,Identifiant) VALUES ('5', 'Champion98');
INSERT INTO Client_(Numéro_client,Identifiant) VALUES ('6', 'ENGIE92');

INSERT INTO Particulier(Numéro_particulier,Prénom_particulier,Nom_particulier,Adresse_particulier,Numéro_client) VALUES ('0749146679', 'Didier', 'Deschamps', '1 rue du pastice', '1');
INSERT INTO Particulier(Numéro_particulier,Prénom_particulier,Nom_particulier,Adresse_particulier,Numéro_client) VALUES ('0708091011', 'Olivia', 'Defeuillet', '12 allée de Montmartre', '2');
INSERT INTO Particulier(Numéro_particulier,Prénom_particulier,Nom_particulier,Adresse_particulier,Numéro_client) VALUES ('0634035412', 'Charles', 'Douvier', '32 du rond point', '3');
INSERT INTO Particulier(Numéro_particulier,Prénom_particulier,Nom_particulier,Adresse_particulier,Numéro_client) VALUES ('0744146249', 'Ngolo', 'Kante', '1 rue du champion', '4');
INSERT INTO Particulier(Numéro_particulier,Prénom_particulier,Nom_particulier,Adresse_particulier,Numéro_client) VALUES ('0794937810', 'Zinédine', 'Zidane', '1 allée de la réussite', '5');

INSERT INTO Entreprise(Numéro_siret,Nom_entreprise,Nom_référent,Adresse_entreprise,Numéro_client) VALUES ('569305','Engie','Apagnan','1 passage du Mongole','6');

INSERT INTO Cuisinier(Numéro_cuisinier,Prénom_cuisinier,Adresse_cuisinier,Identifiant,Nom_particulier) VALUES ('0749146679','Didier','1 rue du pastice','DDEDF','Deschamps');
INSERT INTO Cuisinier(Numéro_cuisinier,Prénom_cuisinier,Adresse_cuisinier,Identifiant,Nom_particulier) VALUES ('0634035412','Charles','32 du rond point','Charlou','Douvier');

INSERT INTO Commande(Numéro_commande,Prix_commande,Note_commande,Identifiant) VALUES ('10', '26','4','Champion98');
INSERT INTO Commande(Numéro_commande,Prix_commande,Note_commande,Identifiant) VALUES ('11', '20','5','Olive');

INSERT INTO Plat(Numéro_plat,Nom_plat,Nombre_de_personne_plat,Nationalité_plat,Date_péremption_plat,prix_plat,Ingrédients_plat,Numéro_cuisinier,Type_plat,Régime_alimentaire_plat,Date_fabrication_plat,Numéro_commande) VALUES ('17','Chili con carne','4','Etats Unis','25-03-06','26','épices,boeuf,tomates,poivrons,sel','0749146679','Omnivore','plat principal','25-02-27','10');
INSERT INTO Plat(Numéro_plat,Nom_plat,Nombre_de_personne_plat,Nationalité_plat,Date_péremption_plat,prix_plat,Ingrédients_plat,Numéro_cuisinier,Type_plat,Régime_alimentaire_plat,Date_fabrication_plat,Numéro_commande) VALUES ('21','Salade Caesar','3','Italie','25-03-03','20','lettue, mozarella, tomate, lardons, oeuf, sel','0634035412','Omnivore','entrée','25-02-26','11');

