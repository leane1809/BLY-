using BLY.ViewModels;

namespace BLY.Views;

public partial class LocationCreatePage : ContentPage
{
    public LocationCreatePage(LocationCreateViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
