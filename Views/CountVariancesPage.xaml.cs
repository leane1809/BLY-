using BLY.ViewModels;

namespace BLY.Views;

public partial class CountVariancesPage : ContentPage
{
    public CountVariancesPage(CountVariancesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
