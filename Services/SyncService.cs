using CommunityToolkit.Mvvm.ComponentModel;

namespace BLY.Services;

/// <summary>
/// État réseau + file d'attente hors ligne (maquettes 11 et 39).
/// Branchez ici la vraie synchronisation par lots compressés ; l'option « Wi-Fi uniquement » est lue ici.
/// </summary>
public partial class SyncService : ObservableObject
{
    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsOffline))] bool _isOnline;
    [ObservableProperty] int _pendingCount = 12;
    [ObservableProperty] bool _autoSync = true;
    [ObservableProperty] bool _wifiOnly;
    [ObservableProperty] bool _hasFailed;
    [ObservableProperty] DateTime _lastSync = DateTime.Now;

    public bool IsOffline => !IsOnline;
    public string LastSyncText => LastSync.ToString("HH:mm");

    public SyncService()
    {
        IsOnline = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        Connectivity.Current.ConnectivityChanged += (_, e) =>
            MainThread.BeginInvokeOnMainThread(() => IsOnline = e.NetworkAccess == NetworkAccess.Internet);
    }

    public async Task SyncNowAsync()
    {
        await Task.Delay(900); // TODO : envoi du lot d'opérations en attente
        HasFailed = false;
        PendingCount = 0;
        LastSync = DateTime.Now;
        OnPropertyChanged(nameof(LastSyncText));
    }
}
