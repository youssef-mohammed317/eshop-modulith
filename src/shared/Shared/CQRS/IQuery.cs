using MediatR;

namespace Shared.CQRS;

// Queries
public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : notnull { }
