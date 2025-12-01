using Toss.Application.Common.Exceptions;
using Toss.Application.Quality.Commands.LogIncident;
using Toss.Domain.Entities.Quality;
using Toss.Domain.Enums;

namespace Toss.Application.FunctionalTests.Quality.Commands;

using static Testing;

public class LogIncidentTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireTitle()
    {
        var command = new LogIncidentCommand
        {
            Type = IncidentType.Safety,
            Severity = IncidentSeverity.Medium,
            OccurredAt = DateTimeOffset.UtcNow
        };

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldLogIncident()
    {
        await RunAsDefaultUserAsync();

        var command = new LogIncidentCommand
        {
            Title = "Spill in storage area",
            Description = "Chemical spill detected in storage area A",
            Type = IncidentType.Safety,
            Severity = IncidentSeverity.High,
            OccurredAt = DateTimeOffset.UtcNow.AddHours(-2),
            Notes = "Immediate cleanup required"
        };

        var incidentId = await SendAsync(command);

        var incident = await FindAsync<Incident>(incidentId);

        incident.ShouldNotBeNull();
        incident!.Title.ShouldBe(command.Title);
        incident.Type.ShouldBe(command.Type);
        incident.Severity.ShouldBe(command.Severity);
        incident.OccurredAt.ShouldBe(command.OccurredAt);
    }
}

