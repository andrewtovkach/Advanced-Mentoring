using CartingService.MessageBroker.Consumer;
using CartingService.Services.Events;
using CatringService.Services.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CartingService.Services.EventHandlers
{
	public class ItemChangedHandler : IKafkaHandler<string, ItemChanged>
	{
        private readonly IItemService _itemService;
		private readonly IUserCartService _userCartService;
		private readonly ILogger<ItemChangedHandler> _logger;

        public ItemChangedHandler(IItemService itemService,
			IUserCartService userCartService,
			ILogger<ItemChangedHandler> logger)
		{
            _itemService = itemService;
			_userCartService = userCartService;
			_logger = logger;
        }

		public Task HandleAsync(string key, ItemChanged value)
		{
			using (_logger.BeginScope(("CorrelationId", value.CorellationId)))
            {
                _logger.LogInformation("The Item chnaged event handler is trigerred.");

				var currentCartId = _userCartService.GetCurrentCartIdentifier();
				if (currentCartId == System.Guid.Empty)
				{
					return Task.CompletedTask;
				}

				_logger.LogInformation($"Trying to update the item with id {value.Id}.");

				_itemService.Update(currentCartId, new Contracts.UpdateItemDto
				{
					Id = value.Id,
					Name = value.Name,
					Price = value.Price
				});

				_logger.LogInformation($"The item with id {value.Id} has been updated.");
			}

			return Task.CompletedTask;
		}
	}
}
