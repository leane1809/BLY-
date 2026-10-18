using BLY.ViewModels;

namespace BLY.Views;

public partial class PickingPage : ContentPage
{
    public PickingPage(PickingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
