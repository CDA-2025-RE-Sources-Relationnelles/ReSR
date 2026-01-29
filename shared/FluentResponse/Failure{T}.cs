using FluentResponse.Interfaces;

namespace FluentResponse;

/// <summary>
/// A failed data response implementation class.
/// </summary>
public record Failure<T> : Failure, IResponse<T> {

    //=============================
    // I N I T I A L I Z A T I O N
    //=============================

        internal Failure(Exception exception) : base(exception) {}

}