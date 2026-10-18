using BLY.ViewModels;

namespace BLY.Views;

public partial class ReceptionsPage : ContentPage
{
    public ReceptionsPage(ReceptionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
