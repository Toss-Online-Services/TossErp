namespace eShop.Identity.API.Models.ConsentViewModels
{
    public class ConsentInputModel
    {
        public required string Button { get; set; }
        public required IEnumerable<string> ScopesConsented { get; set; }
        public bool RememberConsent { get; set; }
        public required string ReturnUrl { get; set; }
        public required string Description { get; set; }
    }
}
