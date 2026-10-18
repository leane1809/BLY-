using BLY.ViewModels;

namespace BLY.Views;

public partial class WarehousePage : ContentPage
{
    public WarehousePage(WarehouseViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
