namespace eShop.Identity.API.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public required string CardNumber { get; set; }
        [Required]
        public required string SecurityNumber { get; set; }
        [Required]
        [RegularExpression(@"(0[1-9]|1[0-2])\/[0-9]{2}", ErrorMessage = "Expiration should match a valid MM/YY value")]
        public required string Expiration { get; set; }
        [Required]
        public required string CardHolderName { get; set; }
        public int CardType { get; set; }
        [Required]
        public required string Street { get; set; }
        [Required]
        public required string City { get; set; }
        [Required]
        public required string State { get; set; }
        [Required]
        public required string Country { get; set; }
        [Required]
        public required string ZipCode { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string LastName { get; set; }
    }
}
