namespace Identity.Domain.Entities;

public class User : Entity
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public bool IsActive { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string TenantId { get; private set; }
    public UserStatus Status { get; private set; }
    
    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    public User(
        string username,
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        string tenantId)
    {
        Id = Guid.NewGuid();
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        IsActive = true;
        EmailConfirmed = false;
        CreatedAt = DateTime.UtcNow;
        Status = UserStatus.Active;
    }

    private User() { }

    public void UpdateProfile(string firstName, string lastName, string email)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
    }

    public void Deactivate()
    {
        IsActive = false;
        Status = UserStatus.Inactive;
    }

    public void Activate()
    {
        IsActive = true;
        Status = UserStatus.Active;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void AddRole(Role role)
    {
        if (role == null)
            throw new ArgumentNullException(nameof(role));

        if (!_userRoles.Any(ur => ur.RoleId == role.Id))
        {
            _userRoles.Add(new UserRole(Id, role.Id));
        }
    }

    public void RemoveRole(Guid roleId)
    {
        var userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);
        if (userRole != null)
        {
            _userRoles.Remove(userRole);
        }
    }

    public bool HasRole(string roleName)
    {
        return _userRoles.Any(ur => ur.Role?.Name == roleName);
    }

    public bool HasPermission(string permission)
    {
        return _userRoles.Any(ur => ur.Role?.Permissions.Any(p => p.Name == permission) == true);
    }

    public string FullName => $"{FirstName} {LastName}";
}

public enum UserStatus
{
    Active,
    Inactive,
    Suspended,
    Deleted
}
