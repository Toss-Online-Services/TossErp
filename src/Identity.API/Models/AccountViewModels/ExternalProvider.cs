namespace eShop.Identity.API.Models.AccountViewModels
{
    public record ExternalProvider
    {
        public required string DisplayName { get; init; }
        public required string AuthenticationScheme { get; init; }
    }
} 
