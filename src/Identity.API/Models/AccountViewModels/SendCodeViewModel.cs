namespace eShop.Identity.API.Models.AccountViewModels
{
    public record SendCodeViewModel
    {
        public required string SelectedProvider { get; init; }

        public required ICollection<SelectListItem> Providers { get; init; }

        public string? ReturnUrl { get; init; }

        public bool RememberMe { get; init; }
    }
}
