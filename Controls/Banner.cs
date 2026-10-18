using Microsoft.Maui.Controls.Shapes;

namespace BLY.Controls;

/// <summary>
/// Bandeau ou pastille d'état. Jamais la couleur seule : toujours icône + mot.
/// Kind : info | success | warning | error | violet.
/// </summary>
public class Banner : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(Banner), string.Empty,
        propertyChanged: (b, _, n) => ((Banner)b)._label.Text = n as string);
    public static readonly BindableProperty KindProperty = BindableProperty.Create(nameof(Kind), typeof(string), typeof(Banner), "info",
        propertyChanged: (b, _, _) => ((Banner)b).Apply());
    public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(string), typeof(Banner), null,
        propertyChanged: (b, _, _) => ((Banner)b).Apply());
    public static readonly BindableProperty CompactProperty = BindableProperty.Create(nameof(Compact), typeof(bool), typeof(Banner), false,
        propertyChanged: (b, _, _) => ((Banner)b).Apply());

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public string Kind { get => (string)GetValue(KindProperty); set => SetValue(KindProperty, value); }
    public string Icon { get => (string)GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public bool Compact { get => (bool)GetValue(CompactProperty); set => SetValue(CompactProperty, value); }

    readonly Border _box = new() { StrokeThickness = 0 };
    readonly BlyIcon _icon = new() { WidthRequest = 18, HeightRequest = 18, VerticalOptions = LayoutOptions.Center };
    readonly Label _label = new() { FontFamily = "AtkinsonSemiBold", VerticalOptions = LayoutOptions.Center };

    public Banner()
    {
        var grid = new Grid { ColumnSpacing = 8, ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star) } };
        grid.Add(_icon, 0, 0);
        grid.Add(_label, 1, 0);
        _box.Content = grid;
        Content = _box;
        Apply();
    }

    void Apply()
    {
        var (name, defaultIcon) = (Kind ?? "info").ToLowerInvariant() switch
        {
            "success" => ("Succes", "icon_circle_check"),
            "warning" => ("Alerte", "icon_triangle_alert"),
            "error" => ("Erreur", "icon_circle_alert"),
            "violet" => ("Violet", "icon_truck"),
            _ => ("Info", "icon_info")
        };
        _box.SetDynamicResource(VisualElement.BackgroundProperty, $"Brush{name}Fond");
        _label.SetDynamicResource(Label.TextColorProperty, $"Color{name}");
        _icon.SetDynamicResource(BlyIcon.TintProperty, $"Color{name}");
        _icon.Source = string.IsNullOrEmpty(Icon) ? defaultIcon : Icon;
        _label.FontSize = Compact ? 13 : 14;
        _icon.WidthRequest = _icon.HeightRequest = Compact ? 16 : 20;
        _box.Padding = Compact ? new Thickness(8, 4) : new Thickness(12, 10);
        _box.StrokeShape = new RoundRectangle { CornerRadius = Compact ? 8 : 10 };
        _box.HorizontalOptions = Compact ? LayoutOptions.Start : LayoutOptions.Fill;
    }
}
