namespace eShop.Identity.API.Models.AccountViewModels
{
    public record LogoutViewModel
    {
        public required string LogoutId { get; init; }
        public bool ShowLogoutPrompt { get; init; } = true;
    }
}
