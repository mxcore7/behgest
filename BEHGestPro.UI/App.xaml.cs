using BEHGestPro.Application.Services;
using BEHGestPro.Infrastructure.Data;
using BEHGestPro.Infrastructure.Repositories;
using BEHGestPro.UI.Configuration;
using BEHGestPro.UI.ViewModels;
using BEHGestPro.UI.ViewModels.Apprenants;
using BEHGestPro.UI.ViewModels.Commandes;
using BEHGestPro.UI.ViewModels.Employes;
using BEHGestPro.UI.ViewModels.Formations;
using BEHGestPro.UI.ViewModels.Paiements;
using BEHGestPro.UI.ViewModels.Salaires;
using BEHGestPro.UI.ViewModels.Stagiaires;
using BEHGestPro.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Windows;

namespace BEHGestPro.UI;

public partial class App : System.Windows.Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        // Fix Npgsql DateTime Kind=Unspecified error (Npgsql 6+ requires explicit UTC)
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        QuestPDF.Settings.License = LicenseType.Community;

        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                var connStr = config.GetConnectionString("DefaultConnection")!;
                var appSettings = config.GetSection("AppSettings").Get<AppSettings>() ?? new AppSettings();

                services.AddSingleton(appSettings);
                services.AddSingleton(config);

                // DbContext factory (Singleton - thread-safe, creates short-lived contexts)
                services.AddDbContextFactory<BehGestionDbContext>(opt =>
                    opt.UseNpgsql(connStr));

                // Transient DbContext for services/repositories (each gets a fresh instance)
                services.AddTransient<BehGestionDbContext>(sp =>
                    sp.GetRequiredService<IDbContextFactory<BehGestionDbContext>>().CreateDbContext());

                // Repositories
                services.AddTransient<ApprenantRepository>();
                services.AddTransient<StagiaireRepository>();
                services.AddTransient<FormationRepository>();
                services.AddTransient<PaiementRepository>();
                services.AddTransient<CommandeRepository>();
                services.AddTransient<SalaireRepository>();
                services.AddTransient<EmployeRepository>();

                // Services
                services.AddSingleton<AuthService>();
                services.AddTransient<ApprenantService>();
                services.AddTransient<StagiaireService>();
                services.AddTransient<FormationService>();
                services.AddTransient<PaiementService>();
                services.AddTransient<CommandeService>();
                services.AddTransient<SalaireService>();
                services.AddTransient<EmployeService>();
                services.AddTransient<DashboardService>();

                // ViewModels
                services.AddTransient<LoginViewModel>();
                services.AddTransient<MainViewModel>();
                services.AddTransient<DashboardViewModel>();
                services.AddTransient<ApprenantListViewModel>();
                services.AddTransient<ApprenantFormViewModel>();
                services.AddTransient<StagiaireListViewModel>();
                services.AddTransient<StagiaireFormViewModel>();
                services.AddTransient<FormationListViewModel>();
                services.AddTransient<FormationFormViewModel>();
                services.AddTransient<PaiementListViewModel>();
                services.AddTransient<PaiementFormViewModel>();
                services.AddTransient<CommandeListViewModel>();
                services.AddTransient<CommandeFormViewModel>();
                services.AddTransient<SalaireListViewModel>();
                services.AddTransient<SalaireFormViewModel>();
                services.AddTransient<EmployeListViewModel>();
                services.AddTransient<EmployeFormViewModel>();

                // Views
                services.AddTransient<LoginView>();
                services.AddTransient<MainWindow>();
            })
            .Build();

        await _host.StartAsync();

        // Ensure DB is created
        try
        {
            var dbFactory = _host.Services.GetRequiredService<IDbContextFactory<BehGestionDbContext>>();
            await using var db = await dbFactory.CreateDbContextAsync();
            await db.Database.EnsureCreatedAsync();

            var authService = _host.Services.GetRequiredService<AuthService>();
            if (!await authService.HasUsersAsync())
            {
                await authService.CreateUserAsync("Admin", "Système", "admin@behgest.com", "admin", "admin");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur de connexion à la base de données :\n{ex.Message}",
                "BEHGest Pro — Erreur critique", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }

        var loginView = _host.Services.GetRequiredService<LoginView>();
        loginView.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
        base.OnExit(e);
    }

    public static T GetService<T>() where T : notnull =>
        ((App)Current)._host!.Services.GetRequiredService<T>();
}
