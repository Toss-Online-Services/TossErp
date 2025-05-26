namespace eShop.Identity.API.Models.ConsentViewModels
{
    public class ConsentViewModel : ConsentInputModel
    {
        public required string ClientName { get; set; }
        public required string ClientUrl { get; set; }
        public required string ClientLogoUrl { get; set; }
        public bool AllowRememberConsent { get; set; }

        public required IEnumerable<ScopeViewModel> IdentityScopes { get; set; }
        public required IEnumerable<ScopeViewModel> ApiScopes { get; set; }
    }
}
