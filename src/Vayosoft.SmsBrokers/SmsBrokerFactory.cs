namespace Vayosoft.SmsBrokers
{
    public class SmsBrokerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public SmsBrokerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ISmsBroker GetFor(string brokerName)
        {
            return brokerName switch
            {
                "Diafan" => (ISmsBroker) _serviceProvider.GetService(typeof(DiafanSmsBroker))!,
                _ => throw new ArgumentOutOfRangeException(nameof(brokerName), brokerName, null)
            };
        }
    }
}
