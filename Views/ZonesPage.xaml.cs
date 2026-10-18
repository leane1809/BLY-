using BLY.ViewModels;

namespace BLY.Views;

public partial class ZonesPage : ContentPage
{
    public ZonesPage(ZonesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
