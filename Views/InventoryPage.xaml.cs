using BLY.ViewModels;

namespace BLY.Views;

public partial class InventoryPage : ContentPage
{
    public InventoryPage(InventoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
