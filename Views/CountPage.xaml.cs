using BLY.ViewModels;

namespace BLY.Views;

public partial class CountPage : ContentPage
{
    public CountPage(CountViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
