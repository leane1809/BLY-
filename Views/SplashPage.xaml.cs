using BLY.ViewModels;

namespace BLY.Views;

public partial class SplashPage : ContentPage
{
    readonly SplashViewModel _vm;

    public SplashPage(SplashViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _vm = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.StartCommand.Execute(null);
    }
}
