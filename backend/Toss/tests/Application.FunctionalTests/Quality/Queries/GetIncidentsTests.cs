using Toss.Application.Quality.Commands.LogIncident;
using Toss.Application.Quality.Queries.GetIncidents;
using Toss.Domain.Enums;

namespace Toss.Application.FunctionalTests.Quality.Queries;

using static Testing;

public class GetIncidentsTests : BaseTestFixture
{
    [Test]
    public async Task ShouldFilterBySeverity()
    {
        await RunAsDefaultUserAsync();

        // Create incidents with different severities
        var highSeverityId = await SendAsync(new LogIncidentCommand
        {
            Title = "High Severity Incident",
            Type = IncidentType.Safety,
            Severity = IncidentSeverity.High,
            OccurredAt = DateTimeOffset.UtcNow
        });

        var lowSeverityId = await SendAsync(new LogIncidentCommand
        {
            Title = "Low Severity Incident",
            Type = IncidentType.Quality,
            Severity = IncidentSeverity.Low,
            OccurredAt = DateTimeOffset.UtcNow
        });

        var query = new GetIncidentsQuery
        {
            Severity = IncidentSeverity.High,
            PageSize = 10
        };

        var result = await SendAsync(query);

        result.Items.Count.ShouldBe(1);
        result.Items[0].Id.ShouldBe(highSeverityId);
        result.Items[0].Severity.ShouldBe(IncidentSeverity.High);
    }

    [Test]
    public async Task ShouldFilterByDateRange()
    {
        await RunAsDefaultUserAsync();

        var now = DateTimeOffset.UtcNow;
        var yesterday = now.AddDays(-1);
        var tomorrow = now.AddDays(1);

        // Create incidents at different times
        var oldIncidentId = await SendAsync(new LogIncidentCommand
        {
            Title = "Old Incident",
            Type = IncidentType.Safety,
            Severity = IncidentSeverity.Medium,
            OccurredAt = yesterday
        });

        var recentIncidentId = await SendAsync(new LogIncidentCommand
        {
            Title = "Recent Incident",
            Type = IncidentType.Quality,
            Severity = IncidentSeverity.Medium,
            OccurredAt = now
        });

        var query = new GetIncidentsQuery
        {
            FromDate = now.AddHours(-1),
            ToDate = tomorrow,
            PageSize = 10
        };

        var result = await SendAsync(query);

        result.Items.Count.ShouldBe(1);
        result.Items[0].Id.ShouldBe(recentIncidentId);
    }

    [Test]
    public async Task ShouldFilterByType()
    {
        await RunAsDefaultUserAsync();

        var safetyId = await SendAsync(new LogIncidentCommand
        {
            Title = "Safety Incident",
            Type = IncidentType.Safety,
            Severity = IncidentSeverity.Medium,
            OccurredAt = DateTimeOffset.UtcNow
        });

        var qualityId = await SendAsync(new LogIncidentCommand
        {
            Title = "Quality Incident",
            Type = IncidentType.Quality,
            Severity = IncidentSeverity.Medium,
            OccurredAt = DateTimeOffset.UtcNow
        });

        var query = new GetIncidentsQuery
        {
            Type = IncidentType.Safety,
            PageSize = 10
        };

        var result = await SendAsync(query);

        result.Items.Count.ShouldBe(1);
        result.Items[0].Id.ShouldBe(safetyId);
        result.Items[0].Type.ShouldBe(IncidentType.Safety);
    }
}

