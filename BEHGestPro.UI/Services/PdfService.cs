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
            var cleanPrefix = string.Join("_", fileNamePrefix.Split(Path.GetInvalidFileNameChars()));
            var fileName = $"{cleanPrefix}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string filePath = null;

            try
            {
                var outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BEHGestPro_PDFs");
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }
                filePath = Path.Combine(outputDir, fileName);
                document.GeneratePdf(filePath);
            }
            catch
            {
                // Fallback to Temp directory if MyDocuments is not accessible or causes any path issues
                var tempDir = Path.Combine(Path.GetTempPath(), "BEHGestPro_PDFs");
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }
                filePath = Path.Combine(tempDir, fileName);
                document.GeneratePdf(filePath);
            }

            if (filePath != null && File.Exists(filePath))
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
