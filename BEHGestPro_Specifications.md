# BEHGest Pro — Spécifications Techniques Complètes

> **Logiciel de Gestion Intégrée : Formation, Stagiaires, Commandes, Paiements & RH**
> Version cible : 1.0.0 — Stack : C# (.NET 8 WPF) · PostgreSQL (`behgestion`) · Material Design

---

## Table des matières

1. [Vue d'ensemble du projet](#1-vue-densemble-du-projet)
2. [Stack technique](#2-stack-technique)
3. [Architecture de la solution](#3-architecture-de-la-solution)
4. [Modèle de données (Schéma PostgreSQL)](#4-modèle-de-données-schéma-postgresql)
5. [Modules fonctionnels](#5-modules-fonctionnels)
   - 5.1 [Gestion des Apprenants](#51-gestion-des-apprenants)
   - 5.2 [Gestion des Stagiaires](#52-gestion-des-stagiaires)
   - 5.3 [Gestion des Formations](#53-gestion-des-formations)
   - 5.4 [Gestion des Paiements](#54-gestion-des-paiements)
   - 5.5 [Gestion des Commandes](#55-gestion-des-commandes)
   - 5.6 [Gestion des Salaires](#56-gestion-des-salaires)
6. [Génération de documents (PDF)](#6-génération-de-documents-pdf)
7. [UI/UX — Design System](#7-uiux--design-system)
8. [Sécurité & Authentification](#8-sécurité--authentification)
9. [Structure des projets C#](#9-structure-des-projets-c)
10. [Conventions de code](#10-conventions-de-code)
11. [Règles de génération pour l'IA](#11-règles-de-génération-pour-lia)

---

## 1. Vue d'ensemble du projet

**BEHGest Pro** est une application desktop Windows développée en C# WPF (.NET 8), connectée à une base de données PostgreSQL nommée `behgestion`. Elle centralise six domaines métier au sein d'une interface unifiée, moderne et fluide.

| Attribut | Valeur |
|---|---|
| Nom du logiciel | **BEHGest Pro** |
| Type d'application | Desktop Windows (WPF) |
| Framework | .NET 8 |
| Base de données | PostgreSQL — base : `behgestion` |
| ORM | Entity Framework Core 8 (Npgsql provider) |
| Langue de l'interface | Français |
| Résolution cible | 1366×768 minimum, 1920×1080 recommandé |
| Thème visuel | Dark/Light switch — Palette primaire : `#1565C0` (bleu nuit) + `#00ACC1` (cyan accent) |

---

## 2. Stack technique

### Packages NuGet obligatoires

```xml
<!-- ORM & Base de données -->
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.*" />

<!-- UI -->
<PackageReference Include="MaterialDesignThemes" Version="5.*" />
<PackageReference Include="MaterialDesignColors" Version="3.*" />

<!-- Génération PDF -->
<PackageReference Include="QuestPDF" Version="2024.*" />

<!-- Utilitaires -->
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.*" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.*" />
<PackageReference Include="BCrypt.Net-Next" Version="4.*" />
<PackageReference Include="SkiaSharp" Version="2.*" />
<PackageReference Include="QRCoder" Version="1.*" />
```

### Chaîne de connexion (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=behgestion;Username=postgres;Password=YOUR_PASSWORD;Include Error Detail=true;"
  },
  "AppSettings": {
    "AppName": "BEHGest Pro",
    "Version": "1.0.0",
    "LogoPath": "Assets/logo.png",
    "CompanyName": "",
    "CompanyAddress": "",
    "CompanyPhone": "",
    "DefaultCurrency": "XAF"
  }
}
```

---

## 3. Architecture de la solution

```
BEHGestPro.sln
│
├── BEHGestPro.Domain/               # Entités, enums, interfaces de repository
│   ├── Entities/
│   │   ├── Apprenant.cs
│   │   ├── Stagiaire.cs
│   │   ├── Formation.cs
│   │   ├── SessionFormation.cs
│   │   ├── InscriptionFormation.cs
│   │   ├── Paiement.cs
│   │   ├── Commande.cs
│   │   ├── Employe.cs
│   │   ├── Salaire.cs
│   │   ├── AvanceSalaire.cs
│   │   └── Utilisateur.cs
│   ├── Enums/
│   │   ├── StatutCommande.cs
│   │   ├── TypePaiement.cs
│   │   └── StatutSession.cs
│   └── Interfaces/
│       └── IRepository.cs
│
├── BEHGestPro.Infrastructure/       # EF Core, Repositories, Migrations
│   ├── Data/
│   │   └── BehGestionDbContext.cs
│   ├── Repositories/
│   │   ├── ApprenantRepository.cs
│   │   ├── StagiaireRepository.cs
│   │   ├── FormationRepository.cs
│   │   ├── PaiementRepository.cs
│   │   ├── CommandeRepository.cs
│   │   └── SalaireRepository.cs
│   └── Migrations/
│
├── BEHGestPro.Application/          # Services métier, DTOs, logique
│   ├── Services/
│   │   ├── ApprenantService.cs
│   │   ├── StagiaireService.cs
│   │   ├── FormationService.cs
│   │   ├── PaiementService.cs
│   │   ├── CommandeService.cs
│   │   └── SalaireService.cs
│   ├── DTOs/
│   └── Mappers/
│
├── BEHGestPro.Documents/            # Génération PDF (QuestPDF)
│   ├── Attestations/
│   │   ├── AttestationFormationDocument.cs
│   │   └── AttestationStageDocument.cs
│   ├── Recus/
│   │   ├── RecuPaiementDocument.cs
│   │   └── RecuCommandeDocument.cs
│   ├── Fiches/
│   │   └── FicheDePayeDocument.cs
│   └── Badges/
│       └── BadgeEmployeDocument.cs
│
└── BEHGestPro.UI/                   # WPF — Vues, ViewModels, Controls
    ├── App.xaml
    ├── App.xaml.cs
    ├── Assets/
    │   ├── logo.png
    │   └── Fonts/
    ├── Styles/
    │   ├── BaseStyles.xaml
    │   ├── ButtonStyles.xaml
    │   ├── DataGridStyles.xaml
    │   └── Colors.xaml
    ├── Views/
    │   ├── MainWindow.xaml
    │   ├── LoginView.xaml
    │   ├── Apprenants/
    │   ├── Stagiaires/
    │   ├── Formations/
    │   ├── Paiements/
    │   ├── Commandes/
    │   └── Salaires/
    └── ViewModels/
        ├── MainViewModel.cs
        ├── Apprenants/
        ├── Stagiaires/
        ├── Formations/
        ├── Paiements/
        ├── Commandes/
        └── Salaires/
```

**Pattern obligatoire** : MVVM strict via `CommunityToolkit.Mvvm`. Toute logique métier dans les services `Application`. Les ViewModels n'appellent jamais directement EF Core.

---

## 4. Modèle de données (Schéma PostgreSQL)

> Toutes les tables ont : `id SERIAL PRIMARY KEY`, `created_at TIMESTAMPTZ DEFAULT NOW()`, `updated_at TIMESTAMPTZ`.

### 4.1 Table `utilisateurs`
```sql
CREATE TABLE utilisateurs (
    id          SERIAL PRIMARY KEY,
    nom         VARCHAR(100) NOT NULL,
    prenom      VARCHAR(100) NOT NULL,
    email       VARCHAR(150) UNIQUE NOT NULL,
    mot_de_passe_hash VARCHAR(255) NOT NULL,
    role        VARCHAR(50) NOT NULL DEFAULT 'operateur',  -- 'admin' | 'operateur' | 'lecteur'
    actif       BOOLEAN DEFAULT TRUE,
    created_at  TIMESTAMPTZ DEFAULT NOW(),
    updated_at  TIMESTAMPTZ DEFAULT NOW()
);
```

### 4.2 Table `apprenants`
```sql
CREATE TABLE apprenants (
    id              SERIAL PRIMARY KEY,
    matricule       VARCHAR(30) UNIQUE NOT NULL,
    nom             VARCHAR(100) NOT NULL,
    prenom          VARCHAR(100) NOT NULL,
    date_naissance  DATE,
    sexe            CHAR(1),
    telephone       VARCHAR(20),
    email           VARCHAR(150),
    adresse         TEXT,
    photo_path      VARCHAR(255),
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW()
);
```

### 4.3 Table `formations`
```sql
CREATE TABLE formations (
    id              SERIAL PRIMARY KEY,
    code            VARCHAR(30) UNIQUE NOT NULL,
    intitule        VARCHAR(200) NOT NULL,
    description     TEXT,
    duree_heures    INTEGER NOT NULL,
    cout            NUMERIC(12,2) NOT NULL DEFAULT 0,
    categorie       VARCHAR(100),
    programme       TEXT,
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW()
);
```

### 4.4 Table `sessions_formation`
```sql
CREATE TABLE sessions_formation (
    id              SERIAL PRIMARY KEY,
    formation_id    INTEGER NOT NULL REFERENCES formations(id),
    code_session    VARCHAR(50) UNIQUE NOT NULL,
    date_debut      DATE NOT NULL,
    date_fin        DATE NOT NULL,
    lieu            VARCHAR(200),
    formateur       VARCHAR(200),
    capacite_max    INTEGER DEFAULT 30,
    statut          VARCHAR(30) DEFAULT 'ouverte',  -- 'ouverte' | 'fermee' | 'annulee' | 'terminee'
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW()
);
```

### 4.5 Table `inscriptions_formation`
```sql
CREATE TABLE inscriptions_formation (
    id              SERIAL PRIMARY KEY,
    apprenant_id    INTEGER NOT NULL REFERENCES apprenants(id),
    session_id      INTEGER NOT NULL REFERENCES sessions_formation(id),
    date_inscription DATE DEFAULT CURRENT_DATE,
    statut          VARCHAR(30) DEFAULT 'inscrit',  -- 'inscrit' | 'en_cours' | 'certifie' | 'abandonne'
    note_finale     NUMERIC(5,2),
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(apprenant_id, session_id)
);
```

### 4.6 Table `stagiaires`
```sql
CREATE TABLE stagiaires (
    id              SERIAL PRIMARY KEY,
    matricule       VARCHAR(30) UNIQUE NOT NULL,
    nom             VARCHAR(100) NOT NULL,
    prenom          VARCHAR(100) NOT NULL,
    date_naissance  DATE,
    sexe            CHAR(1),
    telephone       VARCHAR(20),
    email           VARCHAR(150),
    etablissement   VARCHAR(200),
    niveau_etude    VARCHAR(100),
    service_accueil VARCHAR(200),
    encadrant_id    INTEGER REFERENCES employes(id),
    date_debut      DATE NOT NULL,
    date_fin        DATE NOT NULL,
    statut          VARCHAR(30) DEFAULT 'en_cours',  -- 'en_cours' | 'termine' | 'abandonne'
    photo_path      VARCHAR(255),
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW()
);
```

### 4.7 Table `employes`
```sql
CREATE TABLE employes (
    id              SERIAL PRIMARY KEY,
    matricule       VARCHAR(30) UNIQUE NOT NULL,
    nom             VARCHAR(100) NOT NULL,
    prenom          VARCHAR(100) NOT NULL,
    poste           VARCHAR(150),
    departement     VARCHAR(150),
    telephone       VARCHAR(20),
    email           VARCHAR(150),
    date_embauche   DATE,
    salaire_base    NUMERIC(12,2) NOT NULL DEFAULT 0,
    photo_path      VARCHAR(255),
    actif           BOOLEAN DEFAULT TRUE,
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW()
);
```

### 4.8 Table `commandes`
```sql
CREATE TABLE commandes (
    id              SERIAL PRIMARY KEY,
    numero          VARCHAR(50) UNIQUE NOT NULL,
    client_nom      VARCHAR(200) NOT NULL,
    client_telephone VARCHAR(20),
    client_email    VARCHAR(150),
    type_service    VARCHAR(150) NOT NULL,
    description     TEXT,
    cout_total      NUMERIC(12,2) NOT NULL DEFAULT 0,
    statut          VARCHAR(30) DEFAULT 'en_attente',  -- 'en_attente' | 'en_cours' | 'terminee' | 'livree' | 'annulee'
    employe_id      INTEGER REFERENCES employes(id),
    date_commande   DATE DEFAULT CURRENT_DATE,
    date_livraison_prevue DATE,
    date_livraison_reelle DATE,
    notes           TEXT,
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW()
);
```

### 4.9 Table `paiements`
```sql
CREATE TABLE paiements (
    id              SERIAL PRIMARY KEY,
    reference       VARCHAR(80) UNIQUE NOT NULL,
    type_source     VARCHAR(30) NOT NULL,  -- 'formation' | 'commande'
    source_id       INTEGER NOT NULL,
    montant_total   NUMERIC(12,2) NOT NULL,
    montant_verse   NUMERIC(12,2) NOT NULL DEFAULT 0,
    montant_restant NUMERIC(12,2) GENERATED ALWAYS AS (montant_total - montant_verse) STORED,
    mode_paiement   VARCHAR(50),  -- 'especes' | 'mobile_money' | 'virement' | 'cheque'
    statut          VARCHAR(30) DEFAULT 'partiel',  -- 'partiel' | 'complet' | 'en_attente'
    date_paiement   DATE DEFAULT CURRENT_DATE,
    notes           TEXT,
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW()
);
```

### 4.10 Table `salaires`
```sql
CREATE TABLE salaires (
    id              SERIAL PRIMARY KEY,
    employe_id      INTEGER NOT NULL REFERENCES employes(id),
    periode_mois    INTEGER NOT NULL,  -- 1-12
    periode_annee   INTEGER NOT NULL,
    salaire_base    NUMERIC(12,2) NOT NULL,
    prime_total     NUMERIC(12,2) DEFAULT 0,
    avance_deduite  NUMERIC(12,2) DEFAULT 0,
    retenues        NUMERIC(12,2) DEFAULT 0,
    net_a_payer     NUMERIC(12,2) GENERATED ALWAYS AS (salaire_base + prime_total - avance_deduite - retenues) STORED,
    date_paiement   DATE,
    statut          VARCHAR(20) DEFAULT 'en_attente',  -- 'en_attente' | 'paye'
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(employe_id, periode_mois, periode_annee)
);
```

### 4.11 Table `avances_salaire`
```sql
CREATE TABLE avances_salaire (
    id              SERIAL PRIMARY KEY,
    employe_id      INTEGER NOT NULL REFERENCES employes(id),
    montant         NUMERIC(12,2) NOT NULL,
    date_avance     DATE DEFAULT CURRENT_DATE,
    remboursee      BOOLEAN DEFAULT FALSE,
    salaire_id      INTEGER REFERENCES salaires(id),
    notes           TEXT,
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW()
);
```

---

## 5. Modules fonctionnels

### 5.1 Gestion des Apprenants

#### Écrans requis

| Écran | Description |
|---|---|
| `ApprenantListView` | DataGrid paginée avec recherche full-text, filtre par formation/session, export Excel |
| `ApprenantFormView` | Formulaire création/édition avec upload photo |
| `ApprenantDetailView` | Fiche complète : infos, historique formations, état paiements |
| `InscriptionFormationView` | Wizard 3 étapes : choix apprenant → choix formation → choix session |

#### Fonctionnalités détaillées

```
[F-APP-01] Enregistrement d'un apprenant
  - Champs : Matricule (auto-généré : APP-YYYYMMDD-XXX), Nom, Prénom, Date naissance,
             Sexe, Téléphone, Email, Adresse, Photo
  - Validation : matricule unique, email valide si renseigné, nom/prénom obligatoires
  - La photo est redimensionnée en 200×200px et stockée localement

[F-APP-02] Inscription à une formation
  - Sélectionner un apprenant existant
  - Choisir une formation parmi la liste des formations actives
  - Sélectionner une session ouverte (avec vérification capacité max)
  - Vérifier qu'il n'est pas déjà inscrit à cette session
  - Créer l'enregistrement dans inscriptions_formation
  - Option : créer un paiement associé immédiatement

[F-APP-03] Association à une session de formation
  - Depuis la fiche apprenant : liste déroulante des sessions disponibles
  - Même règle de doublon que F-APP-02

[F-APP-04] Historique des cours suivis
  - Vue chronologique par apprenant de toutes ses inscriptions
  - Colonnes : Formation, Session, Date début/fin, Statut, Note finale
  - Filtrable par année, statut

[F-APP-05] Liste des apprenants par formation
  - Sélectionner une formation → liste toutes sessions → liste apprenants
  - Exportable en PDF et Excel
  - Affiche statut paiement de chaque apprenant

[F-APP-06] Génération attestation de formation
  - Disponible uniquement si statut = 'certifie'
  - PDF avec : logo, nom complet apprenant, intitulé formation, durée, date, signature
  - QR code de vérification

[F-APP-07] Suivi des paiements
  - Vue des paiements liés aux inscriptions de l'apprenant
  - Distinction paiement complet / partiel / en attente
  - Accès rapide à la génération de reçu
```

---

### 5.2 Gestion des Stagiaires

#### Écrans requis

| Écran | Description |
|---|---|
| `StagiaireListView` | DataGrid avec filtre statut, service, encadrant, période |
| `StagiaireFormView` | Formulaire création/édition, sélecteur encadrant (liste employés) |
| `StagiaireDetailView` | Fiche complète avec durée calculée automatiquement |

#### Fonctionnalités détaillées

```
[F-STG-01] Enregistrement d'un stagiaire
  - Champs : Matricule (auto : STG-YYYYMMDD-XXX), Nom, Prénom, Date naissance, Sexe,
             Téléphone, Email, Établissement, Niveau d'étude, Date début, Date fin,
             Service d'accueil, Encadrant (FK employe), Photo
  - Calcul automatique : Durée du stage = date_fin - date_debut (en jours et semaines)

[F-STG-02] Association service / encadrant
  - Service d'accueil : champ texte libre + suggestions (autocomplete)
  - Encadrant : liste déroulante filtrée sur les employés actifs

[F-STG-03] Suivi durée du stage
  - Indicateur visuel (barre de progression) : jours écoulés / durée totale
  - Statut automatique : si date_fin < aujourd'hui → passe à 'termine'

[F-STG-04] Historique des stagiaires
  - Vue globale triable par : établissement, service, encadrant, période, statut
  - Recherche par nom/prénom/établissement

[F-STG-05] Génération attestation de stage
  - PDF avec : logo, nom stagiaire, établissement, service, encadrant, durée,
               date début/fin, signature du responsable
```

---

### 5.3 Gestion des Formations

#### Écrans requis

| Écran | Description |
|---|---|
| `FormationListView` | Catalogue des formations avec indicateurs de sessions actives |
| `FormationFormView` | Formulaire avec éditeur de programme (multi-lignes) |
| `SessionFormationView` | Gestion des sessions d'une formation |
| `ProgressionFormationView` | Dashboard de suivi d'avancement |

#### Fonctionnalités détaillées

```
[F-FOR-01] Enregistrement d'une formation
  - Champs : Code (auto : FOR-XXX), Intitulé, Description, Durée (heures), Coût,
             Catégorie, Programme (éditeur riche multi-lignes par module/jour)
  - Le programme est stocké en texte structuré (format Markdown simplifié)

[F-FOR-02] Ouverture et fermeture de session
  - Depuis la fiche formation : bouton "Ouvrir une session"
  - Champs session : Code, Date début, Date fin, Lieu, Formateur, Capacité max
  - Fermeture : change statut → 'fermee', bloque toute nouvelle inscription
  - Clôture : change statut → 'terminee', déclenche génération automatique des attestations

[F-FOR-03] Inscription des participants
  - Depuis la session : bouton "Ajouter participant"
  - Recherche d'apprenants existants ou création rapide inline
  - Vérification capacité max avant inscription

[F-FOR-04] Suivi progression
  - Dashboard par session : total inscrits, en cours, certifiés, abandons
  - Graphique en barres ou donut (WPF LiveCharts ou dessin manuel SVG)
  - Taux de réussite calculé dynamiquement

[F-FOR-05] Liste des apprenants par formation
  - Vue tableau avec : Nom, Prénom, Téléphone, Statut, Note, Paiement
  - Export PDF (format liste officielle signée) et Excel

[F-FOR-06] Programme de la formation
  - Visualisation formatée du programme (par jour ou par module)
  - Imprimable en PDF
```

---

### 5.4 Gestion des Paiements

#### Écrans requis

| Écran | Description |
|---|---|
| `PaiementListView` | Vue globale tous paiements, filtrable par type, statut, période |
| `PaiementFormView` | Formulaire de saisie / mise à jour d'un paiement |
| `RecuPaiementView` | Aperçu avant impression du reçu |

#### Fonctionnalités détaillées

```
[F-PAI-01] Enregistrement d'un paiement
  - Champs : Référence (auto : PAY-YYYY-XXXXX), Type source (formation/commande),
             Source (recherche intelligente), Montant total, Montant versé,
             Mode de paiement, Date, Notes
  - Calcul automatique du montant restant
  - Le statut est calculé : versé = total → 'complet', sinon → 'partiel'

[F-PAI-02] Suivi paiements partiels / complets
  - Indicateur coloré : Vert (complet), Orange (partiel), Rouge (en attente)
  - Possibilité d'ajouter un versement complémentaire sur un paiement partiel
  - Historique des versements successifs sur un même dossier

[F-PAI-03] Génération de reçu
  - PDF avec : logo, référence, date, bénéficiaire, objet (formation/commande),
               montant versé, montant restant, mode de paiement, cachet
  - Mention "REÇU EN BONNE ET DUE FORME"
```

---

### 5.5 Gestion des Commandes

#### Écrans requis

| Écran | Description |
|---|---|
| `CommandeListView` | Kanban ou DataGrid, filtrée par statut, employé, période |
| `CommandeFormView` | Formulaire complet avec calcul de coût |
| `CommandeDetailView` | Fiche détail avec timeline de l'état |

#### Fonctionnalités détaillées

```
[F-CMD-01] Enregistrement d'une commande
  - Champs : Numéro (auto : CMD-YYYYMMDD-XXX), Nom client, Téléphone, Email,
             Type de service (liste configurable), Description, Coût total,
             Date commande, Date livraison prévue, Notes
  - Statut initial : 'en_attente'

[F-CMD-02] Identification du type de service
  - Liste déroulante de types de services paramétrables (table de référence)
  - Possibilité de saisir un type libre

[F-CMD-03] Association client
  - Champ client textuel (pas de table client obligatoire en v1)
  - Téléphone et email optionnels pour recontact

[F-CMD-04] Suivi de l'état
  - Timeline visuelle : En attente → En cours → Terminée → Livrée
  - Changement de statut avec confirmation + horodatage automatique
  - Notification visuelle à chaque changement

[F-CMD-05] Affectation à un employé
  - Liste déroulante des employés actifs
  - Un employé peut avoir plusieurs commandes simultanées
  - Vue filtrée "Mes commandes" par employé

[F-CMD-06] Calcul du coût
  - Saisie manuelle du coût total
  - Extension possible : lignes de détail (article/service, qté, PU) avec total calculé

[F-CMD-07] Génération de reçu de commande
  - PDF avec : numéro, client, type service, description, coût, statut, date, employé
```

---

### 5.6 Gestion des Salaires

#### Écrans requis

| Écran | Description |
|---|---|
| `SalaireListView` | Vue par période (mois/année), liste tous employés avec statut |
| `SalaireFormView` | Formulaire de paie mensuelle par employé |
| `AvanceSalaireView` | Gestion des avances |
| `FichePayeView` | Aperçu et impression de la fiche de paie |
| `BadgeEmployeView` | Génération et impression du badge |

#### Fonctionnalités détaillées

```
[F-SAL-01] Enregistrement des salaires
  - Sélectionner un employé + une période (mois/année)
  - Préremplissage du salaire de base depuis la fiche employé
  - Unicité garantie : un seul enregistrement par employé/période

[F-SAL-02] Définition du salaire de base
  - Modifiable directement dans la fiche employé
  - Historique des modifications conservé (table salaires)

[F-SAL-03] Primes et bonus
  - Table de détail : Nature de la prime, Montant
  - Total primes calculé automatiquement et intégré dans le net à payer

[F-SAL-04] Avances sur salaire
  - Enregistrement d'une avance : Employé, Montant, Date, Notes
  - Déduction automatique dans la paie du mois sélectionné
  - Indicateur visuel "Avance en cours" sur la fiche employé

[F-SAL-05] Génération fiche de paie
  - PDF avec : en-tête société, employé (matricule, poste, département),
               période, salaire brut, détail primes, avances déduites,
               retenues diverses, NET À PAYER encadré, date, signatures

[F-SAL-06] Historique des paiements par employé
  - Vue chronologique de tous les mois payés / en attente
  - Accès direct à chaque fiche de paie

[F-SAL-07] Badge employé
  - PDF/Image imprimable au format carte (85×54mm)
  - Contenu : Photo (rognée cercle), Nom complet, Poste, Matricule,
               Logo société, QR code (données employé en JSON)
  - Fond de couleur selon le département (configurable)
```

---

## 6. Génération de documents (PDF)

Tous les documents sont générés avec **QuestPDF**. Chaque document est une classe héritant de `IDocument`.

### Palette de documents

| Document | Classe | Déclencheur |
|---|---|---|
| Attestation de formation | `AttestationFormationDocument` | Statut inscription = 'certifie' |
| Attestation de stage | `AttestationStageDocument` | Statut stagiaire = 'termine' |
| Reçu de paiement | `RecuPaiementDocument` | Tout enregistrement de paiement |
| Reçu de commande | `RecuCommandeDocument` | Toute commande |
| Fiche de paie | `FicheDePayeDocument` | Validation salaire mensuel |
| Badge employé | `BadgeEmployeDocument` | Demande manuelle depuis fiche employé |
| Liste apprenants | `ListeApprenantsPdfDocument` | Export depuis vue formation |

### Modèle de structure QuestPDF (exemple)

```csharp
public class RecuPaiementDocument : IDocument
{
    private readonly RecuPaiementDto _data;
    private readonly AppSettings _settings;

    public RecuPaiementDocument(RecuPaiementDto data, AppSettings settings)
    {
        _data = data;
        _settings = settings;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A5);
            page.Margin(20, Unit.Millimetre);
            page.DefaultTextStyle(x => x.FontFamily("Calibri").FontSize(10));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    private void ComposeHeader(IContainer container) { /* Logo + titre */ }
    private void ComposeContent(IContainer container) { /* Corps du reçu */ }
    private void ComposeFooter(IContainer container) { /* Bas de page */ }
}
```

---

## 7. UI/UX — Design System

### 7.1 Thème général

```xml
<!-- App.xaml — ResourceDictionary -->
<materialDesign:BundledTheme BaseTheme="Dark"
                              PrimaryColor="Blue"
                              SecondaryColor="Cyan" />

<!-- Couleurs personnalisées -->
<Color x:Key="PrimaryColor">#1565C0</Color>
<Color x:Key="AccentColor">#00ACC1</Color>
<Color x:Key="SurfaceDark">#1E2130</Color>
<Color x:Key="SurfaceLight">#F5F7FA</Color>
<Color x:Key="SuccessColor">#2E7D32</Color>
<Color x:Key="WarningColor">#F57F17</Color>
<Color x:Key="DangerColor">#C62828</Color>
```

### 7.2 Layout principal (MainWindow)

```
┌─────────────────────────────────────────────────────────┐
│  [LOGO]  BEHGest Pro           [🔍] [🔔] [👤] [🌙/☀️]  │  ← TopBar (60px)
├────────────┬────────────────────────────────────────────┤
│            │                                             │
│  📋 Tableau │                                            │
│     de bord│         Zone de contenu principale         │
│            │         (Frame/ContentControl navigable)   │
│  👨‍🎓 Appren. │                                            │
│  🎓 Stagiai │                                            │
│  📚 Format. │                                            │
│  💳 Paiemts │                                            │
│  📦 Commd.  │                                            │
│  💰 Salaires│                                            │
│            │                                             │
│  ─────────  │                                            │
│  ⚙️ Paramètres                                          │
│  ❓ Aide    │                                            │
└────────────┴────────────────────────────────────────────┘
  SideNav (220px)     ContentArea (fill)
```

### 7.3 Composants UI obligatoires

| Composant | Spécification |
|---|---|
| `SideNav` | NavigationDrawer Material, items avec icône + label, item actif surligné |
| `DataGrid` | Style épuré, alternance de lignes, hover coloré, pagination intégrée |
| `SearchBox` | Barre de recherche avec icône loupe, debounce 300ms |
| `StatusBadge` | Pilule colorée (Chip MaterialDesign) selon statut |
| `ActionBar` | Barre en haut de chaque vue : titre + boutons d'action alignés à droite |
| `ConfirmDialog` | Dialog modal avec titre, message, bouton Confirmer/Annuler |
| `LoadingOverlay` | Spinner centré sur la vue pendant les opérations async |
| `SnackBar` | Notifications en bas d'écran (succès vert, erreur rouge, info bleu) |
| `FormCard` | Card Material avec ombre légère pour regrouper les champs de formulaire |
| `StatCard` | Carte de statistique : icône + valeur + libellé (dashboard) |

### 7.4 Dashboard (Tableau de bord)

La vue d'accueil affiche :
- 6 `StatCard` : Total apprenants, Formations actives, Commandes en cours, Paiements du mois, Stagiaires actuels, Salaires en attente
- Graphique mensuel des inscriptions (12 mois glissants)
- Tableau des dernières commandes (top 5)
- Indicateur des paiements partiels en attente

### 7.5 Règles UX

- Tous les formulaires valident en temps réel (rouge + message sous le champ invalide)
- Les listes de plus de 50 items sont paginées (25 par page par défaut)
- Tout chargement de données > 200ms affiche un spinner
- Toute suppression demande une confirmation
- Toute action irréversible est précédée d'un dialog explicite
- Les champs de date utilisent un DatePicker Material
- Les montants s'affichent toujours avec le séparateur de milliers et la devise (ex : `150 000 XAF`)
- Raccourcis clavier : `Ctrl+N` → Nouveau, `Ctrl+S` → Sauvegarder, `Escape` → Fermer dialog

---

## 8. Sécurité & Authentification

```
[F-SEC-01] Écran de connexion
  - Champs : Email + Mot de passe
  - Hachage : BCrypt (cost factor 12)
  - 3 tentatives échouées → verrouillage 5 minutes
  - Option "Se souvenir de moi" (token chiffré en local)

[F-SEC-02] Rôles
  - admin    : accès complet, gestion utilisateurs, paramètres
  - operateur : accès CRUD à tous les modules, pas de suppression définitive
  - lecteur  : consultation uniquement, export autorisé

[F-SEC-03] Journalisation
  - Table audit_logs : utilisateur_id, action, entite, entite_id, timestamp, ip
  - Toute création, modification, suppression est journalisée
```

---

## 9. Structure des projets C#

### `BehGestionDbContext.cs` (Infrastructure)

```csharp
public class BehGestionDbContext : DbContext
{
    public BehGestionDbContext(DbContextOptions<BehGestionDbContext> options) : base(options) { }

    public DbSet<Apprenant> Apprenants => Set<Apprenant>();
    public DbSet<Stagiaire> Stagiaires => Set<Stagiaire>();
    public DbSet<Formation> Formations => Set<Formation>();
    public DbSet<SessionFormation> SessionsFormation => Set<SessionFormation>();
    public DbSet<InscriptionFormation> InscriptionsFormation => Set<InscriptionFormation>();
    public DbSet<Employe> Employes => Set<Employe>();
    public DbSet<Commande> Commandes => Set<Commande>();
    public DbSet<Paiement> Paiements => Set<Paiement>();
    public DbSet<Salaire> Salaires => Set<Salaire>();
    public DbSet<AvanceSalaire> AvancesSalaire => Set<AvanceSalaire>();
    public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BehGestionDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
```

### Interface Repository générique (Domain)

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

### Service métier (pattern)

```csharp
public class ApprenantService
{
    private readonly IRepository<Apprenant> _repo;
    private readonly IRepository<InscriptionFormation> _inscriptionRepo;
    private readonly BehGestionDbContext _context;

    public ApprenantService(
        IRepository<Apprenant> repo,
        IRepository<InscriptionFormation> inscriptionRepo,
        BehGestionDbContext context)
    {
        _repo = repo;
        _inscriptionRepo = inscriptionRepo;
        _context = context;
    }

    public async Task<string> GenererMatriculeAsync()
    {
        var today = DateTime.Today.ToString("yyyyMMdd");
        var count = await _context.Apprenants.CountAsync(a => a.Matricule.StartsWith($"APP-{today}"));
        return $"APP-{today}-{(count + 1):D3}";
    }
}
```

### ViewModel (pattern CommunityToolkit.Mvvm)

```csharp
[ObservableObject]
public partial class ApprenantListViewModel
{
    private readonly ApprenantService _service;

    [ObservableProperty]
    private ObservableCollection<Apprenant> _apprenants = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public ApprenantListViewModel(ApprenantService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        var data = await _service.GetAllAsync();
        Apprenants = new ObservableCollection<Apprenant>(data);
        IsLoading = false;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsLoading = true;
        var data = await _service.SearchAsync(SearchText);
        Apprenants = new ObservableCollection<Apprenant>(data);
        IsLoading = false;
    }
}
```

---

## 10. Conventions de code

| Règle | Détail |
|---|---|
| Langue du code | Anglais (noms de variables, méthodes, classes) |
| Langue des données | Français (libellés, messages d'erreur, contenu UI) |
| Commentaires | Uniquement sur les zones complexes (algorithmes non triviaux, règles métier critiques) — **aucun commentaire évident** |
| Async/Await | Toutes les opérations DB sont async |
| Null safety | Nullable reference types activés (`<Nullable>enable</Nullable>`) |
| Nommage | PascalCase classes/props, camelCase vars locales, `_camelCase` pour les champs privés |
| Validation | FluentValidation ou validation inline dans les services |
| Gestion des erreurs | Try/catch dans les ViewModels, affichage SnackBar, log en base |

---

## 11. Règles de génération pour l'IA

> Ces règles s'appliquent à toute IA qui génère le code à partir de ce document.

1. **Ne jamais générer de commentaires évidents**. Commenter uniquement : algorithmes de calcul complexes, règles métier non triviales, contournements techniques justifiés.

2. **Respecter strictement la séparation des couches** : Domain → Infrastructure → Application → UI. Aucun accès EF Core direct dans un ViewModel.

3. **Injection de dépendances via Microsoft.Extensions.DependencyInjection** dans `App.xaml.cs`. Aucun `new Service()` dans les ViewModels.

4. **Chaque vue WPF** doit avoir un ViewModel associé, injecté via DI. Le code-behind ne contient que des initialisations d'UI (aucune logique métier).

5. **QuestPDF pour tous les PDF**. Jamais de WebBrowser control ou de HTML/CSS pour les documents.

6. **La génération de matricules/numéros** est toujours effectuée côté service, jamais dans le ViewModel ni dans la vue.

7. **Les DatePicker** utilisent le style MaterialDesign et sont liés à des propriétés `DateTime?` dans les ViewModels.

8. **Les DataGrids** sont liées à `ObservableCollection<T>` avec tri et pagination implémentés.

9. **Les dialogs de confirmation** utilisent `MaterialDesignThemes.Wpf.DialogHost`.

10. **Le dashboard** est la vue par défaut après connexion. Il recalcule ses indicateurs à chaque navigation vers lui.

11. **Les suppressions** sont logiques (soft delete) : un champ `IsDeleted` ou statut 'archive' — jamais de `DELETE SQL` définitif sur les entités métier.

12. **Le thème Dark/Light** est persisté en `AppSettings` local et rechargé au démarrage.

13. **Toutes les vues de liste** incluent une barre de recherche debounce 300ms et un bouton "Réinitialiser les filtres".

14. **Les exports PDF** ouvrent automatiquement le fichier avec le lecteur PDF par défaut après génération.

15. **Les formats monétaires** utilisent `ToString("N0") + " XAF"` sur tous les affichages de montants.

---

*Document généré pour BEHGest Pro v1.0.0 — Spécifications complètes à soumettre à un générateur de code IA.*
*Toute modification de ce document doit être répercutée dans les migrations EF Core correspondantes.*
