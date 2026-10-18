using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using BLY.Models;
using BLY.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace BLY.ViewModels;

// ---------- 33 / 34 Historique ----------
public partial class HistoryViewModel : ObservableObject
{
    readonly List<MovementGroup> _all = DemoData.HistoryGroups();
    public ObservableCollection<MovementGroup> Groups { get; } = new();

    [ObservableProperty] string _search = "";
    [ObservableProperty] bool _showFilters;
    [ObservableProperty] bool _showEntries = true;
    [ObservableProperty] bool _showExits = true;
    [ObservableProperty] bool _showAdjust;
    [ObservableProperty] bool _showTransfers;
    [ObservableProperty] int _period = 1; // 0 aujourd'hui, 1 sept jours, 2 trente jours, 3 personnalisée
    [ObservableProperty] bool _zoneA;
    [ObservableProperty] bool _zoneB;
    [ObservableProperty] bool _zoneC;
    [ObservableProperty] bool _quai;

    public bool IsToday => Period == 0;
    public bool Is7Days => Period == 1;
    public bool Is30Days => Period == 2;
    public bool IsCustom => Period == 3;
    public string TypeSummary => ShowEntries && ShowExits && !ShowAdjust && !ShowTransfers ? "Entrées et sorties" : "Types filtrés";
    public string PeriodSummary => Period switch { 0 => "Aujourd'hui", 1 => "7 jours", 2 => "30 jours", _ => "Personnalisée" };
    public int Count => Groups.Sum(g => g.Count);
    public string ApplyText => $"Afficher {Count} mouvement{(Count > 1 ? "s" : "")}";
    public bool IsEmpty => Count == 0;

    public HistoryViewModel() => Refresh();

    partial void OnSearchChanged(string value) => Refresh();
    partial void OnShowEntriesChanged(bool value) => Refresh();
    partial void OnShowExitsChanged(bool value) => Refresh();
    partial void OnShowAdjustChanged(bool value) => Refresh();
    partial void OnShowTransfersChanged(bool value) => Refresh();
    partial void OnPeriodChanged(int value)
    {
        OnPropertyChanged(nameof(IsToday)); OnPropertyChanged(nameof(Is7Days)); OnPropertyChanged(nameof(Is30Days)); OnPropertyChanged(nameof(IsCustom));
        Refresh();
    }

    void Refresh()
    {
        Groups.Clear();
        var groups = Period == 0 ? _all.Take(1) : _all;
        foreach (var g in groups)
        {
            var items = g.Where(m =>
                (m.Kind == MovementKind.Entry && ShowEntries) || (m.Kind == MovementKind.Exit && ShowExits) ||
                (m.Kind == MovementKind.Adjust && ShowAdjust) || (m.Kind == MovementKind.Transfer && ShowTransfers))
                .Where(m => string.IsNullOrWhiteSpace(Search) || m.Title.Contains(Search, StringComparison.OrdinalIgnoreCase) || m.Subtitle.Contains(Search, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (items.Count > 0) Groups.Add(new MovementGroup(g.Title, items));
        }
        OnPropertyChanged(nameof(Count)); OnPropertyChanged(nameof(ApplyText)); OnPropertyChanged(nameof(IsEmpty));
        OnPropertyChanged(nameof(TypeSummary)); OnPropertyChanged(nameof(PeriodSummary));
    }

    [RelayCommand] void OpenFilters() => ShowFilters = true;
    [RelayCommand] void CloseFilters() => ShowFilters = false;
    [RelayCommand] void SetPeriod(string p) => Period = int.Parse(p);
    [RelayCommand]
    void ClearFilters()
    {
        ShowEntries = ShowExits = true; ShowAdjust = ShowTransfers = false; Period = 1; ZoneA = ZoneB = ZoneC = Quai = false; Search = "";
    }
    [RelayCommand] Task OpenMovement(Movement m) => Nav.Go($"{Routes.MovementDetail}?id={m.Id}");
    [RelayCommand] Task Scan() => Nav.Go(Routes.Scan);
}

// ---------- 35 Détail d'un mouvement ----------
public record DetailRow(string Label, string Value);

public partial class MovementDetailViewModel : ObservableObject, IQueryAttributable
{
    public SyncService Sync { get; }
    [ObservableProperty] string _id = "M-10482";
    public List<DetailRow> Rows { get; } = new()
    {
        new("Article", "Ciment CPJ 42,5R — sac 50 kg"),
        new("De", "Quai de réception (QUAI-01)"),
        new("Vers", "A-03-B-2 · Zone A"),
        new("Document", "BR-0231 · Kamga & Fils"),
        new("Effectué par", "Jean Mbarga · magasinier"),
        new("Date", "06/10/2026 à 09:12"),
        new("Saisie", "Scan · hors ligne"),
        new("Valeur", "588 000 FCFA"),
    };
    public string Title => $"Mouvement {Id}";
    public string SyncText => $"Synchronisé le 06/10/2026 à 09:41";

    public MovementDetailViewModel(SyncService sync) => Sync = sync;
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var v) && v is string s) { Id = s; OnPropertyChanged(nameof(Title)); }
    }

    [RelayCommand] Task OpenLocation() => Nav.Go($"{Routes.LocationDetail}?code=A-03-B-2");
    [RelayCommand] Task Print() => Nav.Alert("Impression", "Impression du mouvement (à brancher).");
}

// ---------- 36 Profil et paramètres ----------
public partial class SettingsViewModel : ObservableObject
{
    public SessionService Session { get; }
    public SyncService Sync { get; }
    [ObservableProperty] bool _darkMode;

    public string ProfileLine => $"{Session.RoleLabel} · {Session.Phone}";
    public string LastSyncText => $"Activé · dernière à {Sync.LastSyncText}";
    public string WarehouseText => $"{Session.WarehouseName} · {Session.WarehouseCity}";

    public SettingsViewModel(SessionService session, SyncService sync)
    {
        Session = session; Sync = sync;
        _darkMode = Application.Current?.UserAppTheme == AppTheme.Dark ||
                    (Application.Current?.UserAppTheme == AppTheme.Unspecified && Application.Current?.RequestedTheme == AppTheme.Dark);
    }

    partial void OnDarkModeChanged(bool value)
    {
        if (Application.Current != null) Application.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
    }

    [RelayCommand] Task Warehouse() => Nav.Go(Routes.Warehouse);
    [RelayCommand] Task Language() => Nav.Alert("Langue", "Français (autres langues à venir).");
    [RelayCommand] Task Printer() => Nav.Alert("Imprimante d'étiquettes", "Thermique 80 mm · connectée en Bluetooth.");
    [RelayCommand] Task Help() => Nav.Alert("Aide et support", "Guide d'utilisation et contact du support (à brancher).");
    [RelayCommand] Task OpenSync() => Nav.Go(Routes.Sync);
    [RelayCommand]
    async Task LogoutAsync()
    {
        if (await Shell.Current.DisplayAlert("Se déconnecter", "Vos opérations hors ligne sont conservées.", "Se déconnecter", "Annuler"))
        { Session.SignOut(); await Nav.Go(Routes.Login); }
    }
}

// ---------- 18 (central) / 37 / 38 Scanner ----------
public partial class ScanViewModel : ObservableObject, IQueryAttributable
{
    string _mode = "dispatch", _target = "";
    [ObservableProperty] string _title = "Scanner";
    [ObservableProperty] string _subtitle = "Article, emplacement ou bon";
    [ObservableProperty] bool _hasError;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("mode", out var m)) _mode = m?.ToString() ?? "dispatch";
        if (query.TryGetValue("target", out var t)) _target = t?.ToString() ?? "";
        if (_mode == "return") Subtitle = "Scannez le code demandé";
    }

    /// <summary>Le Scanner central ouvre l'écran adapté au code lu : bon, emplacement ou article.</summary>
    public async Task HandleCodeAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) { HasError = true; return; }
        if (_mode == "return")
        {
            WeakReferenceMessenger.Default.Send(new ScanResultMessage(code, _target));
            await Nav.Back();
            return;
        }
        if (Regex.IsMatch(code, @"^[A-Z]-\d{2}-[A-Z]-\d$", RegexOptions.IgnoreCase) || code.StartsWith("QUAI", StringComparison.OrdinalIgnoreCase))
            await Nav.Go($"../{Routes.LocationDetail}?code={code.ToUpperInvariant()}");
        else if (code.StartsWith("BS-", StringComparison.OrdinalIgnoreCase))
            await Nav.Go($"../{Routes.Picking}?id={code.ToUpperInvariant()}");
        else if (code.StartsWith("BR-", StringComparison.OrdinalIgnoreCase))
            await Nav.Go($"../{Routes.Reception}?id={code.ToUpperInvariant()}");
        else
            await Nav.Go($"../{Routes.Locate}?query={Uri.EscapeDataString(code)}");
    }
}

// ---------- 39 Échec de synchronisation ----------
public partial class SyncViewModel : ObservableObject
{
    public SyncService Sync { get; }
    public List<Movement> Ops { get; } = DemoData.PendingOps().Select(p => new Movement("", p.Title, p.Detail, p.Qty, "unités", p.Kind)).ToList();
    [ObservableProperty, NotifyPropertyChangedFor(nameof(HasFailed), nameof(Subtitle))] bool _isDone;
    [ObservableProperty] bool _isRetrying;

    public bool HasFailed => !IsDone;
    public string Subtitle => IsDone ? "Tout est synchronisé" : $"{Ops.Count} opérations bloquées";
    public string BannerText => $"Échec de synchro · {Ops.Count} opérations";
    public string LastTry => "Dernière tentative à 11:05 · délai dépassé (réseau 2G)";

    public SyncViewModel(SyncService sync) => Sync = sync;

    [RelayCommand]
    async Task RetryAsync()
    {
        IsRetrying = true;
        await Sync.SyncNowAsync();
        IsRetrying = false;
        IsDone = true;
    }

    [RelayCommand] Task Export() => Nav.Alert("Journal", "Export du journal de synchronisation (à brancher).");
}
