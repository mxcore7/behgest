using BEHGestPro.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BEHGestPro.Documents.Fiches;

public class FicheDePayeDocument : IDocument
{
    private readonly Salaire _salaire;
    private readonly string _nomSociete;
    private readonly string _adresseSociete;

    public FicheDePayeDocument(Salaire salaire, string nomSociete, string adresseSociete)
    {
        _salaire = salaire;
        _nomSociete = nomSociete;
        _adresseSociete = adresseSociete;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
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
                    c.Item().Text(_adresseSociete).FontSize(9).FontColor(Colors.Grey.Darken2);
                });
                row.ConstantItem(120).AlignRight().Column(c =>
                {
                    c.Item().Text("BULLETIN DE PAIE").Bold().FontSize(12);
                    c.Item().Text($"Période : {_salaire.PeriodeLibelle}").FontSize(10);
                });
            });
            col.Item().PaddingTop(5).LineHorizontal(2).LineColor("#1565C0");
        });
    }

    private void ComposeContent(IContainer container)
    {
        var emp = _salaire.Employe;
        container.PaddingTop(10).Column(col =>
        {
            col.Item().Background(Colors.Grey.Lighten4).Padding(8).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("EMPLOYÉ").SemiBold().FontSize(9).FontColor(Colors.Grey.Darken2);
                    c.Item().Text(emp?.NomComplet ?? "—").Bold().FontSize(12);
                    c.Item().Text($"Matricule : {emp?.Matricule}").FontSize(9);
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("POSTE / DÉPARTEMENT").SemiBold().FontSize(9).FontColor(Colors.Grey.Darken2);
                    c.Item().Text(emp?.Poste ?? "—").FontSize(11);
                    c.Item().Text(emp?.Departement ?? "—").FontSize(9);
                });
            });

            col.Item().PaddingTop(12).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(3);
                    c.RelativeColumn(2);
                });

                table.Header(h =>
                {
                    h.Cell().Background("#1565C0").Padding(5).Text("DÉSIGNATION").Bold().FontColor(Colors.White);
                    h.Cell().Background("#1565C0").Padding(5).Text("MONTANT (XAF)").Bold().FontColor(Colors.White).AlignRight();
                });

                void LigneGain(string label, decimal montant)
                {
                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(label);
                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"{montant:N0}").AlignRight();
                }

                void LigneDeduction(string label, decimal montant)
                {
                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(label).FontColor(Colors.Red.Darken1);
                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"- {montant:N0}").AlignRight().FontColor(Colors.Red.Darken1);
                }

                LigneGain("Salaire de base", _salaire.SalaireBase);
                if (_salaire.PrimeTotal > 0) LigneGain("Primes et bonus", _salaire.PrimeTotal);
                if (_salaire.Retenues > 0) LigneDeduction("Retenues diverses", _salaire.Retenues);
                if (_salaire.AvanceDeduite > 0) LigneDeduction("Avance sur salaire déduite", _salaire.AvanceDeduite);

                table.Cell().ColumnSpan(2)
                    .Background("#1565C0").Padding(8)
                    .Row(r =>
                    {
                        r.RelativeItem().Text("NET À PAYER").Bold().FontSize(13).FontColor(Colors.White);
                        r.RelativeItem().Text($"{_salaire.NetAPayer:N0} XAF").Bold().FontSize(13).FontColor(Colors.White).AlignRight();
                    });
            });

            col.Item().PaddingTop(5).Text(
                _salaire.DatePaiement.HasValue
                    ? $"Payé le {_salaire.DatePaiement.Value:dd/MM/yyyy}"
                    : "Statut : En attente de paiement")
                .FontSize(10).FontColor(_salaire.Statut == "paye" ? "#2E7D32" : "#F57F17").AlignRight();

            col.Item().PaddingTop(30).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Signature Employé").AlignCenter().SemiBold();
                    c.Item().PaddingTop(30).Text("_______________________").AlignCenter();
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Cachet Direction").AlignCenter().SemiBold();
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
            col.Item().PaddingTop(3).Text($"BEHGest Pro — Bulletin de paie confidentiel — {DateTime.Now:dd/MM/yyyy}")
                .FontSize(8).FontColor(Colors.Grey.Darken1).AlignCenter();
        });
    }
}
