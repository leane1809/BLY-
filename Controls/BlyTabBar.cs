using BLY.Services;

namespace BLY.Controls;

/// <summary>
/// Barre inférieure à 5 entrées : Accueil, Zones, Scanner (central, ambre), Inventaire, Historique.
/// Le Scanner est réservé à l'action « scanner » : il ouvre l'écran adapté au code lu.
/// </summary>
public class BlyTabBar : ContentView
{
    public static readonly BindableProperty SelectedProperty = BindableProperty.Create(nameof(Selected), typeof(string), typeof(BlyTabBar), "home",
        propertyChanged: (b, _, _) => ((BlyTabBar)b).Refresh());

    /// <summary>home | zones | inventory | history</summary>
    public string Selected { get => (string)GetValue(SelectedProperty); set => SetValue(SelectedProperty, value); }

    readonly List<(string Key, Border Pill, BlyIcon Icon, Label Label)> _tabs = new();

    public BlyTabBar()
    {
        var grid = new Grid
        {
            Padding = new Thickness(4, 8, 4, 8),
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star)
            }
        };
        grid.SetDynamicResource(BackgroundColorProperty, "ColorSurface");

        AddTab(grid, 0, "home", "icon_house", "Accueil", Routes.Dashboard);
        AddTab(grid, 1, "zones", "icon_layout_grid", "Zones", Routes.Zones);
        AddScanButton(grid, 2);
        AddTab(grid, 3, "inventory", "icon_clipboard_check", "Inventaire", Routes.Inventory);
        AddTab(grid, 4, "history", "icon_history", "Historique", Routes.History);

        var line = new BoxView { HeightRequest = 1, VerticalOptions = LayoutOptions.Start };
        line.SetDynamicResource(BoxView.ColorProperty, "ColorBordure");
        Content = new Grid { Children = { grid, line } };
        Refresh();
    }

    void AddTab(Grid grid, int column, string key, string iconName, string text, string route)
    {
        var icon = new BlyIcon { Source = iconName, WidthRequest = 24, HeightRequest = 24 };
        var pill = new Border
        {
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
            Padding = new Thickness(18, 5),
            HorizontalOptions = LayoutOptions.Center,
            Content = icon
        };
        var label = new Label { Text = text, FontSize = 12, HorizontalTextAlignment = TextAlignment.Center, FontFamily = "AtkinsonMedium" };
        var stack = new VerticalStackLayout { Spacing = 2, VerticalOptions = LayoutOptions.Center, Children = { pill, label } };
        SemanticProperties.SetDescription(stack, text);

        var tap = new TapGestureRecognizer();
        tap.Tapped += async (_, _) => { if (Selected != key) await Nav.Go(route); };
        stack.GestureRecognizers.Add(tap);

        grid.Add(stack, column, 0);
        _tabs.Add((key, pill, icon, label));
    }

    void AddScanButton(Grid grid, int column)
    {
        var icon = new BlyIcon { Source = "icon_scan_line", WidthRequest = 30, HeightRequest = 30 };
        icon.SetDynamicResource(BlyIcon.TintProperty, "ColorSurAccent");
        var button = new Border
        {
            WidthRequest = 64, HeightRequest = 64, StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 32 },
            Margin = new Thickness(6, -18, 6, 0),
            VerticalOptions = LayoutOptions.Start,
            Content = icon
        };
        button.SetDynamicResource(VisualElement.BackgroundProperty, "BrushAccent");
        SemanticProperties.SetDescription(button, "Scanner un code");
        var tap = new TapGestureRecognizer();
        tap.Tapped += async (_, _) => await Nav.Go(Routes.Scan);
        button.GestureRecognizers.Add(tap);
        grid.Add(button, column, 0);
    }

    void Refresh()
    {
        foreach (var (key, pill, icon, label) in _tabs)
        {
            var selected = key == Selected;
            var tint = selected ? "ColorMarque" : "ColorTexteSecondaire";
            icon.SetDynamicResource(BlyIcon.TintProperty, tint);
            label.SetDynamicResource(Label.TextColorProperty, tint);
            label.FontFamily = selected ? "AtkinsonBold" : "AtkinsonMedium";
            if (selected) pill.SetDynamicResource(VisualElement.BackgroundProperty, "BrushMarqueFond");
            else { pill.RemoveDynamicResource(VisualElement.BackgroundProperty); pill.Background = Brush.Transparent; }
        }
    }
}
