namespace Toss.Domain.Constants;

public abstract class Policies
{
    public const string CanPurge = nameof(CanPurge);

    public const string RequireOwnerOrManager = nameof(RequireOwnerOrManager);
    public const string RequirePosAccess = nameof(RequirePosAccess);
    public const string RequireStaffOrAbove = nameof(RequireStaffOrAbove);
}