namespace eShop.Identity.API.Models.ManageViewModels
{
    public record VerifyPhoneNumberViewModel
    {
        [Required]
        public required string Code { get; init; }

        [Required]
        [Phone]
        [Display(Name = "Phone number")]
        public required string PhoneNumber { get; init; }
    }
}
