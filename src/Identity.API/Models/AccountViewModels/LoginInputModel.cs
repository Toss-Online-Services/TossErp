namespace eShop.Identity.API.Models.AccountViewModels
{
    public record LoginInputModel
    {
        [Required]
        public required string Username { get; init; }

        [Required]
        public required string Password { get; init; }

        public bool RememberLogin { get; init; }
        public string? ReturnUrl { get; init; }
    }
} 
