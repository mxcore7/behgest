using BEHGestPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Infrastructure.Data;

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
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Apprenant>(e =>
        {
            e.ToTable("apprenants");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Matricule).HasColumnName("matricule").IsRequired().HasMaxLength(30);
            e.Property(x => x.Nom).HasColumnName("nom").IsRequired().HasMaxLength(100);
            e.Property(x => x.Prenom).HasColumnName("prenom").IsRequired().HasMaxLength(100);
            e.Property(x => x.DateNaissance).HasColumnName("date_naissance");
            e.Property(x => x.Sexe).HasColumnName("sexe");
            e.Property(x => x.Telephone).HasColumnName("telephone").HasMaxLength(20);
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(150);
            e.Property(x => x.Adresse).HasColumnName("adresse");
            e.Property(x => x.PhotoPath).HasColumnName("photo_path").HasMaxLength(255);
            e.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Ignore(x => x.NomComplet);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Stagiaire>(e =>
        {
            e.ToTable("stagiaires");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Matricule).HasColumnName("matricule").IsRequired().HasMaxLength(30);
            e.Property(x => x.Nom).HasColumnName("nom").IsRequired().HasMaxLength(100);
            e.Property(x => x.Prenom).HasColumnName("prenom").IsRequired().HasMaxLength(100);
            e.Property(x => x.DateNaissance).HasColumnName("date_naissance");
            e.Property(x => x.Sexe).HasColumnName("sexe");
            e.Property(x => x.Telephone).HasColumnName("telephone").HasMaxLength(20);
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(150);
            e.Property(x => x.Etablissement).HasColumnName("etablissement").HasMaxLength(200);
            e.Property(x => x.NiveauEtude).HasColumnName("niveau_etude").HasMaxLength(100);
            e.Property(x => x.ServiceAccueil).HasColumnName("service_accueil").HasMaxLength(200);
            e.Property(x => x.EncadrantId).HasColumnName("encadrant_id");
            e.Property(x => x.DateDebut).HasColumnName("date_debut");
            e.Property(x => x.DateFin).HasColumnName("date_fin");
            e.Property(x => x.Statut).HasColumnName("statut").HasMaxLength(30);
            e.Property(x => x.PhotoPath).HasColumnName("photo_path").HasMaxLength(255);
            e.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Ignore(x => x.NomComplet);
            e.Ignore(x => x.DureeJours);
            e.Ignore(x => x.DureeSemaines);
            e.Ignore(x => x.ProgressionPct);
            e.HasOne(x => x.Encadrant).WithMany(emp => emp.Stagiaires).HasForeignKey(x => x.EncadrantId).OnDelete(DeleteBehavior.SetNull);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Formation>(e =>
        {
            e.ToTable("formations");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Code).HasColumnName("code").IsRequired().HasMaxLength(30);
            e.Property(x => x.Intitule).HasColumnName("intitule").IsRequired().HasMaxLength(200);
            e.Property(x => x.Description).HasColumnName("description");
            e.Property(x => x.DureeHeures).HasColumnName("duree_heures");
            e.Property(x => x.Cout).HasColumnName("cout").HasPrecision(12, 2);
            e.Property(x => x.Categorie).HasColumnName("categorie").HasMaxLength(100);
            e.Property(x => x.Programme).HasColumnName("programme");
            e.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<SessionFormation>(e =>
        {
            e.ToTable("sessions_formation");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.FormationId).HasColumnName("formation_id");
            e.Property(x => x.CodeSession).HasColumnName("code_session").IsRequired().HasMaxLength(50);
            e.Property(x => x.DateDebut).HasColumnName("date_debut");
            e.Property(x => x.DateFin).HasColumnName("date_fin");
            e.Property(x => x.Lieu).HasColumnName("lieu").HasMaxLength(200);
            e.Property(x => x.Formateur).HasColumnName("formateur").HasMaxLength(200);
            e.Property(x => x.CapaciteMax).HasColumnName("capacite_max").HasDefaultValue(30);
            e.Property(x => x.Statut).HasColumnName("statut").HasMaxLength(30);
            e.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Ignore(x => x.NbInscrits);
            e.Ignore(x => x.EstComplete);
            e.HasOne(x => x.Formation).WithMany(f => f.Sessions).HasForeignKey(x => x.FormationId);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<InscriptionFormation>(e =>
        {
            e.ToTable("inscriptions_formation");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.ApprenantId).HasColumnName("apprenant_id");
            e.Property(x => x.SessionId).HasColumnName("session_id");
            e.Property(x => x.DateInscription).HasColumnName("date_inscription");
            e.Property(x => x.Statut).HasColumnName("statut").HasMaxLength(30);
            e.Property(x => x.NoteFinale).HasColumnName("note_finale").HasPrecision(5, 2);
            e.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(x => x.Apprenant).WithMany(a => a.Inscriptions).HasForeignKey(x => x.ApprenantId);
            e.HasOne(x => x.Session).WithMany(s => s.Inscriptions).HasForeignKey(x => x.SessionId);
            e.HasIndex(x => new { x.ApprenantId, x.SessionId }).IsUnique();
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Employe>(e =>
        {
            e.ToTable("employes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Matricule).HasColumnName("matricule").IsRequired().HasMaxLength(30);
            e.Property(x => x.Nom).HasColumnName("nom").IsRequired().HasMaxLength(100);
            e.Property(x => x.Prenom).HasColumnName("prenom").IsRequired().HasMaxLength(100);
            e.Property(x => x.Poste).HasColumnName("poste").HasMaxLength(150);
            e.Property(x => x.Departement).HasColumnName("departement").HasMaxLength(150);
            e.Property(x => x.Telephone).HasColumnName("telephone").HasMaxLength(20);
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(150);
            e.Property(x => x.DateEmbauche).HasColumnName("date_embauche");
            e.Property(x => x.SalaireBase).HasColumnName("salaire_base").HasPrecision(12, 2);
            e.Property(x => x.PhotoPath).HasColumnName("photo_path").HasMaxLength(255);
            e.Property(x => x.Actif).HasColumnName("actif").HasDefaultValue(true);
            e.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Ignore(x => x.NomComplet);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Commande>(e =>
        {
            e.ToTable("commandes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Numero).HasColumnName("numero").IsRequired().HasMaxLength(50);
            e.Property(x => x.ClientNom).HasColumnName("client_nom").IsRequired().HasMaxLength(200);
            e.Property(x => x.ClientTelephone).HasColumnName("client_telephone").HasMaxLength(20);
            e.Property(x => x.ClientEmail).HasColumnName("client_email").HasMaxLength(150);
            e.Property(x => x.TypeService).HasColumnName("type_service").IsRequired().HasMaxLength(150);
            e.Property(x => x.Description).HasColumnName("description");
            e.Property(x => x.CoutTotal).HasColumnName("cout_total").HasPrecision(12, 2);
            e.Property(x => x.Statut).HasColumnName("statut").HasMaxLength(30);
            e.Property(x => x.EmployeId).HasColumnName("employe_id");
            e.Property(x => x.DateCommande).HasColumnName("date_commande");
            e.Property(x => x.DateLivraisonPrevue).HasColumnName("date_livraison_prevue");
            e.Property(x => x.DateLivraisonReelle).HasColumnName("date_livraison_reelle");
            e.Property(x => x.Notes).HasColumnName("notes");
            e.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(x => x.Employe).WithMany(emp => emp.Commandes).HasForeignKey(x => x.EmployeId).OnDelete(DeleteBehavior.SetNull);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Paiement>(e =>
        {
            e.ToTable("paiements");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Reference).HasColumnName("reference").IsRequired().HasMaxLength(80);
            e.Property(x => x.TypeSource).HasColumnName("type_source").IsRequired().HasMaxLength(30);
            e.Property(x => x.SourceId).HasColumnName("source_id");
            e.Property(x => x.MontantTotal).HasColumnName("montant_total").HasPrecision(12, 2);
            e.Property(x => x.MontantVerse).HasColumnName("montant_verse").HasPrecision(12, 2);
            e.Property(x => x.ModePaiement).HasColumnName("mode_paiement").HasMaxLength(50);
            e.Property(x => x.Statut).HasColumnName("statut").HasMaxLength(30);
            e.Property(x => x.DatePaiement).HasColumnName("date_paiement");
            e.Property(x => x.Notes).HasColumnName("notes");
            e.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Ignore(x => x.MontantRestant);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Salaire>(e =>
        {
            e.ToTable("salaires");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.EmployeId).HasColumnName("employe_id");
            e.Property(x => x.PeriodeMois).HasColumnName("periode_mois");
            e.Property(x => x.PeriodeAnnee).HasColumnName("periode_annee");
            e.Property(x => x.SalaireBase).HasColumnName("salaire_base").HasPrecision(12, 2);
            e.Property(x => x.PrimeTotal).HasColumnName("prime_total").HasPrecision(12, 2);
            e.Property(x => x.AvanceDeduite).HasColumnName("avance_deduite").HasPrecision(12, 2);
            e.Property(x => x.Retenues).HasColumnName("retenues").HasPrecision(12, 2);
            e.Property(x => x.DatePaiement).HasColumnName("date_paiement");
            e.Property(x => x.Statut).HasColumnName("statut").HasMaxLength(20);
            e.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Ignore(x => x.NetAPayer);
            e.Ignore(x => x.PeriodeLibelle);
            e.HasOne(x => x.Employe).WithMany(emp => emp.Salaires).HasForeignKey(x => x.EmployeId);
            e.HasIndex(x => new { x.EmployeId, x.PeriodeMois, x.PeriodeAnnee }).IsUnique();
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<AvanceSalaire>(e =>
        {
            e.ToTable("avances_salaire");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.EmployeId).HasColumnName("employe_id");
            e.Property(x => x.Montant).HasColumnName("montant").HasPrecision(12, 2);
            e.Property(x => x.DateAvance).HasColumnName("date_avance");
            e.Property(x => x.Remboursee).HasColumnName("remboursee").HasDefaultValue(false);
            e.Property(x => x.SalaireId).HasColumnName("salaire_id");
            e.Property(x => x.Notes).HasColumnName("notes");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(x => x.Employe).WithMany(emp => emp.Avances).HasForeignKey(x => x.EmployeId);
            e.HasOne(x => x.Salaire).WithMany().HasForeignKey(x => x.SalaireId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Utilisateur>(e =>
        {
            e.ToTable("utilisateurs");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Nom).HasColumnName("nom").IsRequired().HasMaxLength(100);
            e.Property(x => x.Prenom).HasColumnName("prenom").IsRequired().HasMaxLength(100);
            e.Property(x => x.Email).HasColumnName("email").IsRequired().HasMaxLength(150);
            e.Property(x => x.MotDePasseHash).HasColumnName("mot_de_passe_hash").IsRequired().HasMaxLength(255);
            e.Property(x => x.Role).HasColumnName("role").IsRequired().HasMaxLength(50).HasDefaultValue("operateur");
            e.Property(x => x.Actif).HasColumnName("actif").HasDefaultValue(true);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Ignore(x => x.NomComplet);
            e.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<AuditLog>(e =>
        {
            e.ToTable("audit_logs");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UtilisateurId).HasColumnName("utilisateur_id");
            e.Property(x => x.Action).HasColumnName("action").IsRequired().HasMaxLength(100);
            e.Property(x => x.Entite).HasColumnName("entite").IsRequired().HasMaxLength(100);
            e.Property(x => x.EntiteId).HasColumnName("entite_id");
            e.Property(x => x.Timestamp).HasColumnName("timestamp");
            e.Property(x => x.Ip).HasColumnName("ip").HasMaxLength(45);
        });

        base.OnModelCreating(modelBuilder);
    }
}
