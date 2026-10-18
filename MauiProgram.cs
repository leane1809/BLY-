using BLY.Services;
using BLY.ViewModels;
using BLY.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;

namespace BLY;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("AtkinsonHyperlegibleNext-Regular.ttf", "Atkinson");
                fonts.AddFont("AtkinsonHyperlegibleNext-Medium.ttf", "AtkinsonMedium");
                fonts.AddFont("AtkinsonHyperlegibleNext-SemiBold.ttf", "AtkinsonSemiBold");
                fonts.AddFont("AtkinsonHyperlegibleNext-Bold.ttf", "AtkinsonBold");
                fonts.AddFont("JetBrainsMono-Regular.ttf", "Mono");
                fonts.AddFont("JetBrainsMono-Medium.ttf", "MonoMedium");
                fonts.AddFont("JetBrainsMono-Bold.ttf", "MonoBold");
            });

        // Services (singletons : état partagé de l'application)
        builder.Services.AddSingleton<SessionService>();
        builder.Services.AddSingleton<SyncService>();

        // Pages + ViewModels (transient : un état neuf à chaque ouverture)
        AddPage<SplashPage, SplashViewModel>(builder.Services);
        AddPage<OnboardingPage, OnboardingViewModel>(builder.Services);
        AddPage<LoginPage, LoginViewModel>(builder.Services);
        AddPage<RegisterPage, RegisterViewModel>(builder.Services);
        AddPage<ForgotPinPage, ForgotPinViewModel>(builder.Services);
        AddPage<WarehousePage, WarehouseViewModel>(builder.Services);
        AddPage<DashboardPage, DashboardViewModel>(builder.Services);
        AddPage<ZonesPage, ZonesViewModel>(builder.Services);
        AddPage<LocationDetailPage, LocationDetailViewModel>(builder.Services);
        AddPage<QrLabelPage, QrLabelViewModel>(builder.Services);
        AddPage<LocationCreatePage, LocationCreateViewModel>(builder.Services);
        AddPage<ReceptionsPage, ReceptionsViewModel>(builder.Services);
        AddPage<ReceptionPage, ReceptionViewModel>(builder.Services);
        AddPage<ReceptionDonePage, ReceptionDoneViewModel>(builder.Services);
        AddPage<IssueNotesPage, IssueNotesViewModel>(builder.Services);
        AddPage<PickingPage, PickingViewModel>(builder.Services);
        AddPage<IssueValidationPage, IssueValidationViewModel>(builder.Services);
        AddPage<LocatePage, LocateViewModel>(builder.Services);
        AddPage<InventoryPage, InventoryViewModel>(builder.Services);
        AddPage<CountPage, CountViewModel>(builder.Services);
        AddPage<CountVariancesPage, CountVariancesViewModel>(builder.Services);
        AddPage<CountValidationPage, CountValidationViewModel>(builder.Services);
        AddPage<HistoryPage, HistoryViewModel>(builder.Services);
        AddPage<MovementDetailPage, MovementDetailViewModel>(builder.Services);
        AddPage<SettingsPage, SettingsViewModel>(builder.Services);
        AddPage<ScanPage, ScanViewModel>(builder.Services);
        AddPage<SyncPage, SyncViewModel>(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }

    static void AddPage<TPage, TViewModel>(IServiceCollection services)
        where TPage : class where TViewModel : class
    {
        services.AddTransient<TViewModel>();
        services.AddTransient<TPage>();
    }
}
