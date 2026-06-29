namespace BEHGestPro.UI.Configuration;

public class AppSettings
{
    public string AppName { get; set; } = "BEHGest Pro";
    public string Version { get; set; } = "1.0.0";
    public string LogoPath { get; set; } = "Assets/logo.png";
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyAddress { get; set; } = string.Empty;
    public string CompanyPhone { get; set; } = string.Empty;
    public string DefaultCurrency { get; set; } = "XAF";
    public string Theme { get; set; } = "Dark";
}
