using System.Collections.ObjectModel;
using BLY.Models;
using BLY.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BLY.ViewModels;

// ---------- 13 Zones ----------
public partial class ZonesViewModel : ObservableObject
{
    readonly List<Zone> _all = DemoData.Zones();
    public SessionService Session { get; }

    [ObservableProperty] string _search = "";
    [ObservableProperty] int _filter; // 0 toutes, 1 presque pleines, 2 bloquées
    public ObservableCollection<Zone> Zones { get; } = new();

    public bool IsAll => Filter == 0;
    public bool IsFull => Filter == 1;
    public bool IsBlocked => Filter == 2;
    /// <summary>« + » réservé au gérant (masqué, pas grisé, pour le magasinier).</summary>
    public string AddIcon => Session.IsManager ? "icon_plus" : "";

    public ZonesViewModel(SessionService session) { Session = session; Refresh(); }

    partial void OnSearchChanged(string value) => Refresh();
    partial void OnFilterChanged(int value)
    {
        OnPropertyChanged(nameof(IsAll)); OnPropertyChanged(nameof(IsFull)); OnPropertyChanged(nameof(IsBlocked));
        Refresh();
    }

    void Refresh()
    {
        Zones.Clear();
        foreach (var z in _all)
        {
            if (Filter == 1 && z.Fill < 0.9) continue;
            if (Filter == 2) continue; // aucun emplacement bloqué dans les données de démo
            if (!string.IsNullOrWhiteSpace(Search) && !z.Name.Contains(Search, StringComparison.OrdinalIgnoreCase)) continue;
            Zones.Add(z);
        }
    }

    [RelayCommand] void SetFilter(string index) => Filter = int.Parse(index);
    [RelayCommand] Task OpenZone(Zone z) => Nav.Go($"{Routes.LocationDetail}?code=A-03-B-2");
    [RelayCommand] Task Add() => Session.IsManager ? Nav.Go(Routes.LocationCreate) : Task.CompletedTask;
    [RelayCommand] Task Scan() => Nav.Go(Routes.Scan);
}

// ---------- 14 Emplacement — détail ----------
public partial class LocationDetailViewModel : ObservableObject, IQueryAttributable
{
    public SessionService Session { get; }
    [ObservableProperty] string _code = "A-03-B-2";
    public List<LocationContent> Contents { get; } = DemoData.LocationContents();
    public Movement LastMovement { get; } = DemoData.RecentMovements()[0];
    public List<Movement> LastMovements => new() { LastMovement };
    public string EditIcon => Session.IsManager ? "icon_pencil" : "";

    public LocationDetailViewModel(SessionService session) => Session = session;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("code", out var c) && c is string s && s.Length > 0) Code = s;
    }

    [RelayCommand] Task Label() => Nav.Go($"{Routes.QrLabel}?code={Code}");
    [RelayCommand] Task Move() => Nav.Alert("Déplacer", "Scannez l'emplacement de destination (écran à brancher).");
    [RelayCommand] Task Edit() => Nav.Alert("Modifier", "Modification de l'emplacement (réservé au gérant).");
    [RelayCommand] Task OpenMovement() => Nav.Go($"{Routes.MovementDetail}?id={LastMovement.Id}");
}

// ---------- 15 Étiquette QR ----------
public partial class QrLabelViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty] string _code = "A-03-B-2";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(PrintText), nameof(CountUnit))] int _count = 2;
    public string PrintText => $"Imprimer {Count} étiquette{(Count > 1 ? "s" : "")}";
    public string CountUnit => Count > 1 ? "étiquettes" : "étiquette";
    public string Detail => "Zone A · Allée 03\nRayon B · Niveau 2\nBLY · Dépôt Bonabéri";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("code", out var c) && c is string s && s.Length > 0) Code = s;
    }

    [RelayCommand] void Increase() => Count = Math.Min(20, Count + 1);
    [RelayCommand] void Decrease() => Count = Math.Max(1, Count - 1);

    [RelayCommand]
    async Task PrintAsync()
    {
        // TODO : impression ESC/POS en Bluetooth (étiquette 80 mm)
        await Nav.Alert("Impression", $"{PrintText.ToLower()} envoyée à l'imprimante.");
        await Nav.Back();
    }
}

// ---------- 16 Création d'emplacement (gérant) ----------
public partial class LocationCreateViewModel : ObservableObject
{
    public List<string> ZoneNames { get; } = new() { "Zone A — Matériaux", "Zone B — Alimentaire", "Zone C — Boissons" };
    [ObservableProperty, NotifyPropertyChangedFor(nameof(GeneratedCode), nameof(ZoneSubtitle))] int _zoneIndex;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(GeneratedCode))] string _aisle = "04";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(GeneratedCode))] string _rack = "A";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(GeneratedCode))] string _level = "1";
    [ObservableProperty] string _capacity = "4 palettes";
    [ObservableProperty] bool _printAfter = true;
    [ObservableProperty] bool _heavyOnly;

    public string ZoneSubtitle => ZoneIndex >= 0 ? ZoneNames[ZoneIndex] : "";
    public string GeneratedCode => $"Code généré : {"ABC"[Math.Max(0, ZoneIndex)]}-{Aisle}-{Rack?.ToUpper()}-{Level}";

    [RelayCommand]
    async Task CreateAsync()
    {
        var code = GeneratedCode.Replace("Code généré : ", "");
        if (PrintAfter) await Nav.Go($"../{Routes.QrLabel}?code={code}");
        else { await Nav.Alert("Emplacement créé", code); await Nav.Back(); }
    }
}

// ---------- 17 / 41 / 42 Réceptions — liste ----------
public partial class ReceptionsViewModel : ObservableObject
{
    [ObservableProperty] int _tab;
    [ObservableProperty] bool _isError;
    public ObservableCollection<ReceptionSummary> Items { get; } = new();

    public bool IsAttendues => Tab == 0;
    public bool IsUnplanned => Tab == 1;
    public bool IsDone => Tab == 2;
    public bool IsEmpty => !IsError && Items.Count == 0;
    public bool HasItems => !IsError && Items.Count > 0;
    public string AttenduesText => $"Attendues · {DemoData.Receptions().Count}";
    public string Subtitle => $"{DemoData.Receptions().Count} attendues aujourd'hui";

    public ReceptionsViewModel() => Load();

    partial void OnTabChanged(int value)
    {
        OnPropertyChanged(nameof(IsAttendues)); OnPropertyChanged(nameof(IsUnplanned)); OnPropertyChanged(nameof(IsDone));
        Load();
    }

    void Load()
    {
        IsError = false;
        Items.Clear();
        if (Tab == 0) foreach (var r in DemoData.Receptions()) Items.Add(r); // TODO : charger depuis SQLite
        OnPropertyChanged(nameof(IsEmpty)); OnPropertyChanged(nameof(HasItems));
    }

    [RelayCommand] void SetTab(string index) => Tab = int.Parse(index);
    [RelayCommand] void Retry() => Load();
    [RelayCommand] Task Open(ReceptionSummary r) => Nav.Go($"{Routes.Reception}?id={r.Id}");
    [RelayCommand] Task NewReception() => Nav.Go($"{Routes.Reception}?id=BR-NP");
}

// ---------- 18 → 21 Réception guidée en 4 étapes ----------
public partial class ReceptionViewModel : ObservableObject, IQueryAttributable
{
    public List<ReceptionLine> Lines { get; } = DemoData.ReceptionLines();
    public List<LocationSuggestion> Suggestions { get; } = DemoData.Suggestions();

    [ObservableProperty] string _receptionId = "BR-0231";
    [ObservableProperty] string _supplier = "Kamga & Fils";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsStep1), nameof(IsStep2), nameof(IsStep3), nameof(IsStep4), nameof(Subtitle), nameof(Footer))] int _step = 1;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(CurrentLine), nameof(Subtitle))] int _lineIndex;
    [ObservableProperty] bool _controlChecked = true;

    public ReceptionLine CurrentLine => Lines[LineIndex];
    public bool IsStep1 => Step == 1;
    public bool IsStep2 => Step == 2;
    public bool IsStep3 => Step == 3;
    public bool IsStep4 => Step == 4;
    public ReceptionViewModel()
    {
        foreach (var l in Lines)
            l.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName != nameof(ReceptionLine.Received)) return;
                OnPropertyChanged(nameof(ContinueText)); OnPropertyChanged(nameof(RangePrompt));
            };
    }

    public string Title => $"Réception {ReceptionId}";
    public string Subtitle => Step == 4 ? "Vérifiez avant de valider" : Step == 1 ? $"{Supplier} · {Lines.Count} lignes" : $"Ligne {LineIndex + 1} sur {Lines.Count}";
    public string NextExpected => $"0/{CurrentLine.Expected}";
    public string Footer => Step switch { 2 => "Continuer · ranger", 3 => "Ranger", _ => "Valider la réception" };

    public string ContinueText => $"Continuer · ranger {CurrentLine.Received} {CurrentLine.Unit}";
    public string RangeText => $"Ranger en {Suggestions.First(s => s.IsSelected).Code}";
    public string RangePrompt => $"Où rangez-vous ces {CurrentLine.Received} {CurrentLine.Unit} ?";

    public int TotalUnits => Lines.Sum(l => l.Received);
    public string RecapArticles => $"{Lines.Count} lignes · {TotalUnits} unités";
    public bool HasAnyDelta => Lines.Any(l => l.HasDelta);
    public string RecapDelta => string.Join(", ", Lines.Where(l => l.HasDelta).Select(l => $"{l.DeltaText} {l.Unit}"));
    public string RecapValue => "1 250 000 FCFA";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var v) && v is string id) { ReceptionId = id == "BR-NP" ? "BR-NP (non planifiée)" : id; OnPropertyChanged(nameof(Title)); }
    }

    /// <summary>Appelé par le scan (ou la saisie manuelle) à l'étape 1.</summary>
    public void OnCodeScanned(string code)
    {
        if (Step != 1) return;
        Step = 2;
        RefreshLine();
    }

    void RefreshLine()
    {
        OnPropertyChanged(nameof(CurrentLine)); OnPropertyChanged(nameof(ContinueText)); OnPropertyChanged(nameof(RangePrompt));
        OnPropertyChanged(nameof(NextExpected)); OnPropertyChanged(nameof(RangeText));
    }

    [RelayCommand] void SimulateScan() => OnCodeScanned("DEMO");

    [RelayCommand]
    void Continue()
    {
        if (Step == 2)
        {
            foreach (var s in Suggestions) s.IsSelected = s.Code == CurrentLine.DefaultLocation || (s.IsSuggested && !Suggestions.Any(x => x.Code == CurrentLine.DefaultLocation));
            if (!Suggestions.Any(s => s.IsSelected)) Suggestions[0].IsSelected = true;
            Step = 3; RefreshLine(); OnPropertyChanged(nameof(ContinueText));
        }
    }

    [RelayCommand]
    void SelectLocation(LocationSuggestion s)
    {
        foreach (var x in Suggestions) x.IsSelected = x == s;
        OnPropertyChanged(nameof(RangeText));
    }

    [RelayCommand]
    void Store()
    {
        CurrentLine.Location = Suggestions.First(s => s.IsSelected).Code;
        if (LineIndex < Lines.Count - 1) { LineIndex++; Step = 1; RefreshLine(); }
        else { Step = 4; OnPropertyChanged(nameof(RecapArticles)); OnPropertyChanged(nameof(RecapDelta)); OnPropertyChanged(nameof(HasAnyDelta)); }
    }

    [RelayCommand]
    async Task BackAsync()
    {
        if (Step == 1 && LineIndex == 0) { await Nav.Back(); return; }
        if (Step == 1) { LineIndex--; Step = 3; RefreshLine(); return; }
        Step--;
    }

    [RelayCommand]
    async Task ValidateAsync()
    {
        var delta = Uri.EscapeDataString(HasAnyDelta ? RecapDelta : "");
        await Nav.Go($"../{Routes.ReceptionDone}?id={ReceptionId}&lines={Lines.Count}&delta={delta}");
    }
}

// ---------- 22 Confirmation ----------
public partial class ReceptionDoneViewModel : ObservableObject, IQueryAttributable
{
    public SyncService Sync { get; }
    public SessionService Session { get; }
    [ObservableProperty] string _id = "BR-0231";
    [ObservableProperty] int _lines = 3;
    [ObservableProperty] string _delta = "";

    public bool HasDelta => !string.IsNullOrEmpty(Delta);
    public string Summary => $"{Id} · {Lines} articles rangés";
    public string Stamp => $"le {DateTime.Now:dd/MM/yyyy} à {DateTime.Now:HH:mm} par {Session.UserName}";
    public string SyncKind => Sync.IsOnline ? "success" : "warning";
    public string SyncText => Sync.IsOnline ? $"Envoyée au serveur à {DateTime.Now:HH:mm}" : "Enregistrée hors ligne · envoi automatique";
    public string SyncIcon => Sync.IsOnline ? "icon_cloud_check" : "icon_wifi_off";
    public string DeltaText => $"Écart de {Delta} transmis au gérant";

    public ReceptionDoneViewModel(SyncService sync, SessionService session) { Sync = sync; Session = session; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var i)) Id = (string)i;
        if (query.TryGetValue("lines", out var l) && int.TryParse(l?.ToString(), out var n)) Lines = n;
        if (query.TryGetValue("delta", out var d)) Delta = Uri.UnescapeDataString(d?.ToString() ?? "");
        OnPropertyChanged(nameof(HasDelta)); OnPropertyChanged(nameof(Summary)); OnPropertyChanged(nameof(DeltaText));
    }

    [RelayCommand] Task NewReception() => Nav.Go(Routes.Reception);
    [RelayCommand] Task Home() => Nav.Go(Routes.Dashboard);
}
