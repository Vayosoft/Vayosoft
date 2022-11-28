using Vayosoft.PushBrokers.Exceptions;

namespace Vayosoft.PushBrokers
{
    public delegate void HandlerPushBrokerEvent(object tag, PushBrokerException ex = null);

    public class PushBrokerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public PushBrokerFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public IPushBroker GetFor(string platformName)
        {
            return platformName switch
            {
                "IOS" => (IPushBroker) _serviceProvider.GetService(typeof(ApplePushBroker))!,
                "Android" => (IPushBroker) _serviceProvider.GetService(typeof(GooglePushBroker))!,
                _ => throw new ArgumentOutOfRangeException(nameof(platformName), platformName, null)
            };
        }
    }
}
