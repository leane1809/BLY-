namespace BLY.Controls;

/// <summary>
/// Propriétés attachées permettant de lier une couleur de THÈME depuis les données :
/// <c>c:Theme.Text="{Binding StatusKey}"</c> où StatusKey = "ColorErreur", etc.
/// Le DynamicResource suit automatiquement le mode Clair/Sombre.
/// </summary>
public static class Theme
{
    public static readonly BindableProperty TextProperty = BindableProperty.CreateAttached("Text", typeof(string), typeof(Theme), null,
        propertyChanged: (b, _, n) =>
        {
            if (b is Label l && n is string k && k.Length > 0) l.SetDynamicResource(Label.TextColorProperty, k);
            else if (b is Button bt && n is string k2 && k2.Length > 0) bt.SetDynamicResource(Button.TextColorProperty, k2);
        });
    public static string GetText(BindableObject b) => (string)b.GetValue(TextProperty);
    public static void SetText(BindableObject b, string v) => b.SetValue(TextProperty, v);

    public static readonly BindableProperty BackProperty = BindableProperty.CreateAttached("Back", typeof(string), typeof(Theme), null,
        propertyChanged: (b, _, n) => { if (b is VisualElement e && n is string k && k.Length > 0) e.SetDynamicResource(VisualElement.BackgroundProperty, k); });
    public static string GetBack(BindableObject b) => (string)b.GetValue(BackProperty);
    public static void SetBack(BindableObject b, string v) => b.SetValue(BackProperty, v);

    public static readonly BindableProperty StrokeProperty = BindableProperty.CreateAttached("Stroke", typeof(string), typeof(Theme), null,
        propertyChanged: (b, _, n) => { if (b is Border e && n is string k && k.Length > 0) e.SetDynamicResource(Border.StrokeProperty, k); });
    public static string GetStroke(BindableObject b) => (string)b.GetValue(StrokeProperty);
    public static void SetStroke(BindableObject b, string v) => b.SetValue(StrokeProperty, v);

    public static readonly BindableProperty TintProperty = BindableProperty.CreateAttached("Tint", typeof(string), typeof(Theme), null,
        propertyChanged: (b, _, n) => { if (b is BlyIcon e && n is string k && k.Length > 0) e.SetDynamicResource(BlyIcon.TintProperty, k); });
    public static string GetTint(BindableObject b) => (string)b.GetValue(TintProperty);
    public static void SetTint(BindableObject b, string v) => b.SetValue(TintProperty, v);
}
