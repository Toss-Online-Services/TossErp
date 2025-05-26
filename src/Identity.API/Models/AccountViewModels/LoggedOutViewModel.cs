namespace eShop.Identity.API.Models.AccountViewModels
{
    public record LoggedOutViewModel
    {
        public string? PostLogoutRedirectUri { get; init; }
        public string? ClientName { get; init; }
        public string? SignOutIframeUrl { get; init; }
        public bool AutomaticRedirectAfterSignOut { get; init; }
        public required string LogoutId { get; init; }
        public bool TriggerExternalSignout => ExternalAuthenticationScheme != null;
        public string? ExternalAuthenticationScheme { get; init; }
    }
}
