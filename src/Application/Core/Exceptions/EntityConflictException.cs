namespace ReSR.Application.Core.Exceptions;
public class EntityConflictException : Exception {

    public Type   EntityType { get; }
    public string FieldName  { get; }
    public object Value      { get; }

    public EntityConflictException(Type type, Id id)
        : base($"Il existe déjà une entité de type `{type.Name}` avec l'identifiant `{id}` !") {
            this.EntityType = type;
            this.FieldName  = "Id";
            this.Value      = id;
        }

    public EntityConflictException(Type type, string fieldName, object value)
        : base($"une entité de type `{type.Name}` avec le champs `{fieldName}` = `{value}` !") {
            this.EntityType = type;
            this.FieldName  = fieldName;
            this.Value      = value;
        }
}