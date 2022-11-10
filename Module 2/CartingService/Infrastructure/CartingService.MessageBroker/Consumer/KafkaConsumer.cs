using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CartingService.MessageBroker.Consumer
{
    /// <summary>
    /// Base class for implementing Kafka Consumer.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class KafkaConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue> where TValue : class
    {
        private readonly ConsumerConfig _config;
        private IKafkaHandler<TKey, TValue> _handler;
        private IConsumer<TKey, TValue> _consumer;
        private string _topic;
        private bool disposedValue;

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<KafkaConsumer<TKey, TValue>> _logger;

        /// <summary>
        /// Indicates constructor to initialize the serviceScopeFactory and ConsumerConfig
        /// </summary>
        /// <param name="config">Indicates the consumer configuration</param>
        /// <param name="serviceScopeFactory">Indicates the instance for serviceScopeFactory</param>
        public KafkaConsumer(ConsumerConfig config, IServiceScopeFactory serviceScopeFactory,
            ILogger<KafkaConsumer<TKey, TValue>> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// Triggered when the service is ready to consume the Kafka topic.
        /// </summary>
        /// <param name="topic">Indicates Kafka Topic</param>
        /// <param name="stoppingToken">Indicates stopping token</param>
        /// <returns></returns>
        public async Task Consume(string topic, CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            _handler = scope.ServiceProvider.GetRequiredService<IKafkaHandler<TKey, TValue>>();
            _consumer = new ConsumerBuilder<TKey, TValue>(_config).SetValueDeserializer(new KafkaDeserializer<TValue>()).Build();
            _topic = topic;

            await Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
        }

        /// <summary>
        /// This will close the consumer, commit offsets and leave the group cleanly.
        /// </summary>
        public void Close()
        {
            _consumer.Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }

            _consumer.Dispose();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private async Task StartConsumerLoop(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(cancellationToken);

                    if (result != null)
                    {
                        await _handler.HandleAsync(result.Message.Key, result.Message.Value);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    _logger.LogError(e, "Error occurred while consuming the message.");

                    if (e.Error.IsFatal)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unexpected error occurred.");

                    break;
                }
            }
        }
    }
}
