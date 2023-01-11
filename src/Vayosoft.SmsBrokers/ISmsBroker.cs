using Vayosoft.Commons.Enums;
using Vayosoft.Commons.ValueObjects;

namespace Vayosoft.SmsBrokers
{
    public interface ISmsBroker
    {
        string ErrorDesc { get; }
        Task<OperationStatus> SendAsync(PhoneNumber phoneNumber, string sender, string message, bool useUnicode = true);
    }
}