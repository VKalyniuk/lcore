namespace Lumini.Core.Cqrs.Requests;

public interface IBaseRequest
{
}

public interface IRequest : IBaseRequest
{
}

public interface IRequest<out TResponse> : IRequest
{
}
