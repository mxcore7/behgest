using BEHGestPro.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;

namespace BEHGestPro.Documents.Commandes;

public class BonCommandeDocument : IDocument
{
    private readonly Commande _commande;
    private readonly string _nomSociete;

    public BonCommandeDocument(Commande commande, string nomSociete = "BEH GESTION")
    {
        _commande = commande;
        _nomSociete = nomSociete;
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
                    c.Item().Text(_nomSociete).Bold().FontSize(16).FontColor("#1565C0");
                    c.Item().Text("SERVICE DES PRESTATIONS ET COMMANDES").FontSize(9).FontColor(Colors.Grey.Darken2);
                });
                row.ConstantItem(140).AlignRight().Column(c =>
                {
                    c.Item().Text("BON DE COMMANDE").Bold().FontSize(13).FontColor("#1E2130");
                    c.Item().Text($"N° : {_commande.Numero}").SemiBold().FontSize(10);
                    c.Item().Text($"Date : {_commande.DateCommande:dd/MM/yyyy}").FontSize(9);
                });
            });
            col.Item().PaddingTop(8).LineHorizontal(1.5f).LineColor("#1565C0");
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingTop(15).Column(col =>
        {
            col.Item().Background("#F8FAFC").Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("CLIENT / DONNEUR D'ORDRE").SemiBold().FontSize(9).FontColor(Colors.Grey.Darken2);
                    c.Item().Text(_commande.ClientNom).Bold().FontSize(12);
                    if (!string.IsNullOrWhiteSpace(_commande.ClientTelephone))
                        c.Item().Text($"Tél : {_commande.ClientTelephone}").FontSize(9);
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("RESPONSABLE DOSSIER").SemiBold().FontSize(9).FontColor(Colors.Grey.Darken2);
                    c.Item().Text(_commande.Employe?.NomComplet ?? "Non assigné").Bold().FontSize(11);
                });
            });

            col.Item().PaddingTop(15).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(3);
                    c.RelativeColumn(2);
                    c.RelativeColumn(2);
                });

                table.Header(h =>
                {
                    h.Cell().Background("#1565C0").Padding(6).Text("TYPE DE SERVICE").Bold().FontColor(Colors.White);
                    h.Cell().Background("#1565C0").Padding(6).Text("LIVRAISON PRÉVUE").Bold().FontColor(Colors.White).AlignCenter();
                    h.Cell().Background("#1565C0").Padding(6).Text("COÛT TOTAL (XAF)").Bold().FontColor(Colors.White).AlignRight();
                });

                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(6).Text(_commande.TypeService);
                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(6).Text(_commande.DateLivraisonPrevue?.ToString("dd/MM/yyyy") ?? "—").AlignCenter();
                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(6).Text($"{_commande.CoutTotal:N0}").AlignRight().SemiBold();

                table.Cell().ColumnSpan(2).Background("#F1F5F9").Padding(8).Text("MONTANT TOTAL PRESTATION").Bold().AlignRight();
                table.Cell().Background("#1565C0").Padding(8).Text($"{_commande.CoutTotal:N0} XAF").Bold().FontColor(Colors.White).AlignRight();
            });

            if (!string.IsNullOrWhiteSpace(_commande.Description))
            {
                col.Item().PaddingTop(12).Text("Description / Détails :").SemiBold().FontSize(10);
                col.Item().Text(_commande.Description).FontSize(9).Italic();
            }

            col.Item().PaddingTop(10).Text($"Statut actuel : {_commande.Statut.ToUpper()}").FontSize(9).Italic();

            col.Item().PaddingTop(40).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Le Client").AlignCenter().SemiBold();
                    c.Item().PaddingTop(35).Text("_______________________").AlignCenter();
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Validation Direction").AlignCenter().SemiBold();
                    c.Item().PaddingTop(35).Text("_______________________").AlignCenter();
                });
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
            col.Item().PaddingTop(3).Text($"BEHGest Pro — Bon de commande officiel — {DateTime.Now:dd/MM/yyyy HH:mm}")
                .FontSize(8).FontColor(Colors.Grey.Darken1).AlignCenter();
        });
    }
}
