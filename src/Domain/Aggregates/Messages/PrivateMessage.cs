using System.Runtime.CompilerServices;
using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Messages;

/// <summary>
/// A private message between two users.
/// </summary>
public record PrivateMessage : Message<PrivateMessage>
{
    #region PROPERTIES

    /// <summary> The user receiving the private message. </summary>
    public virtual User SentTo { get; internal init; } = null!;

    /// <summary> The resource quoted in the message, if any. </summary>
    public virtual Resource? QuotedResource { get; internal init; } = null!;

    /// <summary> The UTC date and time when the message was created. </summary>
    public DateTime CreatedAt { get; internal init; } = DateTime.UtcNow;

    #endregion

    public static IResponse<PrivateMessage> TryCreate(User sentBy, User sentTo, string content, Resource? quotedResource = null)
    {
        if (sentBy.Id == sentTo.Id)
            return Response.Failure<PrivateMessage>(new InvariantException("Un message ne peut pas être envoyé à soi-même !"));

        return Response.Success(new PrivateMessage
        {
            SentBy = sentBy,
            SentTo = sentTo,
            Content = content,
            QuotedResource = quotedResource,
            CreatedAt = DateTime.UtcNow
        });
    }
}