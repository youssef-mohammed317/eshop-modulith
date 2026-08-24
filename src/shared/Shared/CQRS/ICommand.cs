using MediatR;

namespace Shared.CQRS;

public interface ICommand : IRequest
{
}

public interface ICommand<out TResponse> : ICommand, IRequest<TResponse>
    where TResponse : notnull
{
}

