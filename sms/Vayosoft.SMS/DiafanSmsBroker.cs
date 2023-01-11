using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Commons.Enums;
using Vayosoft.Commons.ValueObjects;
using Vayosoft.Utilities;

namespace Vayosoft.SMS
{
    internal sealed class DiafanSmsBroker : ISmsBroker
    {
        private readonly DiafanConfig _config;
        private readonly ILogger<DiafanSmsBroker> _logger;
        private const string MessageType = "sms.automatic";

        [ActivatorUtilitiesConstructor]
        public DiafanSmsBroker(IConfiguration config, ILogger<DiafanSmsBroker> logger)
            : this(config.GetConfiguration(), logger) { }

        public DiafanSmsBroker(DiafanConfig config, ILogger<DiafanSmsBroker> logger)
        {
            _config = Guard.NotNull(config);
            _logger = logger;
        }

        public string ErrorDesc { get; private set; }
        public Task<OperationStatus> SendAsync(PhoneNumber phoneNumber, string sender, string message, bool useUnicode = true)
        {
            try
            {
                Send(phoneNumber, sender, message, null, useUnicode);

                _logger.LogInformation("Diafan| SEND => {PhoneNumber}\r\n{text}", phoneNumber, message);

                return Task.FromResult(OperationStatus.Complete);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Diafan| SEND => {Message}", e.Message);

                ErrorDesc = e.Message;
                return Task.FromResult(OperationStatus.Failed);
            }
        }

        [Obsolete("Obsolete")]
        private void Send(PhoneNumber phoneNumber, string sender, string text, int? wakeupPort, bool useUnicode)
        {
           var msisdn = PhoneNumber.RemoveIsraelCountryPrefix(phoneNumber);

            var postParams = $"username={_config.Username}&" +
                             $"password={_config.Password}&" +
                             $"to={msisdn}&" +
                             $"message-type={MessageType}&" +
                             $"message={HttpUtility.UrlEncode(text, Encoding.UTF8)}&" +
                             $"amp;from={sender}";

            var request = WebRequest.Create(_config.Url);
            request.Timeout = 30000;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            var buffer = Encoding.UTF8.GetBytes(postParams);
            request.ContentLength = buffer.Length;

            using (var reqStream = request.GetRequestStream())
            {
                reqStream.Write(buffer, 0, buffer.Length);
            }
            using (var resp = request.GetResponse())
            {
                using (var sr = new StreamReader(resp.GetResponseStream()!, Encoding.UTF8))
                {
                    var respText = sr.ReadToEnd();
                    if (string.IsNullOrEmpty(respText) || !respText.Contains("OK", StringComparison.OrdinalIgnoreCase))
                        throw new ApplicationException($"Diafan| SMS not sent, Response: {resp}");
                }
            }
        }
    }

    internal record DiafanConfig
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
    }

    internal static class DiafanOptionsExtensions
    {
        public static DiafanConfig GetConfiguration(this IConfiguration configuration)
        {
            return configuration.GetSection("Diafan").Get<DiafanConfig>();
        }
    }
}
