using BEHGestPro.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BEHGestPro.Documents.Attestations;

public class AttestationStageDocument : IDocument
{
    private readonly Stagiaire _stagiaire;
    private readonly string _nomSociete;

    public AttestationStageDocument(Stagiaire stagiaire, string nomSociete)
    {
        _stagiaire = stagiaire;
        _nomSociete = nomSociete;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(30, Unit.Millimetre);
            page.DefaultTextStyle(x => x.FontFamily("Calibri").FontSize(11));
            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Text(_nomSociete).Bold().FontSize(18).FontColor("#1565C0").AlignCenter();
            col.Item().PaddingTop(5).LineHorizontal(2).LineColor("#00ACC1");
            col.Item().PaddingTop(8).Text("ATTESTATION DE STAGE")
                .Bold().FontSize(20).FontColor("#1E2130").AlignCenter();
            col.Item().PaddingTop(5).LineHorizontal(2).LineColor("#00ACC1");
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingTop(20).Column(col =>
        {
            col.Item().Text("Nous soussignés, certifions que :").FontSize(12);

            col.Item().PaddingTop(15).Text(
                $"{_stagiaire.Prenom.ToUpper()} {_stagiaire.Nom.ToUpper()}")
                .Bold().FontSize(16).AlignCenter();

            col.Item().PaddingTop(5).Text(
                $"Étudiant(e) en {_stagiaire.NiveauEtude ?? "—"} à {_stagiaire.Etablissement ?? "—"}")
                .FontSize(12).AlignCenter().Italic();

            col.Item().PaddingTop(15).Text("a effectué un stage au sein de notre structure selon les modalités suivantes :")
                .FontSize(12);

            col.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn();
                    c.RelativeColumn();
                });

                void Row(string l, string v)
                {
                    table.Cell().Padding(5).Text(l).SemiBold();
                    table.Cell().Padding(5).Text(v);
                }

                Row("Service d'accueil :", _stagiaire.ServiceAccueil ?? "—");
                Row("Encadrant :", _stagiaire.Encadrant?.NomComplet ?? "—");
                Row("Date de début :", _stagiaire.DateDebut.ToString("dd/MM/yyyy"));
                Row("Date de fin :", _stagiaire.DateFin.ToString("dd/MM/yyyy"));
                Row("Durée du stage :", $"{_stagiaire.DureeJours} jours ({_stagiaire.DureeSemaines} semaines)");
            });

            col.Item().PaddingTop(20).Text(
                "Cette attestation est délivrée à l'intéressé(e) pour servir et valoir ce que de droit.")
                .FontSize(11).Italic();

            col.Item().PaddingTop(20).Text(
                $"Fait à ________, le {DateTime.Today:dd MMMM yyyy}").FontSize(11).AlignRight();

            col.Item().PaddingTop(40).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("L'Encadrant").AlignCenter().SemiBold();
                    c.Item().PaddingTop(40).Text("_______________________").AlignCenter();
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Le Responsable RH").AlignCenter().SemiBold();
                    c.Item().PaddingTop(40).Text("_______________________").AlignCenter();
                });
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
            col.Item().PaddingTop(3).Text($"BEHGest Pro — Document officiel — {DateTime.Now:dd/MM/yyyy}")
                .FontSize(8).FontColor(Colors.Grey.Darken1).AlignCenter();
        });
    }
}
