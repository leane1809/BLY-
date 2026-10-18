using BLY.ViewModels;

namespace BLY.Views;

public partial class IssueNotesPage : ContentPage
{
    public IssueNotesPage(IssueNotesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
