# BEHGest Pro 🚀

> **Logiciel de Gestion Intégrée d'Entreprise : Formations, Apprenants, Stagiaires, Commandes, Paiements & Ressources Humaines**

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/UI-WPF%20Desktop-1565C0?style=flat&logo=windows)](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
[![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL%2016+-316192?style=flat&logo=postgresql)](https://www.postgresql.org/)
[![Material Design](https://img.shields.io/badge/Design-Material%20Design-00ACC1?style=flat&logo=materialdesign)](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)
[![QuestPDF](https://img.shields.io/badge/PDF-QuestPDF%20Fluent-2E7D32?style=flat)](https://www.questpdf.com/)

**BEHGest Pro** est une application desktop professionnelle moderne conçue pour l'environnement Windows, développée en **C# WPF (.NET 8)** selon les principes de la **Clean Architecture** et du pattern **MVVM**. Connectée à une base de données **PostgreSQL**, elle centralise l'ensemble des opérations administratives, pédagogiques, financières et de ressources humaines d'une structure professionnelle.

---

## ✨ Fonctionnalités Clés & Modules Métier

L'application est découpée en **6 modules métiers interconnectés**, accessibles depuis une barre de navigation latérale ergonomique avec support du thème **Clair / Sombre** :

### 1. 🎓 Gestion des Apprenants & Formations
* Suivi des inscriptions pédagogiques et historiques des cours.
* Gestion des catalogues de formation (frais d'inscription, mensualités, durées).
* Génération automatique de **modèles d'attestations de formation** officiels.

### 2. 💼 Suivi des Stagiaires
* Gestion des contrats de stage, thèmes de recherche et encadreurs assignés.
* Suivi des présences et des périodes d'évaluation.
* Génération instantanée d'**attestations de fin de stage** certifiées.

### 3. 👥 Ressources Humaines & Employés
* Fiches complètes du personnel (Matricule, Poste, Département, Coordonnées).
* Attribution et suivi des responsabilités internes.
* Génération et impression de **badges d'identification professionnels** (Format A7).

### 4. 💰 Gestion des Salaires (Paie)
* Calcul automatisé ou manuel des rémunérations : salaire de base, primes, retenues et avances déduites.
* Validation hiérarchique des paiements avec suivi des statuts (*En attente*, *Payé*).
* Génération de **bulletins de paie (fiches de paie)** détaillés et conformes au format XAF.

### 5. 📦 Prestations & Commandes
* Enregistrement des bons de commande clients / donneurs d'ordre.
* Assignation des dossiers aux employés responsables et suivi des livraisons.
* Impression de **bons de commande officiels** chiffrés et formatés.

### 6. 💳 Trésorerie & Reçus de Paiement
* Centralisation des encaissements (inscriptions aux formations, règlements de commandes).
* Suivi des montants versés et des soldes restants (paiements partiels ou intégraux).
* Génération instantanée de **reçus de paiement** professionnels avec référence unique.

---

## 🏗️ Architecture de la Solution

Le projet adopte une **Clean Architecture (N-Tier)** stricte garantissant maintenabilité, testabilité et découplage :

```text
BEHGestPro.sln
│
├── 📁 BEHGestPro.Domain/           # Cœur métier : Entités, Enums et Interfaces Repository
├── 📁 BEHGestPro.Application/      # Logique applicative : Services, DTOs, Validations métier
├── 📁 BEHGestPro.Infrastructure/   # Accès aux données : Entity Framework Core 8, DbContext, PostgreSQL
├── 📁 BEHGestPro.Documents/        # Moteur d'impression : Templates PDF fluides via QuestPDF
└── 📁 BEHGestPro.UI/               # Couche Présentation : Vues WPF (XAML), ViewModels (CommunityToolkit)
```

### 🛠️ Stack Technique & Librairies
* **Runtime :** .NET 8.0 (WPF Desktop)
* **Base de données :** PostgreSQL (Base par défaut : `behgestion`)
* **ORM :** Entity Framework Core 8 (`Npgsql.EntityFrameworkCore.PostgreSQL`)
* **Framework MVVM :** `CommunityToolkit.Mvvm` (Génération de code source, `ObservableObject`, `RelayCommand`)
* **UI & Animations :** `MaterialDesignThemes` & `MaterialDesignColors`
* **Génération PDF :** `QuestPDF` (Moteur fluent haute performance, zéro dépendance externe)
* **Sécurité & Hash :** `BCrypt.Net-Next`

---

## 🚀 Prérequis & Installation

### 1. Prérequis Système
* **SDK .NET 8.0** ou supérieur installé sur la machine.
* **PostgreSQL 16+** installé et en cours d'exécution.
* *Optionnel :* Visual Studio 2022 ou JetBrains Rider.

### 2. Configuration de la Base de Données
1. Assurez-vous que votre serveur PostgreSQL est actif sur le port `5432`.
2. Créez une base de données vide nommée `behgestion` (ou laissez Entity Framework la créer automatiquement).
3. Ouvrez le fichier de configuration `BEHGestPro.UI/appsettings.json` et adaptez la chaîne de connexion si nécessaire :
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=behgestion;Username=postgres;Password=VOTRE_MOT_DE_PASSE;Include Error Detail=true;"
     }
   }
   ```

### 3. Application des Migrations & Lancement
Ouvrez un terminal à la racine du projet (`d:\BEHGest Pro`) et exécutez les commandes suivantes :

```powershell
# 1. Restauration des dépendances NuGet
dotnet restore

# 2. Compilation de la solution
dotnet build

# 3. Application des migrations Entity Framework sur PostgreSQL
dotnet ef database update --project BEHGestPro.Infrastructure --startup-project BEHGestPro.UI

# 4. Lancement de l'application
dotnet run --project BEHGestPro.UI/BEHGestPro.UI.csproj
```

---

## 🖨️ Moteur de Génération Documentaire (PDF)

L'application intègre un service de publipostage PDF centralisé (`PdfService`) qui compile et ouvre instantanément les documents certifiés :
* Aucune erreur de rendu de conteneur : conception fluide respectant scrupuleusement les contraintes d'arborescence QuestPDF.
* En-têtes, tableaux de données chiffrés, totaux en surbrillance, zones de signature et pieds de page horodatés.

---

## 🎨 Conception UI / UX

* **Grilles de données modernes (`DataGrid`) :** Formatage explicite des colonnes, alignement monétaire à droite (format `XAF`), badges de statuts colorés et barre d'actions rapides dédiée par ligne (Imprimer 🖨️, Éditer ✏️, Supprimer 🗑️).
* **Boîtes de dialogue contextuelles :** Formulaires modaux fluides intégrés au centre de la fenêtre parente sans perte de focus.
* **Notifications & Rétroaction :** Alertes visuelles instantanées lors des validations et suppressions.

---

## 📝 Licence & Contribution
Projet privé développé pour la gestion interne **BEH GESTION**. Tous droits réservés.
