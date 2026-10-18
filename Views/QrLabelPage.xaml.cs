using BLY.ViewModels;

namespace BLY.Views;

public partial class QrLabelPage : ContentPage
{
    public QrLabelPage(QrLabelViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
