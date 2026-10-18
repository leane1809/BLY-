using BLY.ViewModels;

namespace BLY.Views;

public partial class DashboardPage : ContentPage
{
    readonly DashboardViewModel _vm;

    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _vm = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.AppearCommand.Execute(null);
    }
}
