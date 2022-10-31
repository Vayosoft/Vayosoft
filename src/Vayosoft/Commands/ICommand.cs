using MediatR;

namespace Vayosoft.Commands
{
    public interface ICommand: IRequest { }
    public interface ICommand<out TResponse> : IRequest<TResponse> { }
}
