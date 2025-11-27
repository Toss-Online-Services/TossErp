namespace Toss.Domain.Constants;

public static class BusinessRoles
{
    public const string Owner = nameof(Owner);
    public const string Manager = nameof(Manager);
    public const string Cashier = nameof(Cashier);
    public const string Staff = nameof(Staff);

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Owner,
        Manager,
        Cashier,
        Staff
    };
}

