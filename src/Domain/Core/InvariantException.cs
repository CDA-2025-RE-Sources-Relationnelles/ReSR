namespace ReSR.Domain.Core;

/// <summary>
/// An exception for invalid invariants.
/// </summary>
/// <typeparam name="T">The invariant's entity / value object's type.</typeparam>
public class InvariantException<T>(string message) : Exception(message);