using FluentResponse.Interfaces;

namespace FluentResponse;

/// <summary>
/// A successful data response implementation.
/// </summary>
/// <typeparam name="T">The response's value type.</typeparam>
public record Success<T> : Success, ISuccess<T> {

    //=====================
    // D E F I N I T I O N
    //=====================

        /// <inheritdoc/>
        public T Value { get; }


    //=============================
    // I N I T I A L I Z A T I O N
    //=============================
    
        internal Success(T value) { this.Value = value; }
}