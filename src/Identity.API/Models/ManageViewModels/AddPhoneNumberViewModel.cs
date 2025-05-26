namespace eShop.Identity.API.Models.ManageViewModels
{
    public record AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone number")]
        public required string PhoneNumber { get; init; }
    }
}
