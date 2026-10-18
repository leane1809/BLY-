using BLY.ViewModels;

namespace BLY.Views;

public partial class SyncPage : ContentPage
{
    public SyncPage(SyncViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
