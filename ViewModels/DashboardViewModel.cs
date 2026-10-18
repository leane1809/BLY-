using System.Globalization;
using BLY.Models;
using BLY.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BLY.ViewModels;

// ---------- 09 / 10 / 11 / 12 / 40 Tableau de bord ----------
public partial class DashboardViewModel : ObservableObject
{
    public SessionService Session { get; }
    public SyncService Sync { get; }

    [ObservableProperty] bool _isLoading = true;
    [ObservableProperty] bool _sessionExpired;
    [ObservableProperty] string _bannerKind = "info";
    [ObservableProperty] string _bannerText = "";
    [ObservableProperty] string _bannerIcon = "icon_refresh_cw";

    public List<DashboardTile> Tiles { get; } = DemoData.Tiles();
    public List<Movement> Movements { get; } = DemoData.RecentMovements();

    public string Greeting => $"Bonjour, {Session.FirstName}";
    public string WarehouseLine => $"{Session.WarehouseName} · {Session.WarehouseCity}";
    public string DateLine
    {
        get
        {
            var d = DateTime.Now.ToString("dddd dd/MM/yyyy", CultureInfo.GetCultureInfo("fr-FR"));
            return $"{char.ToUpper(d[0])}{d[1..]} · {Session.WarehouseName}";
        }
    }

    public DashboardViewModel(SessionService session, SyncService sync)
    {
        Session = session;
        Sync = sync;
        Sync.PropertyChanged += (_, _) => MainThread.BeginInvokeOnMainThread(UpdateBanner);
        UpdateBanner();
    }

    void UpdateBanner()
    {
        if (IsLoading) { BannerKind = "info"; BannerIcon = "icon_refresh_cw"; BannerText = "Synchronisation en cours… 3 sur 12"; }
        else if (Sync.HasFailed) { BannerKind = "error"; BannerIcon = "icon_cloud_off"; BannerText = "Échec de synchro · appuyez pour voir"; }
        else if (Sync.IsOffline) { BannerKind = "warning"; BannerIcon = "icon_wifi_off"; BannerText = $"Hors ligne · {Sync.PendingCount} opérations en attente"; }
        else { BannerKind = "success"; BannerIcon = "icon_cloud_check"; BannerText = $"Synchronisé à {Sync.LastSyncText}"; }
    }

    [RelayCommand]
    async Task AppearAsync()
    {
        OnPropertyChanged(nameof(Greeting));
        OnPropertyChanged(nameof(WarehouseLine));
        OnPropertyChanged(nameof(DateLine));
        IsLoading = true; UpdateBanner();
        await Task.Delay(1100); // TODO : charger les indicateurs depuis la base locale (listes paginées par 20)
        IsLoading = false; UpdateBanner();
    }

    [RelayCommand] Task OpenReceptions() => Nav.Go(Routes.Receptions);
    [RelayCommand] Task OpenIssues() => Nav.Go(Routes.IssueNotes);
    [RelayCommand] Task OpenLocate() => Nav.Go(Routes.Locate);
    [RelayCommand] Task OpenCount() => Nav.Go(Routes.Inventory);
    [RelayCommand] Task OpenSettings() => Nav.Go(Routes.Settings);
    [RelayCommand] Task OpenSync() => Nav.Go(Routes.Sync);
    [RelayCommand] Task ChangeWarehouse() => Nav.Go(Routes.Warehouse);
    [RelayCommand] Task OpenMovement(Movement m) => Nav.Go($"{Routes.MovementDetail}?id={m.Id}");
    [RelayCommand] Task Relogin() { SessionExpired = false; return Nav.Go(Routes.Login); }
}
