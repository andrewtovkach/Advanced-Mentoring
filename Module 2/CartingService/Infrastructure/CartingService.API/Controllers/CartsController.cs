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
    [Route("api/[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly ILogger<CartsController> _logger;

        public CartsController(IServiceManager serviceManager,
            ILogger<CartsController> logger)
        {
            _serviceManager = serviceManager;
            _logger = logger;
        }

        /// <summary>
        /// Gets the list of all carts.
        /// </summary>
        /// <returns>A list of all carts</returns>
        /// <response code="200">Returns the list of all carts</response>
        [ProducesResponseType(typeof(IList<CartDto>), 200)]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult GetCarts()
        {
            _logger.LogInformation("Getting the list of carts.");

            var carts = _serviceManager.CartService.GetAll();

            _logger.LogInformation("Returning the list of carts.");

            return Ok(carts);
        }

        /// <summary>
        /// Gets the particular cart.
        /// </summary>
        /// <returns>The particular cart</returns>
        /// <param name="cartId"></param>
        /// <response code="200">Returns the particular cart</response>
        /// <response code="404">If the cart does not exist</response>
        [ProducesResponseType(typeof(CartDto), 200)]
        [ProducesResponseType(typeof(ErrorResult), 404)]
        [Produces("application/json")]
        [HttpGet("{cartId:guid}")]
        [MapToApiVersion("1.0")]
        public IActionResult GetCartByIdV1(Guid cartId)
        {
            _logger.LogInformation($"Getting the cart with id {cartId} (version 1).");

            var cartDto = _serviceManager.CartService.GetById(cartId);

            _logger.LogInformation($"Returning the cart with id {cartId} (version 1).");

            return Ok(cartDto);
        }

        /// <summary>
        /// Gets the list of items for the particular cart.
        /// </summary>
        /// <returns>The list of items for the particular cart</returns>
        /// <param name="cartId"></param>
        /// <response code="200">Returns the list of items for the particular cart</response>
        /// <response code="404">If the cart does not exist</response>
        [ProducesResponseType(typeof(IList<ItemDto>), 200)]
        [ProducesResponseType(typeof(ErrorResult), 404)]
        [Produces("application/json")]
        [HttpGet("{cartId:guid}")]
        [MapToApiVersion("2.0")]
        public IActionResult GetCartByIdV2(Guid cartId)
        {
            _logger.LogInformation($"Getting the cart with id {cartId} (version 2).");

            var cartDto = _serviceManager.ItemService.GetAllByCartId(cartId);

            _logger.LogInformation($"Returning the cart with id {cartId} (version 2).");

            return Ok(cartDto);
        }

        /// <summary>
        /// Creates a new cart.
        /// </summary>
        /// <returns>The recently created cart</returns>
        /// <param name="cartForCreationDto"></param>
        /// <response code="201">Returns the recently created cart</response>
        [ProducesResponseType(typeof(CartDto), 201)]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateCart([FromBody] CreateCartDto cartForCreationDto)
        {
            _logger.LogInformation($"Trying to create a new cart with id {cartForCreationDto.Id}.");

            var cartDto = _serviceManager.CartService.Create(cartForCreationDto);

            _logger.LogInformation($"A new cart with id {cartForCreationDto.Id} was created.");

            return CreatedAtAction(nameof(GetCartByIdV1), new { cartId = cartDto.Id }, cartDto);
        }

        /// <summary>
        /// Deletes a cart.
        /// </summary>
        /// <param name="cartId"></param>
        /// <response code="204">If the cart was deleted</response>
        /// <response code="404">If the cart does not exist</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorResult), 404)]
        [HttpDelete("{cartId:guid}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteCart(Guid cartId)
        {
            _logger.LogInformation($"Trying to delete a cart with id {cartId}.");

            _serviceManager.CartService.Delete(cartId);

            _logger.LogInformation($"The cart with id {cartId} was deleted.");

            return NoContent();
        }
    }
}
