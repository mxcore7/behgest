using System;

namespace BEHGestPro.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public int? UtilisateurId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Entite { get; set; } = string.Empty;
    public int? EntiteId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Ip { get; set; }
}
