using BLY.ViewModels;

namespace BLY.Views;

public partial class ReceptionDonePage : ContentPage
{
    public ReceptionDonePage(ReceptionDoneViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
