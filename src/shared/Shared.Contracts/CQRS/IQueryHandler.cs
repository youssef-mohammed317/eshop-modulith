using MediatR;

namespace Shared.Contracts.CQRS;

// Query Handlers
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{ }