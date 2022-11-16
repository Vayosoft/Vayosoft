using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Threading.Channels.Diagnostics;
using Vayosoft.Threading.Channels.Handlers;
using Vayosoft.Threading.Channels.Models;
using Vayosoft.Threading.Channels.Producers;

namespace Vayosoft.Threading.Channels
{
    public class HandlerChannel<T, TH> : TelemetryProducerConsumerChannelBase<T> where TH : ChannelHandlerBase<T>
    {
        private readonly TH _handler;
        private readonly HandlerMeasurement _measurement;


        [ActivatorUtilitiesConstructor]
        public HandlerChannel(IConfiguration config, ILoggerFactory loggerFactory)
            : this(config.GetSection(typeof(TH).Name).Get<ChannelOptions>(), loggerFactory)
        { }

        public HandlerChannel(ChannelOptions options, ILoggerFactory loggerFactory)
            : base(options, loggerFactory.CreateLogger<HandlerChannel<T, TH>>())
        {
            _measurement = new HandlerMeasurement();
            _handler = CreateHandler(loggerFactory.CreateLogger<TH>());
        }

        protected override async ValueTask OnDataReceivedAsync(T item, CancellationToken token)
        {
            try
            {
                _measurement.StartMeasurement();

                await _handler.HandleAction(item, token);
            }
            catch (OperationCanceledException) { /* ignored */  }
            finally
            {
                _measurement.StopMeasurement();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            try
            {
                _handler.Dispose();
            }
            catch (Exception) { /* ignored */ }
        }

        public bool ShouldBeCleared => _handler.CanBeCleared;

        public override IMetricsSnapshot GetSnapshot()
        {
            var queueSnapshot = (ChannelMetricsSnapshot)base.GetSnapshot();
            var snapshot = new ChannelHandlerTelemetrySnapshot
            {
                HandlerTelemetrySnapshot = (HandlerMetricsSnapshot)_measurement.GetSnapshot(),
                MinTimeMs = queueSnapshot.MinTimeMs,
                MaxTimeMs = queueSnapshot.MaxTimeMs,
                Length = queueSnapshot.Length,
                TotalPendingTimeMs = queueSnapshot.TotalPendingTimeMs,
                OperationCount = queueSnapshot.OperationCount,
                AverageTimePerOperationMs = queueSnapshot.AverageTimePerOperationMs,
                ConsumersCount = queueSnapshot.ConsumersCount,
                DroppedItems = queueSnapshot.DroppedItems
            };

            return snapshot;
        }

        protected TH CreateHandler(ILogger logger)
        {
            var type = typeof(TH);

            var constructor = type.GetConstructor(new[] { typeof(ILogger) });
            if (constructor != null)
            {
                return (TH)Activator.CreateInstance(type,  logger);
            }

            return Activator.CreateInstance<TH>();
        }
    }
}
