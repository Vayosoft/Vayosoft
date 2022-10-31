using MediatR;

namespace Vayosoft.Queries
{
    public class QueryBus: IQueryBus
    {
        private readonly IMediator mediator;

        public QueryBus(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public Task<TResponse> Send<TResponse>(IQuery<TResponse> query,
            CancellationToken cancellationToken = default)
        {
            return mediator.Send(query, cancellationToken);
        }

        public IAsyncEnumerable<TResponse> Send<TResponse>(IStreamQuery<TResponse> query,
            CancellationToken cancellationToken = default)
        {
            return mediator.CreateStream(query, cancellationToken);
        }
    }
}
