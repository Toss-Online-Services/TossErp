namespace Toss.Application.Common.Models.Businesses;

public record BusinessMemberDto(
    int BusinessId,
    string UserId,
    string Email,
    string? FirstName,
    string? LastName,
    string Role,
    bool IsDefault);

