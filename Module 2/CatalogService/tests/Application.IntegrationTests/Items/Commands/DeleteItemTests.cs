using System.Threading.Tasks;
using CatalogService.Application.Categories.Commands.CreateCategory;
using CatalogService.Application.Common.Exceptions;
using CatalogService.Application.Items.Commands.CreateItem;
using CatalogService.Application.Items.Commands.DeleteItem;
using CatalogService.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace CatalogService.Application.IntegrationTests.TodoItems.Commands
{
    using static Testing;

    public class DeleteItemTests : TestBase
    {
        [Test]
        public void ShouldRequireValidItemId()
        {
            var command = new DeleteItemCommand { Id = 99 };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<NotFoundException>();
        }

        [Test]
        public async Task ShouldDeleteItem()
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

            await SendAsync(new DeleteItemCommand
            {
                Id = itemId
            });

            var item = await FindAsync<Item>(itemId);

            item.Should().BeNull();
        }
    }
}
