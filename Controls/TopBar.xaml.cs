using System.Windows.Input;
using BLY.Services;

namespace BLY.Controls;

public partial class TopBar : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(TopBar), string.Empty);
    public static readonly BindableProperty SubtitleProperty = BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(TopBar), string.Empty,
        propertyChanged: (b, _, _) => ((TopBar)b).OnPropertyChanged(nameof(HasSubtitle)));
    public static readonly BindableProperty ShowBackProperty = BindableProperty.Create(nameof(ShowBack), typeof(bool), typeof(TopBar), true);
    public static readonly BindableProperty RightIconProperty = BindableProperty.Create(nameof(RightIcon), typeof(string), typeof(TopBar), string.Empty,
        propertyChanged: (b, _, _) => ((TopBar)b).OnPropertyChanged(nameof(HasRight)));
    public static readonly BindableProperty RightCommandProperty = BindableProperty.Create(nameof(RightCommand), typeof(ICommand), typeof(TopBar));
    public static readonly BindableProperty BackCommandProperty = BindableProperty.Create(nameof(BackCommand), typeof(ICommand), typeof(TopBar));

    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string Subtitle { get => (string)GetValue(SubtitleProperty); set => SetValue(SubtitleProperty, value); }
    public bool ShowBack { get => (bool)GetValue(ShowBackProperty); set => SetValue(ShowBackProperty, value); }
    public string RightIcon { get => (string)GetValue(RightIconProperty); set => SetValue(RightIconProperty, value); }
    public ICommand RightCommand { get => (ICommand)GetValue(RightCommandProperty); set => SetValue(RightCommandProperty, value); }
    public ICommand BackCommand { get => (ICommand)GetValue(BackCommandProperty); set => SetValue(BackCommandProperty, value); }

    public bool HasSubtitle => !string.IsNullOrEmpty(Subtitle);
    public bool HasRight => !string.IsNullOrEmpty(RightIcon);

    public TopBar() => InitializeComponent();

    async void OnBackTapped(object sender, TappedEventArgs e)
    {
        if (BackCommand?.CanExecute(null) == true) BackCommand.Execute(null);
        else await Nav.Back();
    }

    void OnRightTapped(object sender, TappedEventArgs e)
    {
        if (RightCommand?.CanExecute(null) == true) RightCommand.Execute(null);
    }
}
