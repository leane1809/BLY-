using Microsoft.Maui.Controls.Shapes;

namespace BLY.Controls;

/// <summary>Stepper en 4 étapes (Scanner · Contrôler · Ranger · Valider, etc.). Current est 1-based.</summary>
public class StepperBar : ContentView
{
    public static readonly BindableProperty StepsProperty = BindableProperty.Create(nameof(Steps), typeof(string), typeof(StepperBar), string.Empty,
        propertyChanged: (b, _, _) => ((StepperBar)b).Rebuild());
    public static readonly BindableProperty CurrentProperty = BindableProperty.Create(nameof(Current), typeof(int), typeof(StepperBar), 1,
        propertyChanged: (b, _, _) => ((StepperBar)b).Rebuild());

    /// <summary>Libellés séparés par « | », ex. "Scanner|Contrôler|Ranger|Valider".</summary>
    public string Steps { get => (string)GetValue(StepsProperty); set => SetValue(StepsProperty, value); }
    public int Current { get => (int)GetValue(CurrentProperty); set => SetValue(CurrentProperty, value); }

    void Rebuild()
    {
        var names = (Steps ?? string.Empty).Split('|', StringSplitOptions.RemoveEmptyEntries);
        var grid = new Grid { Padding = new Thickness(12, 12, 12, 0), ColumnSpacing = 6, RowSpacing = 6 };
        grid.SetDynamicResource(BackgroundColorProperty, "ColorSurface");
        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        for (var i = 0; i < names.Length; i++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            var n = i + 1;
            var done = n < Current;
            var current = n == Current;

            var circle = new Border
            {
                WidthRequest = 28, HeightRequest = 28, HorizontalOptions = LayoutOptions.Center,
                StrokeShape = new RoundRectangle { CornerRadius = 14 },
                StrokeThickness = done || current ? 0 : 1.5
            };
            if (done || current) circle.SetDynamicResource(VisualElement.BackgroundProperty, "BrushMarque");
            else circle.SetDynamicResource(Border.StrokeProperty, "BrushBordureForte");

            if (done)
            {
                var check = new BlyIcon { Source = "icon_check", WidthRequest = 16, HeightRequest = 16 };
                check.SetDynamicResource(BlyIcon.TintProperty, "ColorSurMarque");
                circle.Content = check;
            }
            else
            {
                var num = new Label
                {
                    Text = n.ToString(), FontSize = 14, FontFamily = "AtkinsonBold",
                    HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center
                };
                num.SetDynamicResource(Label.TextColorProperty, current ? "ColorSurMarque" : "ColorTexteDiscret");
                circle.Content = num;
            }

            var text = new Label
            {
                Text = names[i], FontSize = 13, HorizontalTextAlignment = TextAlignment.Center,
                FontFamily = current ? "AtkinsonBold" : "AtkinsonMedium", LineBreakMode = LineBreakMode.TailTruncation
            };
            text.SetDynamicResource(Label.TextColorProperty, current ? "ColorMarque" : "ColorTexteSecondaire");

            var column = new VerticalStackLayout { Spacing = 4, Children = { circle, text } };
            grid.Add(column, i, 0);

            var line = new BoxView { HeightRequest = 3, CornerRadius = 2 };
            line.SetDynamicResource(BoxView.ColorProperty, n <= Current ? "ColorMarque" : "ColorBordure");
            grid.Add(line, i, 1);
        }
        Content = grid;
    }
}
