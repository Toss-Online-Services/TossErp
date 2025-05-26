namespace eShop.Identity.API.Models.AccountViewModels
{
    public record VerifyCodeViewModel
    {
        [Required]
        public required string Provider { get; init; }

        [Required]
        public required string Code { get; init; }

        public string? ReturnUrl { get; init; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; init; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; init; }
    }
}
