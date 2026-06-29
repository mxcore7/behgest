using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace BEHGestPro.UI.Services;

public static class PdfService
{
    public static void GenerateAndOpenPdf(IDocument document, string fileNamePrefix)
    {
        try
        {
            var outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BEHGestPro_PDFs");
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            var cleanPrefix = string.Join("_", fileNamePrefix.Split(Path.GetInvalidFileNameChars()));
            var filePath = Path.Combine(outputDir, $"{cleanPrefix}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            document.GeneratePdf(filePath);

            if (File.Exists(filePath))
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors de la génération ou de l'ouverture du document PDF :\n{ex.Message}",
                            "Erreur PDF", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
