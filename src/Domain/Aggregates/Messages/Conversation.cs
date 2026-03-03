using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Messages;

public record Conversation : IAggregateRoot<Conversation>
{
    public Id Id { get; init; } = new Id();

    private readonly List<IDomainEvent> _domainEvents = new();
    public IEnumerable<IDomainEvent> DomainEvents => _domainEvents;

    public virtual User User1 { get; internal init; } = null!;
    public virtual User User2 { get; internal init; } = null!;

    public virtual ICollection<PrivateMessage> Messages { get; internal set; } = [];

    public static IResponse<Conversation> TryCreate(User user1, User user2)
    {
        if(user1.Id == user2.Id)
            return Response.Failure<Conversation>(
                new InvariantException("Une conversation nécessite deux utilisateurs différents !")
            );

        return Response.Success(new Conversation
        {
            User1 = user1,
            User2 = user2
        });
    }

    public Conversation WithConsumedEvents(out IEnumerable<IDomainEvent> domainEvents)
    {
        domainEvents = _domainEvents.ToArray();
        _domainEvents.Clear();
        return this;
    }

    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}