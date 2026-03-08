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
                        ManagerPermissions.ManageContent  => "Gestion du contenu",
                        ManagerPermissions.ManageUsers    => "Gestion des utilisateur.rice.s",
                        ManagerPermissions.ManageManagers => "Gestion des managers",
                        _                                 => "Permission inconnue"
                    };
                break;
            }
        }
    }
}