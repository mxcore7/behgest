using BEHGestPro.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BEHGestPro.Documents.Badges;

public class BadgeEmployeDocument : IDocument
{
    private readonly Employe _employe;
    private readonly string _nomSociete;

    public BadgeEmployeDocument(Employe employe, string nomSociete = "BEH GESTION")
    {
        _employe = employe;
        _nomSociete = nomSociete;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            // Format Badge professionnel ID (création format A7 portrait pour impression facile ou badge standard)
            page.Size(74, 105, Unit.Millimetre);
            page.Margin(0);
            page.DefaultTextStyle(x => x.FontFamily("Calibri"));
            
            page.Content().Element(ComposeBadge);
        });
    }

    private void ComposeBadge(IContainer container)
    {
        container.Border(1).BorderColor("#1565C0").Column(col =>
        {
            // En-tête bleu
            col.Item().Background("#1565C0").PaddingVertical(8).PaddingHorizontal(5).Column(header =>
            {
                header.Item().Text(_nomSociete).Bold().FontSize(12).FontColor(Colors.White).AlignCenter();
                header.Item().Text("CARTE D'IDENTIFICATION").FontSize(7).SemiBold().FontColor("#00ACC1").AlignCenter();
            });

            // Espace photo & Infos
            col.Item().Padding(8).Column(body =>
            {
                // Encadré Photo
                var photoPath = _employe.PhotoPath;
                // Si le chemin est relatif, le reconstituer à partir du répertoire de l'application
                if (!string.IsNullOrWhiteSpace(photoPath) && !System.IO.Path.IsPathRooted(photoPath))
                    photoPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, photoPath);

                if (!string.IsNullOrWhiteSpace(photoPath) && System.IO.File.Exists(photoPath))
                {
                    body.Item().AlignCenter().Width(35, Unit.Millimetre).Height(35, Unit.Millimetre)
                        .Image(photoPath);
                }
                else
                {
                    body.Item().AlignCenter().Width(35, Unit.Millimetre).Height(35, Unit.Millimetre)
                        .Background("#E0F7FA").Border(1).BorderColor("#00ACC1")
                        .AlignCenter().AlignMiddle()
                        .Text("PHOTO").FontSize(8).Bold().FontColor("#00838F");
                }

                body.Item().PaddingTop(6).Text($"{_employe.Prenom} {_employe.Nom}".ToUpper())
                    .Bold().FontSize(11).FontColor("#1565C0").AlignCenter();

                body.Item().PaddingTop(2).Text(_employe.Poste ?? "Employé")
                    .SemiBold().FontSize(9).FontColor(Colors.Grey.Darken3).AlignCenter();

                if (!string.IsNullOrWhiteSpace(_employe.Departement))
                {
                    body.Item().Text(_employe.Departement.ToUpper())
                        .FontSize(7).FontColor(Colors.Grey.Darken1).AlignCenter();
                }

                body.Item().PaddingTop(6).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                // Matricule et date
                body.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("MATRICULE").FontSize(6).FontColor(Colors.Grey.Medium);
                        c.Item().Text(_employe.Matricule).FontSize(8).Bold();
                    });
                });

                body.Item().PaddingTop(2).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("EMBAUCHE").FontSize(6).FontColor(Colors.Grey.Medium);
                        c.Item().Text(_employe.DateEmbauche?.ToString("dd/MM/yyyy") ?? "—").FontSize(8).SemiBold();
                    });
                });
            });

            // Pied de page
            col.Item().Background("#F1F5F9").PaddingVertical(4).Text("Ce badge est strictement personnel")
                .FontSize(6).Italic().FontColor(Colors.Grey.Darken1).AlignCenter();
        });
    }
}
