namespace BLY.Controls;

/// <summary>Quantité en grand (44 px, chiffres alignés) avec boutons − / + de 64 px.</summary>
public partial class QuantityStepper : ContentView
{
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(int), typeof(QuantityStepper), 0, BindingMode.TwoWay);
    public static readonly BindableProperty UnitProperty = BindableProperty.Create(nameof(Unit), typeof(string), typeof(QuantityStepper), string.Empty);
    public static readonly BindableProperty MinimumProperty = BindableProperty.Create(nameof(Minimum), typeof(int), typeof(QuantityStepper), 0);
    public static readonly BindableProperty MaximumProperty = BindableProperty.Create(nameof(Maximum), typeof(int), typeof(QuantityStepper), 99999);

    public int Value { get => (int)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public string Unit { get => (string)GetValue(UnitProperty); set => SetValue(UnitProperty, value); }
    public int Minimum { get => (int)GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public int Maximum { get => (int)GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }

    public QuantityStepper() => InitializeComponent();

    void OnMinus(object sender, TappedEventArgs e) => Value = Math.Max(Minimum, Value - 1);
    void OnPlus(object sender, TappedEventArgs e) => Value = Math.Min(Maximum, Value + 1);
}
