using System.Windows.Input;

namespace BLY.Controls;

/// <summary>État vide / erreur / permission : illustration vectorielle légère, 1 message, 1 action.</summary>
public partial class EmptyState : ContentView
{
    public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(string), typeof(EmptyState), "icon_package");
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(EmptyState), string.Empty);
    public static readonly BindableProperty MessageProperty = BindableProperty.Create(nameof(Message), typeof(string), typeof(EmptyState), string.Empty);
    public static readonly BindableProperty ActionTextProperty = BindableProperty.Create(nameof(ActionText), typeof(string), typeof(EmptyState), string.Empty,
        propertyChanged: (b, _, _) => ((EmptyState)b).OnPropertyChanged(nameof(HasAction)));
    public static readonly BindableProperty ActionCommandProperty = BindableProperty.Create(nameof(ActionCommand), typeof(ICommand), typeof(EmptyState));
    public static readonly BindableProperty AccentActionProperty = BindableProperty.Create(nameof(AccentAction), typeof(bool), typeof(EmptyState), false,
        propertyChanged: (b, _, n) =>
        {
            var s = (EmptyState)b;
            if (Application.Current.Resources.TryGetValue((bool)n ? "BtnAccent" : "BtnPrimaire", out var style) && style is Style st)
                s.ActionButton.Style = st;
        });

    public string Icon { get => (string)GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string Message { get => (string)GetValue(MessageProperty); set => SetValue(MessageProperty, value); }
    public string ActionText { get => (string)GetValue(ActionTextProperty); set => SetValue(ActionTextProperty, value); }
    public ICommand ActionCommand { get => (ICommand)GetValue(ActionCommandProperty); set => SetValue(ActionCommandProperty, value); }
    public bool AccentAction { get => (bool)GetValue(AccentActionProperty); set => SetValue(AccentActionProperty, value); }
    public bool HasAction => !string.IsNullOrEmpty(ActionText);

    public EmptyState() => InitializeComponent();
}
