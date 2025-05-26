namespace eShop.Identity.API.Models.ManageViewModels
{
    public record IndexViewModel
    {
        public bool HasPassword { get; init; }

        public required IList<UserLoginInfo> Logins { get; init; }

        public required string PhoneNumber { get; init; }

        public bool TwoFactor { get; init; }

        public bool BrowserRemembered { get; init; }
    }
}
