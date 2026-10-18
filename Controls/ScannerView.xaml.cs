using System.Windows.Input;
using Microsoft.Maui.Graphics;
using ZXing.Net.Maui;

namespace BLY.Controls;

/// <summary>
/// Caméra de scan (ZXing.Net.Maui) + cadre, lampe, saisie manuelle, permission refusée et erreur de scan.
/// Déclenche <see cref="CodeDetected"/> avec la valeur lue.
/// </summary>
public partial class ScannerView : ContentView
{
    public static readonly BindableProperty IsActiveProperty = BindableProperty.Create(nameof(IsActive), typeof(bool), typeof(ScannerView), true);
    public static readonly BindableProperty HintProperty = BindableProperty.Create(nameof(Hint), typeof(string), typeof(ScannerView), "Placez le code-barres dans le cadre");
    public static readonly BindableProperty HasErrorProperty = BindableProperty.Create(nameof(HasError), typeof(bool), typeof(ScannerView), false,
        propertyChanged: (b, _, _) => ((ScannerView)b).UpdateFrame());

    public bool IsActive { get => (bool)GetValue(IsActiveProperty); set => SetValue(IsActiveProperty, value); }
    public string Hint { get => (string)GetValue(HintProperty); set => SetValue(HintProperty, value); }
    /// <summary>Cadre rouge + « Code non reconnu » (maquette 38).</summary>
    public bool HasError { get => (bool)GetValue(HasErrorProperty); set => SetValue(HasErrorProperty, value); }

    public event EventHandler<string> CodeDetected;

    readonly ScanFrameDrawable _drawable = new();
    bool _permissionChecked, _cameraAllowed;
    string _lastCode;
    DateTime _lastTime = DateTime.MinValue;

    public bool CameraAllowed => !_permissionChecked || _cameraAllowed;
    public bool CameraDenied => _permissionChecked && !_cameraAllowed;

    public ICommand OpenSettingsCommand => new Command(() => AppInfo.Current.ShowSettingsUI());
    public ICommand ManualCommand => new Command(async () => await AskManualAsync());

    public ScannerView()
    {
        InitializeComponent();
        Reader.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.OneDimensional | BarcodeFormats.TwoDimensional,
            AutoRotate = true,
            Multiple = false
        };
        FrameView.Drawable = _drawable;
        UpdateFrame();
        Loaded += async (_, _) => await EnsurePermissionAsync();
    }

    public async Task EnsurePermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.Camera>();
        _cameraAllowed = status == PermissionStatus.Granted;
        _permissionChecked = true;
        Root.SetDynamicResource(BackgroundColorProperty, _cameraAllowed ? "ColorCamera" : "ColorFond");
        OnPropertyChanged(nameof(CameraAllowed));
        OnPropertyChanged(nameof(CameraDenied));
    }

    void UpdateFrame()
    {
        _drawable.Color = HasError ? Color.FromArgb("#E15757") : Color.FromArgb("#FFC21F");
        FrameView?.Invalidate();
        if (HintIcon != null) HintIcon.Source = HasError ? "icon_circle_alert" : "icon_scan_line";
        Hint = HasError ? "Code non reconnu" : "Placez le code-barres dans le cadre";
    }

    void OnDetected(object sender, BarcodeDetectionEventArgs e)
    {
        var code = e.Results?.FirstOrDefault()?.Value;
        if (string.IsNullOrWhiteSpace(code)) return;
        // anti-rebond : le même code n'est pas relu pendant 1,5 s
        if (code == _lastCode && DateTime.UtcNow - _lastTime < TimeSpan.FromSeconds(1.5)) return;
        _lastCode = code;
        _lastTime = DateTime.UtcNow;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            HasError = false;
            CodeDetected?.Invoke(this, code.Trim());
        });
    }

    void OnTorch(object sender, TappedEventArgs e) => Reader.IsTorchOn = !Reader.IsTorchOn;

    void OnManual(object sender, TappedEventArgs e) => ManualCommand.Execute(null);

    async Task AskManualAsync()
    {
        var code = await Shell.Current.DisplayPromptAsync("Saisie manuelle", "Entrez le code (article, emplacement ou bon)",
            "Valider", "Annuler", "ex. A-03-B-2", 40, Keyboard.Text);
        if (!string.IsNullOrWhiteSpace(code)) CodeDetected?.Invoke(this, code.Trim());
    }
}

/// <summary>Coins du cadre de visée + ligne centrale.</summary>
public class ScanFrameDrawable : IDrawable
{
    public Color Color { get; set; } = Color.FromArgb("#FFC21F");

    public void Draw(ICanvas canvas, RectF r)
    {
        const float len = 34f, inset = 3f;
        canvas.StrokeColor = Color;
        canvas.StrokeSize = 6;
        canvas.StrokeLineCap = LineCap.Round;
        float l = r.Left + inset, t = r.Top + inset, rt = r.Right - inset, b = r.Bottom - inset;
        canvas.DrawLine(l, t, l + len, t); canvas.DrawLine(l, t, l, t + len);
        canvas.DrawLine(rt, t, rt - len, t); canvas.DrawLine(rt, t, rt, t + len);
        canvas.DrawLine(l, b, l + len, b); canvas.DrawLine(l, b, l, b - len);
        canvas.DrawLine(rt, b, rt - len, b); canvas.DrawLine(rt, b, rt, b - len);
        canvas.StrokeSize = 2;
        canvas.DrawLine(r.Left + 20, r.Center.Y, r.Right - 20, r.Center.Y);
    }
}
