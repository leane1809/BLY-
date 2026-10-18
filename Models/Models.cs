using CommunityToolkit.Mvvm.ComponentModel;

namespace BLY.Models;

/// <summary>Clés de ressources de thème (voir Theme.Light/Dark.xaml).</summary>
public static class K
{
    public const string Texte = "ColorTexte", Secondaire = "ColorTexteSecondaire", Discret = "ColorTexteDiscret";
    public const string Marque = "ColorMarque", Succes = "ColorSucces", Alerte = "ColorAlerte", Erreur = "ColorErreur", Info = "ColorInfo", Violet = "ColorViolet";
    public const string BgMarque = "BrushMarqueFond", BgSucces = "BrushSuccesFond", BgAlerte = "BrushAlerteFond",
                        BgErreur = "BrushErreurFond", BgInfo = "BrushInfoFond", BgViolet = "BrushVioletFond";
    public const string BarMarque = "BrushMarque", BarSucces = "BrushSucces", BarAlerte = "BrushAlerte", BarErreur = "BrushErreur";
}

public enum UserRole { Gerant, Magasinier }

public enum MovementKind { Entry, Exit, Adjust, Transfer }

public record Movement(string Id, string Title, string Subtitle, string Qty, string Unit, MovementKind Kind, string Day = "")
{
    public string Icon => Kind switch
    {
        MovementKind.Entry => "icon_download",
        MovementKind.Exit => "icon_upload",
        MovementKind.Adjust => "icon_triangle_alert",
        _ => "icon_arrow_right_left"
    };
    public string TileKey => Kind switch { MovementKind.Entry => K.BgSucces, MovementKind.Exit => K.BgInfo, MovementKind.Adjust => K.BgAlerte, _ => K.BgViolet };
    public string IconKey => Kind switch { MovementKind.Entry => K.Succes, MovementKind.Exit => K.Info, MovementKind.Adjust => K.Alerte, _ => K.Violet };
    public string QtyKey => Kind switch { MovementKind.Entry => K.Succes, MovementKind.Adjust => K.Alerte, _ => K.Texte };
}

public class MovementGroup : List<Movement>
{
    public string Title { get; }
    public MovementGroup(string title, IEnumerable<Movement> items) : base(items) => Title = title;
}

public class WarehouseOption : ObservableObject
{
    public string Name { get; init; }
    public string Detail { get; init; }
    bool _isSelected;
    public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }
}

public record Zone(string Letter, string Name, string Info, double Fill, string StatusText, string StatusIcon, string StatusKey, string BarKey)
{
    public string PercentText => $"{(int)Math.Round(Fill * 100)} %";
}

public record LocationContent(string Article, string Detail, string Qty, string Unit);

public record ReceptionSummary(string Id, string Supplier, string Lines, string Planned);

public class ReceptionLine : ObservableObject
{
    public string Article { get; init; }
    public string Reference { get; init; }
    public int Expected { get; init; }
    public string Unit { get; init; }
    public string UnitPrice { get; init; }
    public string DefaultLocation { get; init; }
    int _received;
    public int Received { get => _received; set { if (SetProperty(ref _received, value)) { OnPropertyChanged(nameof(Delta)); OnPropertyChanged(nameof(HasDelta)); OnPropertyChanged(nameof(DeltaText)); } } }
    string _location;
    public string Location { get => _location; set => SetProperty(ref _location, value); }
    public int Delta => Received - Expected;
    public bool HasDelta => Delta != 0;
    public string DeltaText => Delta > 0 ? $"+{Delta}" : Delta.ToString().Replace("-", "\u2212");
}

public class LocationSuggestion : ObservableObject
{
    public string Code { get; init; }
    public string Reason { get; init; }
    public bool IsSuggested { get; init; }
    bool _isSelected;
    public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }
}

public record IssueNote(string Id, string Customer, string Detail, string Status, string StatusKind, string StatusIcon, string When);
public record IssueLine(string Article, string Where, string Qty, string Unit);
public record StockLocation(string Code, string Detail, string Qty, string Unit, string Extra = "");

public record CountTask(string Code, string Name, string Detail, string Badge, bool Done = false);

public class VarianceItem : ObservableObject
{
    public string Article { get; init; }
    public string Code { get; init; }
    public int Theoretical { get; init; }
    public int Counted { get; init; }
    public int Delta => Counted - Theoretical;
    public bool HasDelta => Delta != 0;
    public string DeltaText => Delta > 0 ? $"+{Delta}" : Delta == 0 ? "0" : Delta.ToString().Replace("-", "\u2212");
    public string DeltaKey => Delta < 0 ? K.Erreur : Delta > 0 ? K.Alerte : K.Succes;
    public string DeltaIcon => Delta == 0 ? "icon_circle_check" : "icon_triangle_alert";
    public string CardStroke => HasDelta ? "BrushErreurBord" : "BrushBordure";
    string _reason;
    public string Reason { get => _reason; set => SetProperty(ref _reason, value); }
}

public record PendingOp(string Title, string Detail, string Qty, MovementKind Kind);

public record DashboardTile(string Icon, string Count, string Label, string Hint, string TileKey, string IconKey, string HintKey);
