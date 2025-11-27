using System.Collections.Concurrent;
using Toss.Application.Common.Interfaces.Authentication;

namespace Toss.Application.FunctionalTests;

public class TestOtpSender : IOtpSender
{
    private readonly ConcurrentDictionary<string, string> _codes = new();

    public Task SendAsync(string destination, string code, CancellationToken cancellationToken = default)
    {
        _codes[destination] = code;
        return Task.CompletedTask;
    }

    public string? GetLastCode(string destination) =>
        _codes.TryGetValue(destination, out var code) ? code : null;
}

