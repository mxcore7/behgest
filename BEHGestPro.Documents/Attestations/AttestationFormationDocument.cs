using BEHGestPro.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BEHGestPro.Documents.Attestations;

public class AttestationFormationDocument : IDocument
{
    private readonly Apprenant _apprenant;
    private readonly InscriptionFormation _inscription;
    private readonly string _nomSociete;

    public AttestationFormationDocument(Apprenant apprenant, InscriptionFormation inscription, string nomSociete)
    {
        _apprenant = apprenant;
        _inscription = inscription;
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
            col.Item().PaddingTop(8).Text("ATTESTATION DE FORMATION")
                .Bold().FontSize(20).FontColor("#1E2130").AlignCenter();
            col.Item().PaddingTop(5).LineHorizontal(2).LineColor("#00ACC1");
        });
    }

    private void ComposeContent(IContainer container)
    {
        var formation = _inscription.Session?.Formation;
        var session = _inscription.Session;

        container.PaddingTop(20).Column(col =>
        {
            col.Item().Text("Nous soussignés,").FontSize(12);
            col.Item().PaddingTop(5).Text($"attestons que :").FontSize(12);
            col.Item().PaddingTop(15).Text(
                $"{_apprenant.Prenom.ToUpper()} {_apprenant.Nom.ToUpper()}")
                .Bold().FontSize(16).AlignCenter();

            col.Item().PaddingTop(10).Text(
                $"a suivi et validé avec succès la formation intitulée :")
                .FontSize(12).AlignCenter();

            col.Item().PaddingTop(10)
                .Background("#1565C0")
                .Padding(10)
                .Text(formation?.Intitule ?? "—")
                .Bold().FontSize(14).FontColor(Colors.White).AlignCenter();

            col.Item().PaddingTop(15).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn();
                    c.RelativeColumn();
                });

                void Row(string l, string v)
                {
                    table.Cell().Padding(5).Text(l).SemiBold().FontSize(11);
                    table.Cell().Padding(5).Text(v).FontSize(11);
                }

                Row("Durée :", $"{formation?.DureeHeures ?? 0} heures");
                Row("Période :", $"du {session?.DateDebut:dd/MM/yyyy} au {session?.DateFin:dd/MM/yyyy}");
                Row("Lieu :", session?.Lieu ?? "—");
                Row("Formateur :", session?.Formateur ?? "—");
                if (_inscription.NoteFinale.HasValue)
                    Row("Note obtenue :", $"{_inscription.NoteFinale:F2} / 20");
            });

            col.Item().PaddingTop(30).Text(
                $"Fait à ________, le {DateTime.Today:dd MMMM yyyy}")
                .FontSize(11).AlignRight();

            col.Item().PaddingTop(40).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Le Responsable").AlignCenter().SemiBold();
                    c.Item().PaddingTop(40).Text("_______________________").AlignCenter();
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Cachet et Signature").AlignCenter().SemiBold();
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
