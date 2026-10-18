using BLY.ViewModels;

namespace BLY.Views;

public partial class MovementDetailPage : ContentPage
{
    public MovementDetailPage(MovementDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
