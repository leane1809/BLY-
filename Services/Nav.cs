namespace BLY.Services;

/// <summary>Routes Shell (voir AppShell.xaml.cs et l'arborescence de navigation).</summary>
public static class Routes
{
    // Pages racines
    public const string Splash = "//splash", Onboarding = "//onboarding", Login = "//login", Warehouse = "//warehouse";
    // Onglets
    public const string Dashboard = "//main/dashboard", Zones = "//main/zones", Inventory = "//main/inventory", History = "//main/history";
    // Pages empilées
    public const string Register = "register", ForgotPin = "forgotpin", Scan = "scan", Settings = "settings", Sync = "sync",
        LocationDetail = "locationdetail", QrLabel = "qrlabel", LocationCreate = "locationcreate",
        Receptions = "receptions", Reception = "reception", ReceptionDone = "receptiondone",
        IssueNotes = "issuenotes", Picking = "picking", IssueValidation = "issuevalidation",
        Locate = "locate", Count = "count", CountVariances = "countvariances", CountValidation = "countvalidation",
        MovementDetail = "movementdetail";
}

public static class Nav
{
    public static Task Go(string route) => Shell.Current.GoToAsync(route);
    public static Task Go(string route, IDictionary<string, object> parameters) => Shell.Current.GoToAsync(route, parameters);
    public static Task Back() => Shell.Current.GoToAsync("..");
    public static Task Alert(string title, string message) => Shell.Current.DisplayAlert(title, message, "OK");
}
