using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Aggregates.QuizSessions;
using ReSR.Domain.Aggregates.Resources;

namespace ReSR.Application.Exceptions;
public class EntityConflictException : Exception {

    public Type   EntityType { get; }
    public string FieldName  { get; }
    public object Value      { get; }

    public EntityConflictException(Type type, Id id)
        : base($"Il existe déjà { type switch {
            Type when type == typeof(User) => "un compte",
            Type when type == typeof(Manager) => "un compte manager",
            Type when type == typeof(Category) => "une catégorie",
            Type when type == typeof(PrivateMessage) => "un message privé",
            Type when type == typeof(Comment) => "un commentaire",
            Type when type == typeof(QuizSession) => "une session de quiz",
            Type when type == typeof(Category) => "une catégorie",
            Type when type == typeof(Resource) => "une ressource",
            Type when type == typeof(TextResource) => "une ressource textuelle",
            Type when type == typeof(QuizResource) => "une ressource quiz",
            _ => $"une entité de type '{type.Name}'"
        }} avec l'identifiant `{id}` !") {
            this.EntityType = type;
            this.FieldName  = "Id";
            this.Value      = id;
        }

    public EntityConflictException(Type type, string fieldName, object value)
        : base($"Il existe déjà { type switch {
            Type when type == typeof(User) => "un compte",
            Type when type == typeof(Manager) => "un compte manager",
            Type when type == typeof(Category) => "une catégorie",
            Type when type == typeof(PrivateMessage) => "un message privé",
            Type when type == typeof(Comment) => "un commentaire",
            Type when type == typeof(QuizSession) => "une session de quiz",
            Type when type == typeof(Category) => "une catégorie",
            Type when type == typeof(Resource) => "une ressource",
            Type when type == typeof(TextResource) => "une ressource textuelle",
            Type when type == typeof(QuizResource) => "une ressource quiz",
            _ => $"une entité de type '{type.Name}'"
        }} avec le champs `{fieldName}` = `{value}`") {
            this.EntityType = type;
            this.FieldName  = fieldName;
            this.Value      = value;
        }
}