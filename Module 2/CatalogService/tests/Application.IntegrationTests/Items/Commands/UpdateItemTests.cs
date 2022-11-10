using System.Threading.Tasks;
using CatalogService.Application.Categories.Commands.CreateCategory;
using CatalogService.Application.Common.Exceptions;
using CatalogService.Application.Items.Commands.CreateItem;
using CatalogService.Application.Items.Commands.UpdateItem;
using CatalogService.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace CatalogService.Application.IntegrationTests.TodoItems.Commands
{
    using static Testing;

    public class UpdateItemTests : TestBase
    {
        [Test]
        public void ShouldRequireValidItemId()
        {
            var command = new UpdateItemCommand
            {
                Id = 99,
                Price = 1,
                Amount = 10,
                Name = "test"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<NotFoundException>();
        }

        [Test]
        public async Task ShouldUpdateItem()
        {
            var categoryId = await SendAsync(new CreateCategoryCommand
            {
                Name = "test"
            });

            var itemId = await SendAsync(new CreateItemCommand
            {
                CategoryId = categoryId,
                Price = 1,
                Amount = 10,
                Name = "test"
            });

            var command = new UpdateItemCommand
            {
                Id = itemId,
                Price = 2,
                Amount = 11,
                Name = "updated test"
            };

            await SendAsync(command);

            var item = await FindAsync<Item>(itemId);

            item.Name.Should().Be(command.Name);
        }
    }
}
