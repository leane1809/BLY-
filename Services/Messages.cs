using CommunityToolkit.Mvvm.Messaging.Messages;

namespace BLY.Services;

/// <summary>Résultat d'un scan renvoyé à l'écran appelant (mode « return » de la page Scanner).</summary>
public sealed class ScanResultMessage : ValueChangedMessage<string>
{
    public string Target { get; }
    public ScanResultMessage(string code, string target) : base(code) => Target = target;
}
