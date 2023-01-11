namespace Vayosoft.SMS
{
    public class SmsBrokerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public SmsBrokerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ISmsBroker GetFor(string platformName)
        {
            return platformName switch
            {
                "Diafan" => (ISmsBroker) _serviceProvider.GetService(typeof(DiafanSmsBroker))!,
                _ => throw new ArgumentOutOfRangeException(nameof(platformName), platformName, null)
            };
        }
    }
}
