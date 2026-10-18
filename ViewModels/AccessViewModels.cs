using BLY.Models;
using BLY.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BLY.ViewModels;

public static class Validators
{
    public static string Digits(string s) => new((s ?? "").Where(char.IsDigit).ToArray());
    /// <summary>Numéro camerounais : 9 chiffres après +237.</summary>
    public static bool IsPhone(string s)
    {
        var d = Digits(s);
        if (d.StartsWith("237")) d = d[3..];
        return d.Length == 9;
    }
}

// ---------- 01 Splash ----------
public partial class SplashViewModel : ObservableObject
{
    [ObservableProperty] double _progress;

    [RelayCommand]
    async Task StartAsync()
    {
        for (var i = 1; i <= 10; i++) { Progress = i / 10d; await Task.Delay(150); }
        await Nav.Go(Preferences.Get("onboarded", false) ? Routes.Login : Routes.Onboarding);
    }
}

// ---------- 02 Onboarding ----------
public record OnboardingSlide(string Icon, string Title, string Text);

public partial class OnboardingViewModel : ObservableObject
{
    public List<OnboardingSlide> Slides { get; } = new()
    {
        new("icon_scan_line", "Scannez, c'est enregistré", "Un code-barres ou un QR d'emplacement suffit : plus de cahier, plus d'oubli. Fonctionne même sans réseau."),
        new("icon_layout_grid", "Chaque article à sa place", "Zones, allées, rayons : retrouvez un article en quelques secondes grâce à son emplacement."),
        new("icon_cloud_check", "Tout est tracé", "Réceptions, sorties et inventaires sont synchronisés dès que le réseau revient, avec l'historique par utilisateur."),
    };

    [ObservableProperty, NotifyPropertyChangedFor(nameof(NextText))] int _position;
    public string NextText => Position == Slides.Count - 1 ? "Commencer" : "Suivant";

    [RelayCommand]
    async Task NextAsync()
    {
        if (Position < Slides.Count - 1) Position++;
        else await SkipAsync();
    }

    [RelayCommand]
    async Task SkipAsync()
    {
        Preferences.Set("onboarded", true);
        await Nav.Go(Routes.Login);
    }
}

// ---------- 03/04 Connexion ----------
public partial class LoginViewModel : ObservableObject
{
    readonly SessionService _session;
    int _attemptsLeft = 3;

    [ObservableProperty] string _phone = "+237 ";
    [ObservableProperty] string _pin = "";
    [ObservableProperty] string _phoneError = "";
    [ObservableProperty] string _pinError = "";
    [ObservableProperty] bool _isBusy;

    public LoginViewModel(SessionService session) => _session = session;

    partial void OnPhoneChanged(string value) => PhoneError = "";
    partial void OnPinChanged(string value) => PinError = "";

    [RelayCommand]
    async Task LoginAsync()
    {
        PhoneError = Validators.IsPhone(Phone) ? "" : "Numéro invalide : 9 chiffres après +237";
        PinError = Pin?.Length == 4 ? "" : "Le code PIN comporte 4 chiffres";
        if (PhoneError != "" || PinError != "") return;

        IsBusy = true;
        await Task.Delay(500); // TODO : appel d'authentification (ou vérification locale hors ligne)
        IsBusy = false;

        if (Pin != "1234") // code de démonstration
        {
            _attemptsLeft = Math.Max(0, _attemptsLeft - 1);
            PinError = _attemptsLeft > 0 ? $"Code PIN incorrect · {_attemptsLeft} essais restants" : "Trop d'essais. Utilisez « Code PIN oublié ? »";
            return;
        }
        _session.SignIn(Phone);
        await Nav.Go(Routes.Warehouse);
    }

    [RelayCommand] Task RegisterAsync() => Nav.Go(Routes.Register);
    [RelayCommand] Task ForgotAsync() => Nav.Go(Routes.ForgotPin);
}

// ---------- 05/06 Inscription ----------
public partial class RegisterViewModel : ObservableObject
{
    readonly SessionService _session;

    [ObservableProperty] string _fullName = "";
    [ObservableProperty] string _phone = "+237 ";
    [ObservableProperty] string _company = "Quincaillerie Mbarga & Fils — Bonabéri";
    [ObservableProperty] string _pin = "";
    [ObservableProperty] string _pinConfirm = "";
    [ObservableProperty] bool _acceptTerms;
    [ObservableProperty] bool _isManager = true;
    [ObservableProperty] bool _isStorekeeper;

    [ObservableProperty, NotifyPropertyChangedFor(nameof(ErrorCount), nameof(HasSummary), nameof(Summary), nameof(CanSubmit))] string _fullNameError = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(ErrorCount), nameof(HasSummary), nameof(Summary), nameof(CanSubmit))] string _phoneError = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(ErrorCount), nameof(HasSummary), nameof(Summary), nameof(CanSubmit))] string _pinConfirmError = "";

    public RegisterViewModel(SessionService session) => _session = session;

    public int ErrorCount => new[] { FullNameError, PhoneError, PinConfirmError }.Count(e => !string.IsNullOrEmpty(e));
    public bool HasSummary => ErrorCount > 0;
    public string Summary => $"{ErrorCount} champ{(ErrorCount > 1 ? "s" : "")} à corriger avant de continuer";
    public bool CanSubmit => AcceptTerms && ErrorCount == 0;
    public bool PhoneValid => Validators.IsPhone(Phone);

    partial void OnFullNameChanged(string value) => FullNameError = "";
    partial void OnPhoneChanged(string value) { PhoneError = ""; OnPropertyChanged(nameof(PhoneValid)); }
    partial void OnPinChanged(string value) => PinConfirmError = "";
    partial void OnPinConfirmChanged(string value) => PinConfirmError = "";
    partial void OnAcceptTermsChanged(bool value) => OnPropertyChanged(nameof(CanSubmit));
    partial void OnIsManagerChanged(bool value) { if (value) IsStorekeeper = false; }
    partial void OnIsStorekeeperChanged(bool value) { if (value) IsManager = false; }

    [RelayCommand]
    async Task CreateAsync()
    {
        FullNameError = string.IsNullOrWhiteSpace(FullName) ? "Champ obligatoire" : "";
        PhoneError = !Validators.IsPhone(Phone) ? "Numéro invalide : 9 chiffres après +237" : "";
        PinConfirmError = Pin?.Length != 4 ? "4 chiffres requis" : Pin != PinConfirm ? "Ne correspond pas" : "";
        if (ErrorCount > 0) return;

        _session.UserName = FullName.Trim();
        _session.SignIn(Phone, IsManager ? UserRole.Gerant : UserRole.Magasinier);
        await Nav.Go(Routes.Warehouse);
    }
}

// ---------- 07 Code PIN oublié ----------
public partial class ForgotPinViewModel : ObservableObject
{
    [ObservableProperty] string _phone = "+237 ";
    [ObservableProperty] string _phoneError = "";
    partial void OnPhoneChanged(string value) => PhoneError = "";

    [RelayCommand]
    async Task SendCodeAsync()
    {
        if (!Validators.IsPhone(Phone)) { PhoneError = "Numéro invalide : 9 chiffres après +237"; return; }
        // TODO : demander l'envoi du code SMS à 6 chiffres (expire au bout de 10 minutes)
        await Nav.Alert("Code envoyé", "Un code à 6 chiffres vient d'être envoyé par SMS.");
        await Nav.Back();
    }
}

// ---------- 08 Sélection de l'entrepôt ----------
public partial class WarehouseViewModel : ObservableObject
{
    readonly SessionService _session;
    public List<WarehouseOption> Warehouses { get; } = DemoData.Warehouses();

    public string Greeting => $"Bonjour {_session.FirstName},";
    public string ContinueText => $"Continuer vers {Warehouses.First(w => w.IsSelected).Name}";

    public WarehouseViewModel(SessionService session) => _session = session;

    [RelayCommand]
    void Select(WarehouseOption option)
    {
        foreach (var w in Warehouses) w.IsSelected = w == option;
        OnPropertyChanged(nameof(ContinueText));
    }

    [RelayCommand]
    async Task ContinueAsync()
    {
        var w = Warehouses.First(x => x.IsSelected);
        _session.WarehouseName = w.Name;
        _session.WarehouseCity = w.Detail.Split('·')[0].Trim();
        await Nav.Go(Routes.Dashboard);
    }
}
