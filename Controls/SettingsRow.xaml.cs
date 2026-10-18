using System.Windows.Input;

namespace BLY.Controls;

/// <summary>Ligne de paramètre : icône, titre, valeur, puis flèche (navigation) ou interrupteur.</summary>
public partial class SettingsRow : ContentView
{
    public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(string), typeof(SettingsRow), "icon_info");
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(SettingsRow), string.Empty);
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(string), typeof(SettingsRow), string.Empty,
        propertyChanged: (b, _, _) => ((SettingsRow)b).OnPropertyChanged(nameof(HasValue)));
    public static readonly BindableProperty ShowSwitchProperty = BindableProperty.Create(nameof(ShowSwitch), typeof(bool), typeof(SettingsRow), false,
        propertyChanged: (b, _, _) => ((SettingsRow)b).OnPropertyChanged(nameof(ShowChevron)));
    public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(SettingsRow), false, BindingMode.TwoWay);
    public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SettingsRow));

    public string Icon { get => (string)GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string Value { get => (string)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public bool ShowSwitch { get => (bool)GetValue(ShowSwitchProperty); set => SetValue(ShowSwitchProperty, value); }
    public bool IsToggled { get => (bool)GetValue(IsToggledProperty); set => SetValue(IsToggledProperty, value); }
    public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

    public bool HasValue => !string.IsNullOrEmpty(Value);
    public bool ShowChevron => !ShowSwitch;

    public SettingsRow() => InitializeComponent();

    void OnTapped(object sender, TappedEventArgs e)
    {
        if (ShowSwitch) IsToggled = !IsToggled;
        else if (Command?.CanExecute(null) == true) Command.Execute(null);
    }
}
