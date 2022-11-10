using System.Threading.Tasks;
using CatalogService.Application.Categories.Commands.CreateCategory;
using CatalogService.Application.Categories.Commands.DeleteCategory;
using CatalogService.Application.Common.Exceptions;
using CatalogService.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace CatalogService.Application.IntegrationTests.TodoLists.Commands
{
    using static Testing;

    public class DeleteCategoryTests : TestBase
    {
        [Test]
        public void ShouldRequireValidCategoryId()
        {
            var command = new DeleteCategoryCommand { Id = 99 };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<NotFoundException>();
        }

        [Test]
        public async Task ShouldDeleteCategory()
        {
            var categoryId = await SendAsync(new CreateCategoryCommand
            {
                Name = "test"
            });

            await SendAsync(new DeleteCategoryCommand
            {
                Id = categoryId
            });

            var category = await FindAsync<Category>(categoryId);

            category.Should().BeNull();
        }
    }
}
