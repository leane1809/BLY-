using BLY.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BLY.Services;

/// <summary>Utilisateur connecté, rôle et entrepôt actif. Le rôle pilote l'affichage « Réservé au gérant ».</summary>
public partial class SessionService : ObservableObject
{
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FirstName), nameof(Initials))] string _userName = "Jean Mbarga";
    [ObservableProperty] string _phone = "+237 6 77 12 34 56";
    [ObservableProperty] string _warehouseName = "Dépôt Bonabéri";
    [ObservableProperty] string _warehouseCity = "Douala";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsManager), nameof(RoleLabel))] UserRole _role = UserRole.Gerant;

    public bool IsManager => Role == UserRole.Gerant;
    public string RoleLabel => IsManager ? "Gérant" : "Magasinier";
    public string FirstName => UserName.Split(' ')[0];
    public string Initials => string.Concat(UserName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(p => char.ToUpper(p[0])));

    public void SignIn(string phone, UserRole? role = null)
    {
        Phone = phone;
        if (role.HasValue) Role = role.Value;
    }

    public void SignOut() { }
}
