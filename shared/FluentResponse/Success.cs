using FluentResponse.Interfaces;

namespace FluentResponse;

/// <summary>
/// A successful response implementation.
/// </summary>
public record Success : Response, ISuccess {

    //=====================
    // D E F I N I T I O N
    //=====================
    
        /// <inheritdoc/>
        public override bool Successful => true;


    //=============================
    // I N I T I A L I Z A T I O N
    //=============================
    
        internal Success() {}
}