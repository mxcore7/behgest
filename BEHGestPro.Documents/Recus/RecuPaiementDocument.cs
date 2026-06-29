using BEHGestPro.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BEHGestPro.Documents.Recus;

public class RecuPaiementDocument : IDocument
{
    private readonly Paiement _paiement;
    private readonly string _beneficiaire;
    private readonly string _objetPaiement;
    private readonly string _nomSociete;

    public RecuPaiementDocument(Paiement paiement, string beneficiaire, string objetPaiement, string nomSociete)
    {
        _paiement = paiement;
        _beneficiaire = beneficiaire;
        _objetPaiement = objetPaiement;
        _nomSociete = nomSociete;
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

    private void ComposeHeader(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(_nomSociete).Bold().FontSize(14).FontColor("#1565C0");
                    c.Item().Text("REÇU DE PAIEMENT").SemiBold().FontSize(12);
                });
                row.ConstantItem(80).AlignRight().Column(c =>
                {
                    c.Item().Text($"Réf: {_paiement.Reference}").FontSize(9).FontColor(Colors.Grey.Darken2);
                    c.Item().Text(_paiement.DatePaiement.ToString("dd/MM/yyyy")).FontSize(9);
                });
            });
            col.Item().PaddingTop(5).LineHorizontal(1).LineColor("#1565C0");
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingTop(10).Column(col =>
        {
            col.Item().Text("REÇU EN BONNE ET DUE FORME").Bold().FontSize(11).AlignCenter();
            col.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(2);
                    c.RelativeColumn(3);
                });

                void Row(string label, string value)
                {
                    table.Cell().Padding(4).Text(label).SemiBold();
                    table.Cell().Padding(4).Text(value);
                }

                Row("Bénéficiaire :", _beneficiaire);
                Row("Objet :", _objetPaiement);
                Row("Montant total :", $"{_paiement.MontantTotal:N0} XAF");
                Row("Montant versé :", $"{_paiement.MontantVerse:N0} XAF");
                Row("Montant restant :", $"{_paiement.MontantRestant:N0} XAF");
                Row("Mode de paiement :", _paiement.ModePaiement ?? "—");
                Row("Statut :", _paiement.Statut == "complet" ? "PAYÉ INTÉGRALEMENT" : "PAIEMENT PARTIEL");
            });

            if (!string.IsNullOrWhiteSpace(_paiement.Notes))
            {
                col.Item().PaddingTop(8).Text($"Notes : {_paiement.Notes}").FontSize(9).Italic();
            }

            col.Item().PaddingTop(20).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Le Caissier").AlignCenter().SemiBold();
                    c.Item().PaddingTop(30).Text("_______________________").AlignCenter();
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Le Bénéficiaire").AlignCenter().SemiBold();
                    c.Item().PaddingTop(30).Text("_______________________").AlignCenter();
                });
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
            col.Item().PaddingTop(3).Text($"Document généré le {DateTime.Now:dd/MM/yyyy HH:mm} — BEHGest Pro")
                .FontSize(8).FontColor(Colors.Grey.Darken1).AlignCenter();
        });
    }
}
