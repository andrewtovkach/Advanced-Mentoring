using CatalogService.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CatalogService.API.Controllers
{
    [Authorize]
    public class ItemPropertiesController : ApiControllerBase
    {
        private readonly ILogger<ItemPropertiesController> _logger;

        public ItemPropertiesController(ILogger<ItemPropertiesController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{id}")]
        public ActionResult<ItemModel> Get(int id)
        {
            _logger.LogInformation($"Getting the item with id {id}.");

            return new JsonResult(new ItemPropertyModel
            {
                Brand = "test",
                Model = "test"
            });

            _logger.LogInformation($"Returning the item with id {id}.");
        }
    }
}
