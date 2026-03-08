using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Aggregates.Resources.ValueObjects;

namespace ReSR.Domain.Extensions;
public static partial class Extensions {
    public static IEnumerable<string> ToLocalizedNames(this UserPermissions self) {
        switch (self) {
            case UserPermissions.ModeratorRole : {
                yield return "Modérateur.rice";
                yield break;
            }
            case UserPermissions.None : {
                yield break;
            }
            default : {
                foreach (var permission in self.GetUniqueValues())
                    yield return permission switch {
                        UserPermissions.VerifyComments  => "Modération commentaire",
                        UserPermissions.VerifyResources => "Modération ressources",
                        _                               => "Permission inconnue"
                    };
                break;
            }
        }
    }
}