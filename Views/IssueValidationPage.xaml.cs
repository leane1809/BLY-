using BLY.ViewModels;

namespace BLY.Views;

public partial class IssueValidationPage : ContentPage
{
    public IssueValidationPage(IssueValidationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
