namespace eShop.Identity.API.Models.ManageViewModels
{
    public record ConfigureTwoFactorViewModel
    {
        public required string SelectedProvider { get; init; }

        public required ICollection<SelectListItem> Providers { get; init; }
    }
}
