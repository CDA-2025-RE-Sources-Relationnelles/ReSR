using ReSR.Domain.Aggregates.Accounts;

namespace ReSR.Application.ValueObjects.Accounts;
public readonly record struct Session<T> (
    string Token,
    T      Details
) where T : Account<T>;