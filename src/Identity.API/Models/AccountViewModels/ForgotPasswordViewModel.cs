namespace eShop.Identity.API.Models.AccountViewModels
{
    public record ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }
    }
}
