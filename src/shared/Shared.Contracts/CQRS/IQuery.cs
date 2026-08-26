using MediatR;

namespace Shared.Contracts.CQRS;

// Queries
public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : notnull { }
