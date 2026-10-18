using System.Collections.Generic;
using System.Linq;
using BLY.Resources.Styles;

namespace BLY;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        // Applique le thème du système (ou celui choisi dans Paramètres) puis suit ses changements.
        ApplyTheme(RequestedTheme);
        RequestedThemeChanged += (_, e) => ApplyTheme(e.RequestedTheme);
    }

    protected override Window CreateWindow(IActivationState activationState) => new Window(new AppShell());

    /// <summary>Remplace le dictionnaire Clair/Sombre ; les DynamicResource se mettent à jour seuls.</summary>
    public static void ApplyTheme(AppTheme theme)
    {
        var dictionaries = Current!.Resources.MergedDictionaries;
        var wantDark = theme == AppTheme.Dark;
        var existing = dictionaries.FirstOrDefault(d => d is LightTheme || d is DarkTheme);
        if (existing != null && (existing is DarkTheme) == wantDark) return;
        if (existing != null) dictionaries.Remove(existing);
        ResourceDictionary newTheme = wantDark ? (ResourceDictionary)new DarkTheme() : (ResourceDictionary)new LightTheme();
        if (dictionaries is IList<ResourceDictionary> list)
        {
            list.Insert(0, newTheme);
        }
        else
        {
            dictionaries.Add(newTheme);
        }
    }
}
