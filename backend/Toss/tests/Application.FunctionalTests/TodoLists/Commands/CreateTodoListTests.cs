using Toss.Application.Common.Exceptions;
using Toss.Application.TodoLists.Commands.CreateTodoList;
using Toss.Domain.Entities;

namespace Toss.Application.FunctionalTests.TodoLists.Commands;

using static Testing;

public class CreateTodoListTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateTodoListCommand();
        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldRequireUniqueTitle()
    {
        await SendAsync(new CreateTodoListCommand
        {
            Title = "Shopping"
        });

        var command = new CreateTodoListCommand
        {
            Title = "Shopping"
        };

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldCreateTodoList()
    {
        var userId = await RunAsDefaultUserAsync();

        var command = new CreateTodoListCommand
        {
            Title = "Tasks"
        };

        var id = await SendAsync(command);

        var list = await FindAsync<TodoList>(id);

        list.ShouldNotBeNull();
        list!.Title.ShouldBe(command.Title);
        list.CreatedBy.ShouldBe(userId);
        list.Created.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
