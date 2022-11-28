using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;
using Vayosoft.PushBrokers.Exceptions;

namespace Vayosoft.PushBrokers
{
    public class ApplePushBroker : IPushBroker, IDisposable
    {
        private readonly ILogger<ApplePushBroker> _logger;
        private readonly Timer _fbsTimer;
        private const int CallbackTimeout = 1000 * 60 * 30;
        protected ApnsServiceBroker Broker;

        public event HandlerPushBrokerEvent OnEvent = null!;

        public ApplePushBroker(IConfiguration configuration, ILogger<ApplePushBroker> logger)
        {
            _logger = logger;
            var cfg = configuration.GetApplePushBrokerConfig();

            if (string.IsNullOrEmpty(cfg.CertificatePath))
                throw new ArgumentException(nameof(cfg.CertificatePath));

            var appleCert = File.ReadAllBytes(cfg.CertificatePath);
            // Configuration (NOTE: .pfx can also be used here)
            var env = !cfg.IsProduction
                ? ApnsConfiguration.ApnsServerEnvironment.Sandbox
                : ApnsConfiguration.ApnsServerEnvironment.Production;

            var config = new ApnsConfiguration(env, appleCert, cfg.Password, false);

            Broker = new ApnsServiceBroker(config);
            Broker.ChangeScale(10);

            Broker.OnNotificationFailed += NotificationFailed;
            Broker.OnNotificationSucceeded += notification => OnEvent?.Invoke(notification.Tag);

            var fbs = new FeedbackService(config);
            fbs.FeedbackReceived += FeedbackReceived;
            _fbsTimer = new Timer(_ =>
            {
                fbs.Check();
            }, null, CallbackTimeout, Timeout.Infinite);

            Start();
        }

        private void Start()
        {
            Broker!.Start();
            _logger.LogInformation("Service started.");
        }

        private void NotificationFailed(ApnsNotification notification, AggregateException aggregateEx)
        {
            aggregateEx.Handle(ex => {

                // See what kind of exception it was to further diagnose
                if (ex is ApnsNotificationException notificationException)
                {
                    // Deal with the failed notification
                    var apnsNotification = notificationException.Notification;
                    var statusCode = notificationException.ErrorStatusCode;

                    OnEvent?.Invoke(notification.Tag, new PushBrokerException(
                        $"Apple Notification Failed: ID={apnsNotification.Identifier}, Code={statusCode}, Token={apnsNotification.DeviceToken}", notificationException));

                }
                else
                {
                    OnEvent?.Invoke(notification.Tag, new PushBrokerException(
                        $"Apple Notification Failed for some unknown reason : {ex.InnerException}", ex));
                }

                foreach (var e in aggregateEx.Flatten().InnerExceptions)
                {
                    _logger.LogError("{name}| {message}\r\n{exception}\r\n{stackTrace}",
                        ex.GetType().Name, e.Message, e.InnerException, ex.StackTrace);
                }

                return true;
            });
        }

        public void Send(string token, JObject payload, object tag = null)
        {
            if (string.IsNullOrEmpty(token))
                throw new ApplicationException("parameter: 'token' was not received");

            if (payload is null)
                throw new ApplicationException("parameter: 'payload' was not received");

            // Queue a notification to send
            Broker?.QueueNotification(new ApnsNotification
            {
                DeviceToken = token,
                Payload = payload,
                Tag = tag ?? new { broker = nameof(ApplePushBroker), token }
            });
        }

        private void FeedbackReceived(string deviceToken, DateTime timestamp)
        {
            // Remove the deviceToken from your database
            // timestamp is the time the token was reported as expired
            var message = $"Device subscription expired. Device ID: {deviceToken}";
            OnEvent?.Invoke(new { broker = nameof(ApplePushBroker), deviceToken },
                new PushBrokerException(message));
            Trace.TraceWarning(message);
        }

        public void Dispose()
        {
            _fbsTimer?.Dispose();
            Broker?.Stop(true);

            _logger.LogInformation("Services stopped.");
        }
    }
}
