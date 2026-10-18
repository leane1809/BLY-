using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;

namespace BLY.Controls;

/// <summary>Filtre « pastille ». Sélection = fond plein + coche (pas seulement la couleur).</summary>
public class Chip : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(Chip), string.Empty,
        propertyChanged: (b, _, n) => ((Chip)b)._label.Text = n as string);
    public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(Chip), false,
        BindingMode.TwoWay, propertyChanged: (b, _, _) => ((Chip)b).Apply());
    public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(Chip));
    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(Chip));

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public bool IsSelected { get => (bool)GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
    public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }
    public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

    readonly Border _box = new() { StrokeThickness = 1.5, HeightRequest = 44, StrokeShape = new RoundRectangle { CornerRadius = 22 }, Padding = new Thickness(14, 0) };
    readonly BlyIcon _check = new() { Source = "icon_check", WidthRequest = 18, HeightRequest = 18 };
    readonly Label _label = new() { FontFamily = "AtkinsonSemiBold", FontSize = 14, VerticalOptions = LayoutOptions.Center };

    public Chip()
    {
        var row = new HorizontalStackLayout { Spacing = 6, VerticalOptions = LayoutOptions.Center, Children = { _check, _label } };
        _box.Content = row;
        Content = _box;
        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) =>
        {
            if (Command?.CanExecute(CommandParameter) == true) Command.Execute(CommandParameter);
            else IsSelected = !IsSelected;
        };
        _box.GestureRecognizers.Add(tap);
        Apply();
    }

    void Apply()
    {
        _check.IsVisible = IsSelected;
        _box.SetDynamicResource(VisualElement.BackgroundProperty, IsSelected ? "BrushMarque" : "BrushSurface");
        _box.SetDynamicResource(Border.StrokeProperty, IsSelected ? "BrushMarque" : "BrushBordureForte");
        _label.SetDynamicResource(Label.TextColorProperty, IsSelected ? "ColorSurMarque" : "ColorTexte");
        _check.SetDynamicResource(BlyIcon.TintProperty, "ColorSurMarque");
    }
}
