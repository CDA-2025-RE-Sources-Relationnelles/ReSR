using System.Text.Json.Serialization;
using FluentResponse.Interfaces;

namespace FluentResponse;

/// <summary>
/// A failed response implementation class.
/// </summary>
public record Failure : Response, IFailure {

    //=======================
    // D E F I N I T I O N S
    //=======================

        /// <inheritdoc/>
        public override bool Successful => false;

        /// <inheritdoc/>
        [JsonIgnore] public Exception Exception { get; }

        /// <inheritdoc/>
        public string ErrorMessage => this.Exception.Message;


    //=============================
    // I N I T I A L I Z A T I O N
    //=============================

        internal Failure(Exception exception) { this.Exception = exception; }

}