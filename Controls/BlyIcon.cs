using CommunityToolkit.Maui.Behaviors;

namespace BLY.Controls;

/// <summary>Icône vectorielle (SVG monochrome) teintée par le thème : un seul jeu d'icônes pour toute l'app.</summary>
public class BlyIcon : Image
{
    public static readonly BindableProperty TintProperty = BindableProperty.Create(nameof(Tint), typeof(Color), typeof(BlyIcon), null,
        propertyChanged: (b, _, n) => ((BlyIcon)b)._tint.TintColor = n as Color);

    readonly IconTintColorBehavior _tint = new();

    public Color Tint { get => (Color)GetValue(TintProperty); set => SetValue(TintProperty, value); }

    public BlyIcon()
    {
        Behaviors.Add(_tint);
        Aspect = Aspect.AspectFit;
        WidthRequest = 24;
        HeightRequest = 24;
        HorizontalOptions = LayoutOptions.Center;
        VerticalOptions = LayoutOptions.Center;
        SetDynamicResource(TintProperty, "ColorTexte");
    }
}
