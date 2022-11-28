using Newtonsoft.Json.Linq;

namespace Vayosoft.PushBrokers
{
    public interface IPushBroker
    { 
        void Send(string token, JObject data, object tag = null);
    }
}
