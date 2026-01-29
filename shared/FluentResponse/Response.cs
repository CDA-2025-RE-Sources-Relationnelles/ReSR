using FluentResponse.Interfaces;

namespace FluentResponse;

/// <summary>
/// A base response implementation class.
/// </summary>
public abstract record Response : IResponse {

    //=======================
    // D E F I N I T I O N S
    //=======================

        /// <summary>
        /// Indicates whether or not the response is successful.
        /// </summary>
        public abstract bool Successful { get; }


    //===============================
    // I N I T I A L I Z A T I O N S
    //===============================

        /// <returns>A successful response with no value.</returns>
        public static IResponse Success() => new Success();

        /// <param name="exception">The response's exception.</param>
        /// <returns>A failed response with the specified exception.</returns>
        public static IResponse Failure(Exception exception) => new Failure(exception);

        /// <param name="message">The response's error message.</param>
        /// <returns>A failed response with the specified error message.</returns>
        public static IResponse Failure(string message) => new Failure(new Exception(message));

        /// <returns>A failed response with no error message.</returns>
        public static IResponse Failure() => new Failure(new Exception());

        /// <typeparam name="T">The response's value type.</typeparam>
        /// <param name="value">The response's value.</param>
        /// <returns>A successful response with the specified value.</returns>
        public static IResponse<T> Success<T>(T value) => new Success<T>(value);

        /// <typeparam name="T">The response's expected value type.</typeparam>
        /// <param name="exception">The response's exception.</param>
        /// <returns>A failed response for the expected value type with the specified exception.</returns>
        public static IResponse<T> Failure<T>(Exception exception) => new Failure<T>(exception);

        /// <typeparam name="T">The response's expected value type.</typeparam>
        /// <param name="message">The response's error message.</param>
        /// <returns>A failed response for the expected value type with the specified error message.</returns>
        public static IResponse<T> Failure<T>(string message) => new Failure<T>(new Exception(message));

        /// <typeparam name="T">The response's expected value type.</typeparam>
        /// <returns>A failed response for the expected value type and no error message.</returns>
        public static IResponse<T> Failure<T>() => new Failure<T>(new Exception());


    //===============================
    // I M P L E M E N T A T I O N S
    //===============================

        /// <summary>
        /// Runs the scope and outputs a successful response, or a failed response with the catched exception.
        /// </summary>
        /// <returns>A response with no value.</returns>
        public static IResponse Wrap(Action scope) {
            try                         { scope(); return Success(); }
            catch (Exception exception) { return Failure(exception); }
        }

        /// <summary>
        /// Outputs a successful response with the scope's output value, or a failed response with the catched exception.
        /// </summary>
        /// <typeparam name="T">The scope's output value type.</typeparam>
        /// <returns>A response with scope's output value type.</returns>
        public static IResponse<T> Wrap<T>(Func<T> scope) {
            try                         { return Success(scope()); }
            catch (Exception exception) { return Failure<T>(exception); }
        }

        /// <summary>
        /// Runs the scope and outputs a successful response, or a failed response with the catched exception.
        /// </summary>
        /// <returns>A response with no value.</returns>
        public static async Task<IResponse> WrapAsync(Func<Task> scope) {
            try                         { await scope(); return Success(); }
            catch (Exception exception) { return Failure(exception); }
        }

        /// <summary>
        /// Outputs a successful response with the scope's output value, or a failed response with the catched exception.
        /// </summary>
        /// <typeparam name="T">The scope's output value type.</typeparam>
        /// <returns>A response with scope's output value type.</returns>
        public static async Task<IResponse<T>> WrapAsync<T>(Func<Task<T>> scope) {
            try                         { return Success(await scope()); }
            catch (Exception exception) { return Failure<T>(exception); }
        }
}