using CartingService.MessageBroker.Consumer;
using CartingService.Services.Constants;
using CartingService.Services.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CartingService.Services.EventHandlers
{
    public class ItemChangedConsumer : BackgroundService
	{
		private readonly IKafkaConsumer<string, ItemChanged> _consumer;
        private readonly ILogger<ItemChangedConsumer> _logger;

        public ItemChangedConsumer(IKafkaConsumer<string, ItemChanged> kafkaConsumer,
			ILogger<ItemChangedConsumer> logger)
		{
			_consumer = kafkaConsumer;
            _logger = logger;
        }

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			try
			{
				await _consumer.Consume(KafkaTopics.ItemChanged, stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occurred while processing the message from topic {KafkaTopics.ItemChanged}");
			}
		}

		public override void Dispose()
		{
			_consumer.Close();
			_consumer.Dispose();

			base.Dispose();
		}
	}
}
