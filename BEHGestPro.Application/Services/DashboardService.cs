using BEHGestPro.Application.DTOs;
using BEHGestPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Application.Services;

public class DashboardService
{
    private readonly IDbContextFactory<BehGestionDbContext> _factory;

    public DashboardService(IDbContextFactory<BehGestionDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        await using var ctx = await _factory.CreateDbContextAsync();

        // Toutes les requêtes sur le même contexte, séquentiellement
        var totalApprenants    = await ctx.Apprenants.CountAsync();
        var formationsActives  = await ctx.SessionsFormation.CountAsync(s => s.Statut == "ouverte");
        var commandesEnCours   = await ctx.Commandes.CountAsync(c => c.Statut == "en_cours");
        var paiementsMois      = await GetTotalMoisActuelAsync(ctx);
        var stagiairesActuels  = await ctx.Stagiaires.CountAsync(s => s.Statut == "actif");
        var salairesEnAttente  = await ctx.Salaires.CountAsync(s => s.Statut == "en_attente");

        var now   = DateTime.Today;
        var debut = new DateTime(now.Year, now.Month, 1).AddMonths(-11);

        var inscriptionsMensuelles = await ctx.InscriptionsFormation
            .Where(i => i.DateInscription >= debut)
            .GroupBy(i => new { i.DateInscription.Year, i.DateInscription.Month })
            .Select(g => new MoisInscriptionDto
            {
                Annee         = g.Key.Year,
                Mois          = g.Key.Month,
                NbInscriptions = g.Count()
            })
            .OrderBy(m => m.Annee).ThenBy(m => m.Mois)
            .ToListAsync();

        return new DashboardStatsDto
        {
            TotalApprenants        = totalApprenants,
            FormationsActives      = formationsActives,
            CommandesEnCours       = commandesEnCours,
            PaiementsMois          = paiementsMois,
            StagiairesActuels      = stagiairesActuels,
            SalairesEnAttente      = salairesEnAttente,
            InscriptionsMensuelles = inscriptionsMensuelles
        };
    }

    private static async Task<decimal> GetTotalMoisActuelAsync(BehGestionDbContext ctx)
    {
        var now = DateTime.Today;
        return await ctx.Paiements
            .Where(p => p.DatePaiement.Year == now.Year && p.DatePaiement.Month == now.Month)
            .SumAsync(p => (decimal?)p.MontantVerse) ?? 0m;
    }
}
