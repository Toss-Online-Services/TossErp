using Toss.Application.Common.Exceptions;
using Toss.Application.Quality.Commands.CreateChecklist;
using Toss.Domain.Entities.Quality;

namespace Toss.Application.FunctionalTests.Quality.Commands;

using static Testing;

public class CreateChecklistTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireName()
    {
        var command = new CreateChecklistCommand();

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldCreateChecklist()
    {
        await RunAsDefaultUserAsync();

        var command = new CreateChecklistCommand
        {
            Name = "Safety Inspection",
            Description = "Daily safety inspection checklist",
            Items = new List<ChecklistItemDto>
            {
                new() { Title = "Check fire extinguisher", IsRequired = true, Order = 1 },
                new() { Title = "Verify emergency exits", IsRequired = true, Order = 2 },
                new() { Title = "Check first aid kit", IsRequired = false, Order = 3 }
            }
        };

        var checklistId = await SendAsync(command);

        var checklist = await FindAsync<QualityChecklist>(checklistId);

        checklist.ShouldNotBeNull();
        checklist!.Name.ShouldBe(command.Name);
        checklist.Description.ShouldBe(command.Description);
        checklist.Items.Count.ShouldBe(3);
        checklist.IsActive.ShouldBeTrue();
    }

    [Test]
    public async Task ShouldPreventDuplicateNames()
    {
        await RunAsDefaultUserAsync();

        var command1 = new CreateChecklistCommand
        {
            Name = "Duplicate Name",
            Items = new List<ChecklistItemDto>()
        };

        await SendAsync(command1);

        var command2 = new CreateChecklistCommand
        {
            Name = "Duplicate Name", // Same name
            Items = new List<ChecklistItemDto>()
        };

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command2));
    }
}

