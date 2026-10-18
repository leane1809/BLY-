namespace BLY.Controls;

/// <summary>Champ de saisie avec états : normal, focus, validé, erreur (icône + texte, jamais la couleur seule).</summary>
public partial class FieldEntry : ContentView
{
    public static readonly BindableProperty LabelProperty = BindableProperty.Create(nameof(Label), typeof(string), typeof(FieldEntry), string.Empty,
        propertyChanged: (b, _, _) => ((FieldEntry)b).OnPropertyChanged(nameof(HasLabel)));
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(FieldEntry), string.Empty, BindingMode.TwoWay);
    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(FieldEntry), string.Empty);
    public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(string), typeof(FieldEntry), string.Empty,
        propertyChanged: (b, _, _) => ((FieldEntry)b).OnPropertyChanged(nameof(HasIcon)));
    public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(FieldEntry), false,
        propertyChanged: (b, _, _) => { var f = (FieldEntry)b; f.OnPropertyChanged(nameof(IsHidden)); f.OnPropertyChanged(nameof(ShowToggle)); });
    public static readonly BindableProperty InputKeyboardProperty = BindableProperty.Create(nameof(InputKeyboard), typeof(Keyboard), typeof(FieldEntry), Keyboard.Default);
    public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create(nameof(MaxLength), typeof(int), typeof(FieldEntry), int.MaxValue);
    public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(FieldEntry), string.Empty,
        propertyChanged: (b, _, _) => ((FieldEntry)b).UpdateVisual());
    public static readonly BindableProperty HelperTextProperty = BindableProperty.Create(nameof(HelperText), typeof(string), typeof(FieldEntry), string.Empty,
        propertyChanged: (b, _, _) => ((FieldEntry)b).UpdateVisual());
    public static readonly BindableProperty IsValidProperty = BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(FieldEntry), false,
        propertyChanged: (b, _, _) => ((FieldEntry)b).UpdateVisual());

    public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public string Icon { get => (string)GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public bool IsPassword { get => (bool)GetValue(IsPasswordProperty); set => SetValue(IsPasswordProperty, value); }
    public Keyboard InputKeyboard { get => (Keyboard)GetValue(InputKeyboardProperty); set => SetValue(InputKeyboardProperty, value); }
    public int MaxLength { get => (int)GetValue(MaxLengthProperty); set => SetValue(MaxLengthProperty, value); }
    public string ErrorText { get => (string)GetValue(ErrorTextProperty); set => SetValue(ErrorTextProperty, value); }
    public string HelperText { get => (string)GetValue(HelperTextProperty); set => SetValue(HelperTextProperty, value); }
    public bool IsValid { get => (bool)GetValue(IsValidProperty); set => SetValue(IsValidProperty, value); }

    bool _revealed;
    bool _focused;

    public bool HasLabel => !string.IsNullOrEmpty(Label);
    public bool HasIcon => !string.IsNullOrEmpty(Icon);
    public bool HasError => !string.IsNullOrEmpty(ErrorText);
    public bool IsHidden => IsPassword && !_revealed;
    public bool ShowToggle => IsPassword;
    public string ToggleIcon => _revealed ? "icon_eye_off" : "icon_eye";
    public string FootnoteText => HasError ? ErrorText : HelperText;
    public bool HasFootnote => !string.IsNullOrEmpty(FootnoteText);

    public FieldEntry() => InitializeComponent();

    void OnToggle(object sender, TappedEventArgs e)
    {
        _revealed = !_revealed;
        OnPropertyChanged(nameof(IsHidden));
        OnPropertyChanged(nameof(ToggleIcon));
    }

    void OnFocused(object sender, FocusEventArgs e) { _focused = true; UpdateVisual(); }
    void OnUnfocused(object sender, FocusEventArgs e) { _focused = false; UpdateVisual(); }

    void UpdateVisual()
    {
        OnPropertyChanged(nameof(HasError));
        OnPropertyChanged(nameof(FootnoteText));
        OnPropertyChanged(nameof(HasFootnote));
        if (Box == null) return;

        var stroke = HasError ? "BrushErreurBord" : _focused ? "BrushMarque" : IsValid ? "BrushSuccesBord" : "BrushBordure";
        Box.SetDynamicResource(Border.StrokeProperty, stroke);
        Box.StrokeThickness = HasError || _focused || IsValid ? 2 : 1;

        var tint = HasError ? "ColorErreur" : IsValid ? "ColorSucces" : "ColorTexteSecondaire";
        Foot.SetDynamicResource(Microsoft.Maui.Controls.Label.TextColorProperty, tint);
        FootIcon.SetDynamicResource(BlyIcon.TintProperty, tint);
        FootIcon.Source = HasError ? "icon_circle_alert" : IsValid ? "icon_circle_check" : "icon_info";
    }
}
