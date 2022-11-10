using System.Threading.Tasks;
using CatalogService.Application.Categories.Commands.CreateCategory;
using CatalogService.Application.Categories.Commands.UpdateCategory;
using CatalogService.Application.Common.Exceptions;
using CatalogService.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace CatalogService.Application.IntegrationTests.TodoLists.Commands
{
    using static Testing;

    public class UpdateCategoryTests : TestBase
    {
        [Test]
        public void ShouldRequireValidCategoryId()
        {
            var command = new UpdateCategoryCommand
            {
                Id = 99,
                Name = "test"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<NotFoundException>();
        }

        [Test]
        public async Task ShouldUpdateCategory()
        {
            var categoryId = await SendAsync(new CreateCategoryCommand
            {
                Name = "test"
            });

            var command = new UpdateCategoryCommand
            {
                Id = categoryId,
                Name = "updated test"
            };

            await SendAsync(command);

            var category = await FindAsync<Category>(categoryId);

            category.Name.Should().Be(command.Name);
        }
    }
}
