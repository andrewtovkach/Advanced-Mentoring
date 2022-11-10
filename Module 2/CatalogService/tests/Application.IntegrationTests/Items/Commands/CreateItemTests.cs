using System.Threading.Tasks;
using CatalogService.Application.Categories.Commands.CreateCategory;
using CatalogService.Application.Common.Exceptions;
using CatalogService.Application.Items.Commands.CreateItem;
using CatalogService.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace CatalogService.Application.IntegrationTests.TodoItems.Commands
{
    using static Testing;

    public class CreateItemTests : TestBase
    {
        [Test]
        public void ShouldRequireMinimumFields()
        {
            var command = new CreateItemCommand();

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task ShouldCreateItem()
        {
            var categoryId = await SendAsync(new CreateCategoryCommand
            {
                Name = "test"
            });

            var command = new CreateItemCommand
            {
                CategoryId = categoryId,
                Price = 1,
                Amount = 10,
                Name = "test"
            };

            var itemId = await SendAsync(command);

            var item = await FindAsync<Item>(itemId);

            item.Should().NotBeNull();
            item.CategoryId.Should().Be(command.CategoryId);
            item.Name.Should().Be(command.Name);
        }
    }
}
