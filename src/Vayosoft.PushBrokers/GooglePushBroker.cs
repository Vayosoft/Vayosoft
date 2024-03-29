﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PushSharp.Core;
using PushSharp.Google;
using Vayosoft.PushBrokers.Exceptions;

namespace Vayosoft.PushBrokers
{
    public class GooglePushBroker : IPushBroker, IDisposable
    {
        private readonly ILogger<GooglePushBroker> _logger;
        private const string FCM_SEND_URL = "https://fcm.googleapis.com/fcm/send";

        protected readonly GcmServiceBroker Broker;

        public event HandlerPushBrokerEvent OnEvent = null!;

        public GooglePushBroker(IConfiguration configuration, ILogger<GooglePushBroker> logger)
        {
            _logger = logger;
            var cfg = configuration.GetGooglePushBrokerConfig();

            if (string.IsNullOrEmpty(cfg.AuthToken))
                throw new ArgumentException(nameof(cfg.AuthToken));

            var config = new GcmConfiguration(optionalSenderID: cfg.SenderId ?? string.Empty, senderAuthToken: cfg.AuthToken, null);
            config.OverrideUrl(FCM_SEND_URL);

            Broker = new GcmServiceBroker(config);
            Broker.OnNotificationFailed += NotificationFailed;
            Broker.OnNotificationSucceeded += notification =>  OnEvent?.Invoke(notification.Tag);

            Start();
        }

        private void Start()
        {
            Broker!.Start();
            _logger.LogInformation("Service started.");
        }

        private void NotificationFailed(INotification notification, AggregateException aggregateEx)
        {
            aggregateEx.Handle(ex => {

                // See what kind of exception it was to further diagnose
                if (ex is GcmNotificationException notificationException)
                {
                    // Deal with the failed notification
                    var gcmNotification = notificationException.Notification;
                    var description = notificationException.Description;

                    OnEvent?.Invoke(notification.Tag, new PushBrokerException(
                        $"GCM Notification Failed: ID={gcmNotification.MessageId}, Desc={description}", notificationException));
                }
                else if (ex is GcmMulticastResultException multicastException)
                {
                    foreach (var succeededNotification in multicastException.Succeeded)
                    {
                        OnEvent?.Invoke(notification.Tag, new PushBrokerException(
                            $"GCM Notification Failed: ID={succeededNotification.MessageId}", multicastException));
                    }

                    foreach (var failedKvp in multicastException.Failed)
                    {
                        var n = failedKvp.Key;
                        var e = failedKvp.Value;

                        OnEvent?.Invoke(notification.Tag, new PushBrokerException(
                            $"GCM Notification Failed: ID={n.MessageId}, Desc={e.Message}", multicastException));
                    }

                }
                else if (ex is DeviceSubscriptionExpiredException deviceSubscriptionExpiredException)
                {
                    var oldId = deviceSubscriptionExpiredException.OldSubscriptionId;
                    var newId = deviceSubscriptionExpiredException.NewSubscriptionId;

                    var message = $"Device RegistrationId Expired: {oldId}";
                    if (!string.IsNullOrWhiteSpace(newId))
                    {
                        // If this value isn't null, our subscription changed and we should update our database
                        message += $"\r\nDevice RegistrationId Changed To: {newId}";
                    }

                    OnEvent?.Invoke(notification.Tag, new PushBrokerException(message, deviceSubscriptionExpiredException));
                }
                else if (ex is RetryAfterException retryException)
                {
                    // If you get rate limited, you should stop sending messages until after the RetryAfterUtc date
                    OnEvent?.Invoke(notification.Tag, new PushBrokerException(
                        $"GCM Rate Limited, don't send more until after {retryException.RetryAfterUtc}", retryException));
                }
                else
                {
                    OnEvent?.Invoke(notification.Tag, new PushBrokerException("GCM Notification Failed for some unknown reason", ex));
                }

                foreach (var e in aggregateEx.Flatten().InnerExceptions)
                {
                    _logger.LogError("{name}| {message}\r\n{exception}\r\n{stackTrace}",
                        ex.GetType().Name, e.Message, e.InnerException, ex.StackTrace);
                }

                return true;
            });
        }

        public void Send(string token, JObject data, object tag = null)
        {
            if (string.IsNullOrEmpty(token))
                throw new ApplicationException("parameter: 'token' was not received");

            if (data == null)
                throw new ApplicationException("parameter: 'data' was not received");

            Broker?.QueueNotification(new GcmNotification
            {
                RegistrationIds = new List<string> {
                    token
                },
                Data = data,
                Tag = tag ?? new { broker = nameof(GooglePushBroker), token }
            });
        }

        public void Dispose()
        {
            Broker?.Stop(true);
            _logger.LogInformation("Services stopped.");
        }
    }
}
