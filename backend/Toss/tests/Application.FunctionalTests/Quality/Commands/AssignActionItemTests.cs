using Toss.Application.Common.Exceptions;
using Toss.Application.Quality.Commands.AssignActionItem;
using Toss.Application.Quality.Commands.LogIncident;
using Toss.Domain.Entities.Quality;
using Toss.Domain.Enums;

namespace Toss.Application.FunctionalTests.Quality.Commands;

using static Testing;

public class AssignActionItemTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireTitle()
    {
        var command = new AssignActionItemCommand();

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldAssignActionItemToIncident()
    {
        await RunAsDefaultUserAsync();

        // Create an incident first
        var incidentId = await SendAsync(new LogIncidentCommand
        {
            Title = "Test Incident",
            Type = IncidentType.Safety,
            Severity = IncidentSeverity.High,
            OccurredAt = DateTimeOffset.UtcNow
        });

        var command = new AssignActionItemCommand
        {
            IncidentId = incidentId,
            Title = "Clean up spill",
            Description = "Immediate cleanup required",
            DueDate = DateTimeOffset.UtcNow.AddDays(1)
        };

        var actionItemId = await SendAsync(command);

        var actionItem = await FindAsync<ActionItem>(actionItemId);

        actionItem.ShouldNotBeNull();
        actionItem!.IncidentId.ShouldBe(incidentId);
        actionItem.Title.ShouldBe(command.Title);
        actionItem.Status.ShouldBe(ActionItemStatus.Open);
    }

    [Test]
    public async Task ShouldRejectPastDueDate()
    {
        await RunAsDefaultUserAsync();

        var command = new AssignActionItemCommand
        {
            Title = "Action with past due date",
            DueDate = DateTimeOffset.UtcNow.AddDays(-1) // Past date
        };

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }
}

