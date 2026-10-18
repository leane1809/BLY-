namespace BLY.Models;

/// <summary>
/// Données de démonstration issues des maquettes. À remplacer par la base locale (SQLite)
/// et la synchronisation par lots lorsque le back-end sera prêt.
/// </summary>
public static class DemoData
{
    public static List<WarehouseOption> Warehouses() => new()
    {
        new WarehouseOption { Name = "Dépôt Bonabéri", Detail = "Douala · 3 zones · 214 emplacements · dernier accès aujourd'hui", IsSelected = true },
        new WarehouseOption { Name = "Magasin Akwa", Detail = "Douala · 1 zone · 38 emplacements" },
        new WarehouseOption { Name = "Entrepôt Mvan", Detail = "Yaoundé · 2 zones · 120 emplacements" },
    };

    public static List<DashboardTile> Tiles() => new()
    {
        new("icon_download", "8", "Réceptions", "3 à contrôler", K.BgInfo, K.Info, K.Secondaire),
        new("icon_upload", "14", "Sorties", "2 bons en attente", K.BgInfo, K.Info, K.Secondaire),
        new("icon_triangle_alert", "5", "Écarts ouverts", "2 à valider", K.BgAlerte, K.Alerte, K.Alerte),
        new("icon_package", "12", "Alertes de stock", "Sous le seuil", K.BgErreur, K.Erreur, K.Erreur),
    };

    public static List<Movement> RecentMovements() => new()
    {
        new("M-10482", "Ciment CPJ 42,5R — sac 50 kg", "Réception BR-0231 · A-03-B-2 · 09:12", "+120", "sacs", MovementKind.Entry),
        new("M-10481", "Huile végétale — bidon 20 L", "Bon BS-0412 · C-01-A-1 · 08:47", "−36", "bidons", MovementKind.Exit),
        new("M-10480", "Eau minérale 1,5 L — pack de 6", "Inventaire · C-01-A-2 · 09:05", "−3", "packs", MovementKind.Adjust),
    };

    public static List<MovementGroup> HistoryGroups() => new()
    {
        new MovementGroup("Aujourd'hui — 06/10/2026", RecentMovements()),
        new MovementGroup("Hier — 05/10/2026", new[]
        {
            new Movement("M-10470", "Casier bière 65 cl — 12 bouteilles", "Bon BS-0409 · C-02-B-1 · 17:20", "−24", "casiers", MovementKind.Exit),
            new Movement("M-10466", "Riz parfumé — sac 25 kg", "Réception BR-0228 · B-02-A-3 · 10:35", "+80", "sacs", MovementKind.Entry),
        }),
    };

    public static List<Zone> Zones() => new()
    {
        new("A", "Zone A — Matériaux", "4 allées · 96 emplacements", 0.96, "Presque plein", "icon_circle_alert", K.Erreur, K.BarErreur),
        new("B", "Zone B — Alimentaire", "3 allées · 72 emplacements", 0.62, "Disponible", "icon_check", K.Succes, K.BarMarque),
        new("C", "Zone C — Boissons", "2 allées · 46 emplacements", 0.18, "Stock bas", "icon_triangle_alert", K.Alerte, K.BarAlerte),
    };

    public static List<LocationContent> LocationContents() => new()
    {
        new("Ciment CPJ 42,5R — sac 50 kg", "CIM-425-50 · lot du 02/09", "120", "sacs"),
        new("Ciment CPJ 32,5 — sac 50 kg", "CIM-325-50 · lot du 28/08", "40", "sacs"),
    };

    public static List<ReceptionSummary> Receptions() => new()
    {
        new("BR-0231", "Kamga & Fils", "Ciment, fer à béton, tôles · 3 lignes", "Prévue 10:00"),
        new("BR-0232", "Fotso Alimentation", "Bafoussam · riz, huile, sucre · 6 lignes", "Prévue 14:30"),
        new("BR-0233", "Brasserie du Littoral", "Casiers de bière, eau minérale · 4 lignes", "Prévue 16:00"),
        new("BR-0229", "Grossiste Njoya Pharma", "Yaoundé · 8 lignes", "Prévue demain"),
    };

    public static List<ReceptionLine> ReceptionLines() => new()
    {
        new() { Article = "Fer à béton HA 12 — barre 12 m", Reference = "FER-HA12", Expected = 40, Received = 40, Unit = "barres", UnitPrice = "5 900 FCFA", DefaultLocation = "B-01-A-1" },
        new() { Article = "Ciment CPJ 42,5R — sac 50 kg", Reference = "CIM-425-50", Expected = 120, Received = 120, Unit = "sacs", UnitPrice = "4 900 FCFA", DefaultLocation = "A-03-B-2" },
        new() { Article = "Tôle bac 0,40 mm — 2 m", Reference = "TOL-040-2", Expected = 50, Received = 50, Unit = "pièces", UnitPrice = "8 756 FCFA", DefaultLocation = "B-02-C-3" },
    };

    public static List<LocationSuggestion> Suggestions() => new()
    {
        new() { Code = "B-01-A-1", Reason = "Même article déjà stocké · 60 places libres", IsSuggested = true, IsSelected = true },
        new() { Code = "B-01-B-1", Reason = "Emplacement vide · le plus proche du quai" },
        new() { Code = "B-02-A-3", Reason = "Zone B · 2 palettes libres" },
    };

    public static List<IssueNote> IssueNotes() => new()
    {
        new("BS-0412", "Quincaillerie Ngo Bayiha", "Bassa, Douala · 5 lignes · 1 250 000 FCFA", "En préparation", "info", "icon_package", "Aujourd'hui 14:00"),
        new("BS-0413", "Ets Nkeng", "Bamenda · 4 lignes · 812 000 FCFA", "En attente", "info", "icon_clock", "Aujourd'hui 16:00"),
        new("BS-0414", "Pharmacie Abena", "Essos, Yaoundé · 9 lignes · 2 340 000 FCFA", "En attente", "info", "icon_clock", "Demain 08:00"),
        new("BS-0409", "Bar Le Kamga", "Akwa, Douala · 3 lignes · 186 000 FCFA", "Expédié", "violet", "icon_truck", "Hier 17:20"),
    };

    public static List<IssueLine> IssueLines() => new()
    {
        new("Ciment CPJ 42,5R — sac 50 kg", "A-03-B-2 · 4 900 FCFA", "−36", "sacs"),
        new("Fer à béton HA 12 — barre 12 m", "B-01-A-1 · 5 900 FCFA", "−20", "barres"),
        new("Peinture glycéro blanche — 20 L", "C-02-A-1 · 62 500 FCFA", "−6", "bidons"),
        new("Tôle bac 0,40 mm — 2 m", "B-02-C-3 · 8 756 FCFA", "−50", "pièces"),
        new("Pointes 80 mm — carton 25 kg", "B-03-A-2 · 14 280 FCFA", "−10", "cartons"),
    };

    public static List<StockLocation> Locate() => new()
    {
        new("A-03-B-2", "Zone A · Allée 03 · Rayon B · Niveau 2", "640", "sacs"),
        new("A-04-A-1", "Zone A · Allée 04 · Rayon A · Niveau 1", "480", "sacs"),
        new("QUAI-01", "Quai de réception · à ranger, arrivé à 09:12", "120", "sacs"),
    };

    public static List<CountTask> CountToday() => new()
    {
        new("B-02-C-3", "Tôles", "Zone B · 1 article · jamais compté", "1 article"),
        new("B-01-A-1", "Fer à béton", "Zone B · 1 article · dernier comptage 12/09", "1 article"),
        new("A-03-B-2", "Ciment", "Zone A · 2 articles", "2 articles"),
    };

    public static List<VarianceItem> Variances() => new()
    {
        new() { Article = "Tôle bac 0,40 mm — 2 m", Code = "B-02-C-3", Theoretical = 120, Counted = 114, Reason = "Casse lors de la manutention" },
        new() { Article = "Eau minérale 1,5 L — pack de 6", Code = "C-01-A-2", Theoretical = 36, Counted = 38, Reason = "Erreur de réception BR-0228" },
        new() { Article = "Fer à béton HA 12 — barre 12 m", Code = "B-01-A-1", Theoretical = 60, Counted = 60 },
    };

    public static readonly string[] VarianceReasons =
    {
        "Casse lors de la manutention", "Erreur de réception BR-0228", "Erreur de préparation", "Vol ou perte présumée",
        "Erreur de saisie", "Autre motif"
    };

    public static List<PendingOp> PendingOps() => new()
    {
        new("Réception BR-0232 · Fotso Alimentation", "6 lignes · J. Mbarga · 10:48", "+214", MovementKind.Entry),
        new("Bon BS-0413 · Ets Nkeng", "4 lignes · A. Fotso · 11:02", "−58", MovementKind.Exit),
    };
}
