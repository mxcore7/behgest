using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BEHGestPro.UI.Converters;

public class StringToVisibilityConverter : IValueConverter
{
    public static readonly StringToVisibilityConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is string s && !string.IsNullOrEmpty(s) ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class InvertBoolConverter : IValueConverter
{
    public static readonly InvertBoolConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is bool b && !b;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class BoolToVisibilityConverter : IValueConverter
{
    public static readonly BoolToVisibilityConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is bool b && b ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class StatutToColorConverter : IValueConverter
{
    public static readonly StatutToColorConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "complet" or "paye" or "certifie" or "termine" or "livree" => "#2E7D32",
            "partiel" or "en_cours" or "inscrit" => "#F57F17",
            "en_attente" or "ouverte" => "#1565C0",
            "annulee" or "abandonne" => "#C62828",
            _ => "#78909C"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class StatutToLibelleConverter : IValueConverter
{
    public static readonly StatutToLibelleConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "complet"    => "Payé",
            "partiel"    => "Partiel",
            "en_attente" => "En attente",
            "paye"       => "Payé",
            "certifie"   => "Certifié",
            "inscrit"    => "Inscrit",
            "en_cours"   => "En cours",
            "termine"    => "Terminé",
            "abandonne"  => "Abandonné",
            "ouverte"    => "Ouverte",
            "fermee"     => "Fermée",
            "terminee"   => "Terminée",
            "annulee"    => "Annulée",
            "livree"     => "Livrée",
            _            => value?.ToString() ?? "—"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class MontantConverter : IValueConverter
{
    public static readonly MontantConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is decimal d ? $"{d:N0} XAF" : "— XAF";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
