using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vayosoft.Threading.Channels.Diagnostics;
using Vayosoft.Threading.Channels.Handlers;
using Vayosoft.Threading.Channels.Models;

namespace Vayosoft.Threading.Channels
{
    public class MultiHandlerChannel<T, TIdent, TH> : IDisposable where TH : ChannelHandlerBase<T>
    {
        private readonly IConfiguration _config;
        private readonly ILogger<MultiHandlerChannel<T, TIdent, TH>> _logger;
        private readonly ILoggerFactory _loggerFactory;

        private const int ChannelManagementIntervalMin = 35 * 60 * 1000;

        private readonly ConcurrentDictionary<TIdent, HandlerChannel<T, TH>> _channels = new();
        private readonly ConcurrentDictionary<TIdent, object> _locks = new();
        private Timer _timer;

        [ActivatorUtilitiesConstructor]
        public MultiHandlerChannel(IConfiguration config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<MultiHandlerChannel<T, TIdent, TH>>();

            _timer = new Timer(OnTimerCallback, null, ChannelManagementIntervalMin, ChannelManagementIntervalMin);
        }

        private void OnTimerCallback(object state)
        {
            ClearHandlers();
        }

        public Func<T, bool> this[TIdent key]
        {
            get
            {
                if (!_channels.TryGetValue(key, out var channel))
                {
                    lock (_locks.GetOrAdd(key, s => new object()))
                    {
                        if (!_channels.TryGetValue(key, out channel))
                        {
                            channel = Factory(key);
                            _channels.TryAdd(key, channel);
                        }
                    }
                }

                return channel.Enqueue;
            }
        }

        public bool Enqueue(TIdent key, T item)
        {
            return this[key].Invoke(item);
        }

        public QueueHandlerTelemetryReport GeTelemetryReport()
        {
            var snapshots = _channels.Values.Select(w => w.GetSnapshot())
                .Cast<ChannelHandlerTelemetrySnapshot>().ToList();

            return new MeasurementsBuilder<ChannelHandlerTelemetrySnapshot>(snapshots).Build();
        }

        protected HandlerChannel<T, TH> Factory(TIdent key)
        {
            var type = typeof(HandlerChannel<T, TH>);
  
            var constructor = type.GetConstructor(new[] { typeof(ChannelOptions), typeof(ILoggerFactory) });
            if (constructor != null)
            {
                var options = _config.GetSection(typeof(TH).Name).Get<ChannelOptions>() ?? new ChannelOptions();
                {
                    options.ChannelName = key.ToString();
                }

                return (HandlerChannel<T, TH>)Activator.CreateInstance(type, options, _loggerFactory);
            }

            return Activator.CreateInstance<HandlerChannel<T, TH>>();
        }

        public bool ShouldBeCleared(TIdent key)
        {
            return _channels.TryGetValue(key, out var channel) && channel.ShouldBeCleared;
        }

        public void ClearChannel(TIdent key)
        {
            try
            {
                if (_channels.TryRemove(key, out var channel))
                {
                    channel.Dispose();
                }
            }
            catch { }
        }

        private void ClearHandlers()
        {
            var counter = 0;
            foreach (var channel in _channels)
            {
                if (channel.Value.ShouldBeCleared)
                {
                    ClearChannel(channel.Key);
                    counter++;
                }
            }

            if (counter > 0 && _logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogInformation("{Name} | Cleared {Counter} controllers.", typeof(TH).Name, counter);
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
            _timer = null;

            foreach (var channel in _channels)
                ClearChannel(channel.Key);
        }
    }
}
