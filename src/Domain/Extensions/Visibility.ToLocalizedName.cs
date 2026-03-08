using ReSR.Domain.Aggregates.Resources.ValueObjects;

namespace ReSR.Domain.Extensions;
public static partial class Extensions {
    public static string ToLocalizedName(this Visibility visibility) => visibility switch {
        Visibility.Private                => "Privée",
        Visibility.Public                 => "Public",
        Visibility.WaitingForVerification => "En attente de vérification",
        Visibility.Suspended              => "Suspendue",
        _                                 => "Visibilité inconnue"
    };
}