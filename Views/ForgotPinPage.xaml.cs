using BLY.ViewModels;

namespace BLY.Views;

public partial class ForgotPinPage : ContentPage
{
    public ForgotPinPage(ForgotPinViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
