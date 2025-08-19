using MediatR;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Application.Commands.CreateAccount;

/// <summary>
/// Command to create a new account
/// </summary>
public class CreateAccountCommand : IRequest<Guid>
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public AccountType Type { get; init; }
    public AccountCategory Category { get; init; }
    public decimal OpeningBalance { get; init; }
    public string Currency { get; init; } = "ZAR";
}

