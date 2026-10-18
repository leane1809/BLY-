using BLY.ViewModels;

namespace BLY.Views;

public partial class ReceptionPage : ContentPage
{
    readonly ReceptionViewModel _vm;

    public ReceptionPage(ReceptionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _vm = viewModel;
        // Un code lu (caméra ou saisie manuelle) fait passer à l'étape « Contrôler »
        Scanner.CodeDetected += (_, code) => _vm.OnCodeScanned(code);
    }
}
