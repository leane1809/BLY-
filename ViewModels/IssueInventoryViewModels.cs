using System.Collections.ObjectModel;
using BLY.Models;
using BLY.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace BLY.ViewModels;

// ---------- 23 Bons de sortie — liste ----------
public partial class IssueNotesViewModel : ObservableObject
{
    public List<IssueNote> Notes { get; } = DemoData.IssueNotes();
    [ObservableProperty] int _tab;
    public bool IsToPrepare => Tab == 0;
    public bool IsInProgress => Tab == 1;
    public bool IsValidated => Tab == 2;
    public ObservableCollection<IssueNote> Items { get; } = new();
    public bool IsEmpty => Items.Count == 0;

    public IssueNotesViewModel() => Load();
    partial void OnTabChanged(int value)
    {
        OnPropertyChanged(nameof(IsToPrepare)); OnPropertyChanged(nameof(IsInProgress)); OnPropertyChanged(nameof(IsValidated));
        Load();
    }
    void Load()
    {
        Items.Clear();
        foreach (var n in Notes.Where(n => Tab switch { 0 => n.Status is "En attente", 1 => n.Status is "En préparation", _ => n.Status is "Expédié" }))
            Items.Add(n);
        // « À préparer » regroupe aussi les bons en préparation dans la démo
        if (Tab == 0) foreach (var n in Notes.Where(n => n.Status == "En préparation")) Items.Insert(0, n);
        OnPropertyChanged(nameof(IsEmpty));
    }

    [RelayCommand] void SetTab(string index) => Tab = int.Parse(index);
    [RelayCommand] Task Open(IssueNote n) => Nav.Go($"{Routes.Picking}?id={n.Id}");
    [RelayCommand] Task NewNote() => Nav.Alert("Nouveau bon de sortie", "Création d'un bon (écran à brancher).");
}

// ---------- 24 / 25 Préparation guidée ----------
public partial class PickingViewModel : ObservableObject, IQueryAttributable, IRecipient<ScanResultMessage>
{
    [ObservableProperty] string _noteId = "BS-0412";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsScanStep), nameof(IsValidateStep))] int _step = 2;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(Remaining), nameof(Subtitle))] int _quantity = 36;
    [ObservableProperty] int _lineNumber = 3;
    [ObservableProperty] bool _showShortage;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(Remaining))] int _stock = 120;
    [ObservableProperty] string _location = "A-03-B-2";
    [ObservableProperty] string _scanMessage = "";

    public string Title => $"{NoteId} · Ngo Bayiha";
    public string Subtitle => $"Ligne {LineNumber} sur 5 · préparation";
    public bool IsScanStep => Step == 2;
    public bool IsValidateStep => Step == 3;
    public string Remaining => $"Restera {Stock - Quantity} sacs dans {Location} après prélèvement.";
    public string ShortageText => $"Il manque 6 sacs à l'emplacement {Location}. Choisissez comment continuer.";
    public string TakeFromText => "Prendre dans A-04-A-1";

    public PickingViewModel() => WeakReferenceMessenger.Default.Register(this);

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var v) && v is string s) { NoteId = s; OnPropertyChanged(nameof(Title)); }
    }

    public void Receive(ScanResultMessage m)
    {
        if (m.Target != "picking") return;
        ScanMessage = m.Value == Location ? $"Emplacement {Location} confirmé" : $"Code lu : {m.Value}";
        Step = 3;
    }

    [RelayCommand] Task Scan() => Nav.Go($"{Routes.Scan}?mode=return&target=picking");
    [RelayCommand] void Shortage() => ShowShortage = true;
    [RelayCommand] void CloseShortage() => ShowShortage = false;
    [RelayCommand] void TakeElsewhere() { Location = "A-04-A-1"; Stock = 480; ShowShortage = false; OnPropertyChanged(nameof(Remaining)); }
    [RelayCommand] void Partial() { Quantity = Math.Max(0, Quantity - 6); ShowShortage = false; }

    [RelayCommand]
    async Task ValidateLineAsync()
    {
        if (LineNumber < 5) { LineNumber++; Step = 2; ScanMessage = ""; OnPropertyChanged(nameof(Subtitle)); }
        else await Nav.Go($"../{Routes.IssueValidation}?id={NoteId}");
    }
}

// ---------- 26 Validation du gérant ----------
public partial class IssueValidationViewModel : ObservableObject, IQueryAttributable
{
    public SessionService Session { get; }
    public List<IssueLine> Lines { get; } = DemoData.IssueLines();
    [ObservableProperty] string _noteId = "BS-0412";
    public string Title => $"{NoteId} · Validation";
    public string Subtitle => $"Préparé par A. Fotso · {DateTime.Now:dd/MM/yyyy HH:mm}";
    public string Customer => "Ngo Bayiha · Bassa";
    public string TotalValue => "1 250 000 FCFA";

    public IssueValidationViewModel(SessionService session) => Session = session;
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var v) && v is string s) { NoteId = s; OnPropertyChanged(nameof(Title)); }
    }

    [RelayCommand]
    async Task RefuseAsync()
    {
        if (await Shell.Current.DisplayAlert("Refuser la sortie", $"Refuser le bon {NoteId} ?", "Refuser", "Annuler"))
            await Nav.Go(Routes.Dashboard);
    }

    [RelayCommand]
    async Task ValidateAsync()
    {
        await Nav.Alert("Sortie validée", $"Le bon {NoteId} est validé.");
        await Nav.Go(Routes.Dashboard);
    }
}

// ---------- 27 / 28 Localisation ----------
public partial class LocateViewModel : ObservableObject, IQueryAttributable, IRecipient<ScanResultMessage>
{
    [ObservableProperty] string _query = "";
    public ObservableCollection<StockLocation> Results { get; } = new();
    [ObservableProperty] bool _hasResult;
    public bool ShowHint => string.IsNullOrWhiteSpace(Query);
    public bool NoResult => !string.IsNullOrWhiteSpace(Query) && !HasResult;
    public string TotalText => "1 240";
    public string Spread => $"Réparti sur {Results.Count} emplacements";

    public LocateViewModel() => WeakReferenceMessenger.Default.Register(this);

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("query", out var q) && q is string s) Query = Uri.UnescapeDataString(s);
    }
    public void Receive(ScanResultMessage m) { if (m.Target == "locate") Query = m.Value; }

    partial void OnQueryChanged(string value)
    {
        Results.Clear();
        // TODO : recherche sur l'appareil dans SQLite. Démo : « ciment » ou « 42 » retrouve le ciment.
        HasResult = !string.IsNullOrWhiteSpace(value) &&
                    (value.Contains("ciment", StringComparison.OrdinalIgnoreCase) || value.Contains("42") || value.Contains("CIM", StringComparison.OrdinalIgnoreCase));
        if (HasResult) foreach (var l in DemoData.Locate()) Results.Add(l);
        OnPropertyChanged(nameof(HasResult)); OnPropertyChanged(nameof(NoResult)); OnPropertyChanged(nameof(ShowHint)); OnPropertyChanged(nameof(Spread));
    }

    [RelayCommand] Task Scan() => Nav.Go($"{Routes.Scan}?mode=return&target=locate");
    [RelayCommand] void Clear() => Query = "";
    [RelayCommand] Task OpenLocation(StockLocation l) => Nav.Go($"{Routes.LocationDetail}?code={l.Code}");
}

// ---------- 29 Inventaire tournant — planning ----------
public partial class InventoryViewModel : ObservableObject
{
    public List<CountTask> Tasks { get; } = DemoData.CountToday();
    public Movement Done { get; } = new("M-10480", "Eau minérale 1,5 L", "Compté à 09:05 · R. Tchoumi", "−3", "packs", MovementKind.Adjust);
    public List<Movement> DoneList => new() { Done };
    public double Progress => 4d / 12d;
    public string ProgressText => "Progression du jour : 4 sur 12 emplacements";
    public string NextCode => Tasks[0].Code;
    public string CountButton => $"Compter {NextCode}";
    public string Subtitle => "Semaine 41 · 18 emplacements cette semaine";

    [RelayCommand] Task Count(CountTask t) => Nav.Go($"{Routes.Count}?code={t.Code}");
    [RelayCommand] Task CountNext() => Nav.Go($"{Routes.Count}?code={NextCode}");
    [RelayCommand] Task OpenVariances() => Nav.Go(Routes.CountValidation);
}

// ---------- 30 Comptage aveugle ----------
public partial class CountViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty] string _code = "B-02-C-3";
    [ObservableProperty] int _count;
    public string Title => $"Comptage {Code}";
    public string Subtitle => "Zone B · Allée 02 · Rayon C · Niveau 3";
    public string Article => "Tôle bac 0,40 mm — 2 m";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("code", out var c) && c is string s) { Code = s; OnPropertyChanged(nameof(Title)); }
    }

    [RelayCommand] Task Confirm() => Nav.Go($"{Routes.CountVariances}?code={Code}&count={Count}");
}

// ---------- 31 Écarts et justification ----------
public partial class CountVariancesViewModel : ObservableObject
{
    public SessionService Session { get; }
    public List<VarianceItem> Items { get; } = DemoData.Variances();
    public List<string> Reasons { get; } = DemoData.VarianceReasons.ToList();
    public string Subtitle => $"{Items.Count} emplacements · {Session.UserName}";

    public CountVariancesViewModel(SessionService session) => Session = session;

    [RelayCommand]
    async Task SendAsync()
    {
        if (Items.Any(i => i.HasDelta && string.IsNullOrEmpty(i.Reason)))
        { await Nav.Alert("Motif manquant", "Choisissez un motif pour chaque écart avant d'envoyer au gérant."); return; }
        await Nav.Alert("Envoyé", "Les écarts sont transmis au gérant pour validation.");
        await Nav.Go(Routes.Inventory);
    }
}

// ---------- 32 Validation des écarts (gérant) ----------
public partial class CountValidationViewModel : ObservableObject
{
    public SessionService Session { get; }
    public List<VarianceItem> Items { get; } = DemoData.Variances().Where(v => v.HasDelta).ToList();
    public string Subtitle => $"{Items.Count} écarts · comptage du {DateTime.Now:dd/MM/yyyy}";
    public string Impact => "−45 536 FCFA";

    public CountValidationViewModel(SessionService session) => Session = session;

    [RelayCommand] Task Recount() => Nav.Go($"{Routes.Count}?code=B-02-C-3");
    [RelayCommand]
    async Task ValidateAsync()
    {
        await Nav.Alert("Écarts validés", "Stock ajusté et emplacements clôturés.");
        await Nav.Go(Routes.Inventory);
    }
}
