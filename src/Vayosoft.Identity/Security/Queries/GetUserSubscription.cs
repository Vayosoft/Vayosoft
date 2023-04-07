using Vayosoft.Identity.Providers;
using Vayosoft.Identity.Security.Models;
using Vayosoft.Queries;

namespace Vayosoft.Identity.Security.Queries;

public record GetUserSubscription(string UserName, string ProviderName) : IQuery<UserSubscription>;

public class HandleGetUserSubscription : IQueryHandler<GetUserSubscription, UserSubscription>
{
    private readonly ProviderFactory _providerFactory;

    public HandleGetUserSubscription(ProviderFactory providerFactory)
    {
        this._providerFactory = providerFactory;
    }

    public Task<UserSubscription> Handle(GetUserSubscription request, CancellationToken cancellationToken)
    {
        var (userName, providerName) = request;
        var provider = _providerFactory.GetProviderService(providerName);
        return provider.GetUserSubscription(new UserEntity(userName));
    }
}
