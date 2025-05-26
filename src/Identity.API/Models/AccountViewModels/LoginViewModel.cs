namespace eShop.Identity.API.Models.AccountViewModels
{
    public record LoginViewModel
    {
        public bool AllowRememberLogin { get; init; } = true;
        public bool EnableLocalLogin { get; init; } = true;
        public required string Username { get; init; }
        public required string Password { get; init; }
        public bool RememberLogin { get; init; }
        public string? ReturnUrl { get; init; }
        public required IEnumerable<ExternalProvider> ExternalProviders { get; init; } = Enumerable.Empty<ExternalProvider>();
        public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));
        public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;
        public string? ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;
    }
}
