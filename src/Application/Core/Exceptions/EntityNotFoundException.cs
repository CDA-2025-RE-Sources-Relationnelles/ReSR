using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Aggregates.QuizSessions;
using ReSR.Domain.Aggregates.Resources;

namespace ReSR.Application.Core.Exceptions;
public class EntityNotFoundException : Exception {
    
    public Type EntityType { get; }
    public Id?  Id         { get; }

    public EntityNotFoundException(Type type, Id id)
        : base($"Il n'y a pas d'entité de type '{type.Name}' avec l'identifiant '{id}' !") {
            this.EntityType = type;
            this.Id         = id;
        }

    public EntityNotFoundException(Type type)
        : base($"Il n'y a pas { type switch {
            Type when type == typeof(User) => "de compte correspondant",
            Type when type == typeof(Manager) => "de compte manager correspondant",
            Type when type == typeof(Category) => "de catégorie correspondante",
            Type when type == typeof(PrivateMessage) => "de message privé correspondant",
            Type when type == typeof(Comment) => "de commentaire correspondant",
            Type when type == typeof(QuizSession) => "de session de quiz correspondante",
            Type when type == typeof(Category) => "de catégorie correspondante",
            Type when type == typeof(Resource) => "de ressource correspondante",
            Type when type == typeof(TextResource) => "de ressource textuelle correspondante",
            Type when type == typeof(QuizResource) => "de ressource quiz correspondante",
            _ => $"d'entité de type '{type.Name}' correspondante"
        }} !") {
            this.EntityType = type;
        }
}