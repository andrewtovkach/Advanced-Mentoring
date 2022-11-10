using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CatalogService.Application.Common.Models;
using CatalogService.Application.Items.Commands.CreateItem;
using CatalogService.Application.Items.Commands.DeleteItem;
using CatalogService.Application.Items.Commands.UpdateItem;
using CatalogService.Application.Items.Queries.GetItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CatalogService.API.Controllers
{
    [Authorize]
    public class ItemsController : ApiControllerBase
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IMapper _mapper;
        private readonly ILogger<ItemsController> _logger;

        public ItemsController(LinkGenerator linkGenerator,
            IMapper mapper,
            ILogger<ItemsController> logger)
        {
            _linkGenerator = linkGenerator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<LinkCollectionWrapper<ItemModel>>> GetAll([FromQuery] GetItemsQuery query)
        {
            _logger.LogInformation("Getting the list of items.");

            var items = await Mediator.Send(query);

            var metadata = new
            {
                items.TotalCount,
                query.PageSize,
                query.PageNumber,
                items.TotalPages,
                items.HasNextPage,
                items.HasPreviousPage
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var itemsList = new List<ItemModel>();

            foreach (var item in items.Items)
            {
                var itemModel = _mapper.Map<ItemModel>(item);
                itemModel.Links = CreateLinksForItem(item.Id);

                itemsList.Add(itemModel);
            }

            _logger.LogInformation("Returning the list of items.");

            return new LinkCollectionWrapper<ItemModel>(itemsList, CreateLinksForItems(query.CategoryId));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemModel>> Get(int id)
        {
            _logger.LogInformation($"Getting the item with id {id}.");

            var item = await Mediator.Send(new GetItemQuery() { Id = id });

            var itemModel = _mapper.Map<ItemModel>(item);
            itemModel.Links = CreateLinksForItem(item.Id);

            _logger.LogInformation($"Returning the item with id {id}.");

            return itemModel;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> Create(CreateItemCommand command)
        {
            _logger.LogInformation($"Trying to create a new item with name {command.Name}.");

            var result = await Mediator.Send(command);

            _logger.LogInformation($"A new item with name {command.Name} was created.");

            return result;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(int id, UpdateItemCommand command)
        {
            _logger.LogInformation($"Trying to update the item with name {command.Name}.");

            if (id != command.Id)
            {
                return BadRequest();
            }

            await Mediator.Send(command);

            _logger.LogInformation($"The item with name {command.Name} was updated.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation($"Trying to delete an item with id {id}.");

            await Mediator.Send(new DeleteItemCommand { Id = id });

            _logger.LogInformation($"The item with id {id} was deleted.");

            return NoContent();
        }

        private List<Link> CreateLinksForItem(int id)
        {
            return new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Get), values: new { id }), HttpMethods.Get),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Delete), values: new { id }), HttpMethods.Delete, "delete_item"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Update), values: new { id }), HttpMethods.Put, "update_item")
            };
        }

        private List<Link> CreateLinksForItems(int categoryId)
        {
            return new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetAll), values: new { categoryId }), HttpMethods.Get)
            };
        }
    }
}
