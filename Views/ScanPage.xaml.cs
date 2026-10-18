using BLY.ViewModels;

namespace BLY.Views;

public partial class ScanPage : ContentPage
{
    public ScanPage(ScanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        // Le Scanner central ouvre l'écran adapté au code lu (voir ScanViewModel.HandleCodeAsync)
        Scanner.CodeDetected += async (_, code) => await viewModel.HandleCodeAsync(code);
    }

    protected override void OnAppearing() { base.OnAppearing(); Scanner.IsActive = true; }
    protected override void OnDisappearing() { base.OnDisappearing(); Scanner.IsActive = false; }
}
