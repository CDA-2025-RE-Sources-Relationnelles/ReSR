using ReSR.Domain.Aggregates.Accounts.ValueObjects;

namespace ReSR.Domain.Extensions;
public static partial class Extensions {
    public static IEnumerable<string> ToLocalizedNames(this ManagerPermissions self) {
        switch (self) {
            case ManagerPermissions.AdminRole : {
                yield return "Administrateur.rice";
                yield break;
            }
            case ManagerPermissions.SuperAdminRole : {
                yield return "Super administrateur.rice";
                yield break;
            }
            case ManagerPermissions.None : {
                yield break;
            }
            default : {
                foreach (var permission in self.GetUniqueValues())
                    yield return permission switch {
                        ManagerPermissions.ReadContent   => "Accès au contenu",
                        ManagerPermissions.ReadUsers     => "Accès aux utilisateur.rice.s",
                        ManagerPermissions.ReadManagers  => "Accès aux gestionnaires",
                        ManagerPermissions.WriteContent  => "Gestion du contenu",
                        ManagerPermissions.WriteUsers    => "Gestion des utilisateur.rice.s",
                        ManagerPermissions.WriteManagers => "Gestion des gestionnaires",
                        _                                => "Permission inconnue"
                    };
                break;
            }
        }
    }
}