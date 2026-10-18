using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;

namespace BLY.Controls;

/// <summary>Clavier numérique dédié (comptage aveugle) : grosses touches de 64 px, pas de clavier système.</summary>
public class Keypad : ContentView
{
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(int), typeof(Keypad), 0, BindingMode.TwoWay);
    public static readonly BindableProperty ConfirmCommandProperty = BindableProperty.Create(nameof(ConfirmCommand), typeof(ICommand), typeof(Keypad));

    public int Value { get => (int)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public ICommand ConfirmCommand { get => (ICommand)GetValue(ConfirmCommandProperty); set => SetValue(ConfirmCommandProperty, value); }

    public Keypad()
    {
        var grid = new Grid { ColumnSpacing = 8, RowSpacing = 8, Padding = new Thickness(12) };
        for (var i = 0; i < 3; i++) grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        for (var i = 0; i < 4; i++) grid.RowDefinitions.Add(new RowDefinition(new GridLength(64)));

        for (var d = 1; d <= 9; d++) { var digit = d; grid.Add(Key(digit.ToString(), () => Append(digit)), (d - 1) % 3, (d - 1) / 3); }
        grid.Add(IconKey("icon_delete", false, () => Value /= 10), 0, 3);
        grid.Add(Key("0", () => Append(0)), 1, 3);
        grid.Add(IconKey("icon_check", true, () => { if (ConfirmCommand?.CanExecute(Value) == true) ConfirmCommand.Execute(Value); }), 2, 3);
        grid.SetDynamicResource(BackgroundColorProperty, "ColorFond");
        Content = grid;
    }

    void Append(int digit) { if (Value < 100000) Value = Value * 10 + digit; }

    static Border Tile(bool primary, Action tapped)
    {
        var b = new Border { StrokeThickness = 1, StrokeShape = new RoundRectangle { CornerRadius = 12 } };
        b.SetDynamicResource(VisualElement.BackgroundProperty, primary ? "BrushMarque" : "BrushSurface");
        b.SetDynamicResource(Border.StrokeProperty, primary ? "BrushMarque" : "BrushBordure");
        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) => tapped();
        b.GestureRecognizers.Add(tap);
        return b;
    }

    static Border Key(string text, Action tapped)
    {
        var b = Tile(false, tapped);
        var l = new Label { Text = text, FontFamily = "MonoBold", FontSize = 28, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
        l.SetDynamicResource(Label.TextColorProperty, "ColorTexte");
        b.Content = l;
        SemanticProperties.SetDescription(b, text);
        return b;
    }

    static Border IconKey(string icon, bool primary, Action tapped)
    {
        var b = Tile(primary, tapped);
        var i = new BlyIcon { Source = icon, WidthRequest = 28, HeightRequest = 28 };
        i.SetDynamicResource(BlyIcon.TintProperty, primary ? "ColorSurMarque" : "ColorTexte");
        b.Content = i;
        SemanticProperties.SetDescription(b, primary ? "Valider" : "Effacer");
        return b;
    }
}
