using Microsoft.AspNetCore.Identity;
using TossErp.Stock.Application.Common.Models;

namespace TossErp.Stock.Infrastructure.Identity;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }
}
