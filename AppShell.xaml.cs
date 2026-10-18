using BLY.Services;
using BLY.Views;

namespace BLY;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Pages empilées (les routes sont des constantes dans Services/Nav.cs)
        Routing.RegisterRoute(Routes.Register, typeof(RegisterPage));
        Routing.RegisterRoute(Routes.ForgotPin, typeof(ForgotPinPage));
        Routing.RegisterRoute(Routes.Scan, typeof(ScanPage));
        Routing.RegisterRoute(Routes.Settings, typeof(SettingsPage));
        Routing.RegisterRoute(Routes.Sync, typeof(SyncPage));
        Routing.RegisterRoute(Routes.LocationDetail, typeof(LocationDetailPage));
        Routing.RegisterRoute(Routes.QrLabel, typeof(QrLabelPage));
        Routing.RegisterRoute(Routes.LocationCreate, typeof(LocationCreatePage));
        Routing.RegisterRoute(Routes.Receptions, typeof(ReceptionsPage));
        Routing.RegisterRoute(Routes.Reception, typeof(ReceptionPage));
        Routing.RegisterRoute(Routes.ReceptionDone, typeof(ReceptionDonePage));
        Routing.RegisterRoute(Routes.IssueNotes, typeof(IssueNotesPage));
        Routing.RegisterRoute(Routes.Picking, typeof(PickingPage));
        Routing.RegisterRoute(Routes.IssueValidation, typeof(IssueValidationPage));
        Routing.RegisterRoute(Routes.Locate, typeof(LocatePage));
        Routing.RegisterRoute(Routes.Count, typeof(CountPage));
        Routing.RegisterRoute(Routes.CountVariances, typeof(CountVariancesPage));
        Routing.RegisterRoute(Routes.CountValidation, typeof(CountValidationPage));
        Routing.RegisterRoute(Routes.MovementDetail, typeof(MovementDetailPage));
    }
}
