using MediatR;

namespace Vayosoft.Queries
{
    public interface IQuery<out TResponse> : IRequest<TResponse> { }
    public interface IStreamQuery<out TResponse> : IStreamRequest<TResponse> { }
}
