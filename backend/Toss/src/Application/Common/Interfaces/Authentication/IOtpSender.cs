namespace Toss.Application.Common.Interfaces.Authentication;

public interface IOtpSender
{
    Task SendAsync(string destination, string code, CancellationToken cancellationToken = default);
}

