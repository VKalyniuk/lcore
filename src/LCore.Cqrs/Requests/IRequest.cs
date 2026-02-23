namespace Lumini.Core.Cqrs.Requests;

/// <summary>
/// Represents the base interface for all request types in the application.
/// </summary>
/// <remarks>Implementing types should define specific request details and behaviors. This interface serves as a
/// marker, enabling polymorphic handling of various request implementations.</remarks>
public interface IBaseRequest
{
}

/// <summary>
/// Defines a contract for a request message within the system, extending the base request functionality.
/// </summary>
public interface IRequest : IBaseRequest
{
}

/// <summary>
/// Represents a request that returns a response of the specified type.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the request.</typeparam>
public interface IRequest<out TResponse> : IRequest
{
}
