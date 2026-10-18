using BLY.ViewModels;

namespace BLY.Views;

public partial class CountValidationPage : ContentPage
{
    public CountValidationPage(CountValidationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
