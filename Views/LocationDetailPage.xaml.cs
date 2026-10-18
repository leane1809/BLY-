using BLY.ViewModels;

namespace BLY.Views;

public partial class LocationDetailPage : ContentPage
{
    public LocationDetailPage(LocationDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
