using MediatR;

namespace Vayosoft.Commands
{
    public class CommandBus: ICommandBus
    {
        private readonly IMediator mediator;

        public CommandBus(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public Task Send(ICommand command, CancellationToken cancellationToken = default)
        {
            return mediator.Send(command, cancellationToken);
        }

        public Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        {
            return mediator.Send(command, cancellationToken);
        }
    }
}
