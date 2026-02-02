namespace ReSR.Domain.Core;

/// <summary>
/// An exception for invalid invariants.
/// </summary>
public class InvariantException(string message) : Exception(message);