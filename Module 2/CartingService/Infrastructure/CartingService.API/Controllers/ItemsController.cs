using CartingService.Contracts;
using CatringService.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace CartingService.Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/carts/{cartId:guid}/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly ILogger<ItemsController> _logger;

        public ItemsController(IServiceManager serviceManager,
            ILogger<ItemsController> logger)
        {
            _serviceManager = serviceManager;
            _logger = logger;
        }

        /// <summary>
        /// Gets the list of all items.
        /// </summary>
        /// <returns>A list of all items</returns>
        /// <param name="cartId"></param>
        /// <response code="200">Returns the list of all items</response>
        /// <response code="404">If the cart does not exist</response>
        [ProducesResponseType(typeof(IList<ItemDto>), 200)]
        [ProducesResponseType(typeof(ErrorResult), 404)]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult GetItems(Guid cartId)
        {
            _logger.LogInformation($"Getting the list of items for cart with id {cartId}.");

            var itemsDto = _serviceManager.ItemService.GetAllByCartId(cartId);

            _logger.LogInformation($"Returning the list of items for cart with id {cartId}.");

            return Ok(itemsDto);
        }

        /// <summary>
        /// Gets the particular item.
        /// </summary>
        /// <returns>The particular item</returns>
        /// <param name="cartId"></param>
        /// <param name="itemId"></param>
        /// <response code="200">Returns the particular item</response>
        /// <response code="404">If the cart or item does not exist</response>
        [ProducesResponseType(typeof(CartDto), 200)]
        [ProducesResponseType(typeof(ErrorResult), 404)]
        [Produces("application/json")]
        [HttpGet("{itemId:int}")]
        public IActionResult GetItemById(Guid cartId, int itemId)
        {
            _logger.LogInformation($"Getting the item with id {itemId} (version 1).");

            var itemDto = _serviceManager.ItemService.GetById(cartId, itemId);

            _logger.LogInformation($"Returning the item with id {itemId} (version 1).");

            return Ok(itemDto);
        }

        /// <summary>
        /// Creates a new item.
        /// </summary>
        /// <returns>The recently created item</returns>
        /// <param name="cartId"></param>
        /// <param name="itemForCreationDto"></param>
        /// <response code="201">Returns the recently created item</response>
        /// <response code="404">If the cart does not exist</response>
        [ProducesResponseType(typeof(ItemDto), 201)]
        [ProducesResponseType(typeof(ErrorResult), 404)]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateItem(Guid cartId, [FromBody] CreateItemDto itemForCreationDto)
        {
            _logger.LogInformation($"Trying to create a new item with id {itemForCreationDto.Id}.");

            var response = _serviceManager.ItemService.Create(cartId, itemForCreationDto);

            _logger.LogInformation($"A new item with id {itemForCreationDto.Id} was created.");

            return CreatedAtAction(nameof(GetItemById), new { cartId = response.CartId, itemId = response.Id }, response);
        }

        /// <summary>
        /// Deletes an item
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="itemId"></param>
        /// <response code="204">If the item was deleted</response>
        /// <response code="404">If the cart or item does not exist</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorResult), 404)]
        [HttpDelete("{itemId:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteItem(Guid cartId, int itemId)
        {
            _logger.LogInformation($"Trying to delete an item with id {itemId}.");

            _serviceManager.ItemService.Delete(cartId, itemId);

            _logger.LogInformation($"The item with id {itemId} was deleted.");

            return NoContent();
        }
    }
}
