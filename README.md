# Room Booking Backend - Projet Ynov M1 Web

## par - [Alixan Balu](https://github.com/Alixanb) et - [Aurélie Runser](https://github.com/Aurelie-Runser)

> Lien vers le repos du frontend : https://github.com/Aurelie-Runser/RoomBooking--Front

## Comment faire tourner le Projet

### Cloner le projet

```
git clone https://gitlab.com/AurelieRunser/RoomBooking--Back
cd RoomBooking--Back
```

### Installer les packages

Afin de télécharger les packages nécessaires au bon fonctionnement du projet, tappez la commande :

```
dotnet restore
```

### Build le projet

```
dotnet build
```

### Lancer le projet

```
dotnet run
```

## Versions

Ce projet a été développé, et donc testé avec la version **8.0.404 de .NET**.

## Fonctionnalités

### Obligatoires

- [x] Demande de réservation (+ modification)
- [x] Catalogue des salles (+ CRUD par les Admin)
- [ ] Envoi d'un rappel avant l'évènement
- [x] Profil Client
- [x] Profil Administrateur
- [x] Demande de réservation client sur mobile
- [x] Export .cal

### Supplémentaires

- [ ] Calendrier interactif des salles
- [ ] Affichage de l'horaire courant
- [x] Filtre de recherche
- [x] Afficher les créneaux disponibles / indisponibles
- [x] Pouvoir annuler une réservation
- [x] Export .csv
- [ ] Export .xlsx
- [x] Historique des réservations
