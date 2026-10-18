using Microsoft.Maui.Controls.Shapes;

namespace BLY.Controls;

/// <summary>Barre de progression 0..1 (occupation d'une zone, progression d'inventaire).</summary>
public class ProgressTrack : ContentView
{
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(double), typeof(ProgressTrack), 0d,
        propertyChanged: (b, _, _) => ((ProgressTrack)b).Update());
    public static readonly BindableProperty FillKeyProperty = BindableProperty.Create(nameof(FillKey), typeof(string), typeof(ProgressTrack), "BrushMarque",
        propertyChanged: (b, _, _) => ((ProgressTrack)b).Update());

    public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    /// <summary>Clé de ressource Brush : BrushMarque, BrushSucces, BrushAlerte, BrushErreur…</summary>
    public string FillKey { get => (string)GetValue(FillKeyProperty); set => SetValue(FillKeyProperty, value); }

    readonly Grid _grid = new() { HeightRequest = 8 };
    readonly Border _fill = new() { StrokeThickness = 0, StrokeShape = new RoundRectangle { CornerRadius = 4 } };

    public ProgressTrack()
    {
        var track = new Border { StrokeThickness = 0, StrokeShape = new RoundRectangle { CornerRadius = 4 }, HeightRequest = 8 };
        track.SetDynamicResource(VisualElement.BackgroundProperty, "BrushBordure");
        _grid.Add(track, 0, 0);
        _grid.Add(_fill, 0, 0);
        Content = _grid;
        Update();
    }

    void Update()
    {
        var v = Math.Clamp(Value, 0d, 1d);
        _grid.ColumnDefinitions.Clear();
        _grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(Math.Max(v, 0.0001), GridUnitType.Star)));
        _grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(Math.Max(1 - v, 0.0001), GridUnitType.Star)));
        Grid.SetColumnSpan(_grid.Children[0] as BindableObject, 2);
        Grid.SetColumn(_fill, 0);
        _fill.SetDynamicResource(VisualElement.BackgroundProperty, string.IsNullOrEmpty(FillKey) ? "BrushMarque" : FillKey);
    }
}
