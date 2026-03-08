using ReSR.Domain.Aggregates.Resources.ValueObjects;

namespace ReSR.Domain.Extensions;
public static partial class Extensions {
    public static IEnumerable<string> ToLocalizedNames(this Relationships relationships) {
        switch (relationships) {
            case Relationships.All : {
                yield return "Toutes les relations";
                yield break;
            }
            case Relationships.None : {
                yield break;
            }
            default : {
                foreach (var relationship in relationships.GetUniqueValues())
                    yield return relationship switch {
                        Relationships.Self                => "Soi",
                        Relationships.Partner             => "Conjoints",
                        Relationships.Family              => "Famille",
                        Relationships.Work                => "Au travail",
                        Relationships.FriendsAndCommunity => "Ami.e.s et communauté",
                        Relationships.Strangers           => "Inconnu.e.s",
                        _                                 => "Relation inconnue"
                    };
                break;
            }
        }
    }
}