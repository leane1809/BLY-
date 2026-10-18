using BLY.ViewModels;

namespace BLY.Views;

public partial class LocatePage : ContentPage
{
    public LocatePage(LocateViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
