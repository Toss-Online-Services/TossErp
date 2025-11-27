using Microsoft.Extensions.Logging;
using Toss.Application.Common.Interfaces.Authentication;

namespace Toss.Infrastructure.Services.Authentication;

public class SmsOtpSender : IOtpSender
{
    private readonly ILogger<SmsOtpSender> _logger;

    public SmsOtpSender(ILogger<SmsOtpSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string destination, string code, CancellationToken cancellationToken = default)
    {
        // TODO: Integrate with real SMS provider. For now, log for observability.
        _logger.LogInformation("Sending OTP code {Code} to {Destination}", code, destination);
        return Task.CompletedTask;
    }
}

