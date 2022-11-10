using System.Threading.Tasks;
using CatalogService.Application.Categories.Commands.CreateCategory;
using CatalogService.Application.Common.Exceptions;
using CatalogService.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace CatalogService.Application.IntegrationTests.TodoLists.Commands
{
    using static Testing;

    public class CreateCategoryTests : TestBase
    {
        [Test]
        public void ShouldRequireMinimumFields()
        {
            var command = new CreateCategoryCommand();

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task ShouldCreateCategory()
        {
            var command = new CreateCategoryCommand
            {
                Name = "test"
            };

            var id = await SendAsync(command);

            var category = await FindAsync<Category>(id);

            category.Should().NotBeNull();
            category.Name.Should().Be(command.Name);
        }
    }
}
