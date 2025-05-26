using System.Text.Json.Serialization;
using System;

namespace eShop.ClientApp.Models.User;

public class UserInfo
{
    [JsonPropertyName("sub")] public required string UserId { get; set; }

    [JsonPropertyName("preferred_username")]
    public required string PreferredUsername { get; set; }

    [JsonPropertyName("name")] public required string Name { get; set; }

    [JsonPropertyName("last_name")] public required string LastName { get; set; }

    [JsonPropertyName("card_number")] public required string CardNumber { get; set; }

    [JsonPropertyName("card_holder")] public required string CardHolder { get; set; }

    [JsonPropertyName("card_security_number")]
    public required string CardSecurityNumber { get; set; }

    [JsonPropertyName("address_city")] public required string Address { get; set; }

    [JsonPropertyName("address_country")] public required string Country { get; set; }

    [JsonPropertyName("address_state")] public required string State { get; set; }

    [JsonPropertyName("address_street")] public required string Street { get; set; }

    [JsonPropertyName("address_zip_code")] public required string ZipCode { get; set; }

    [JsonPropertyName("email")] public required string Email { get; set; }

    [JsonPropertyName("email_verified")] public bool EmailVerified { get; set; }

    [JsonPropertyName("phone_number")] public required string PhoneNumber { get; set; }

    [JsonPropertyName("phone_number_verified")]
    public bool PhoneNumberVerified { get; set; }

    public static UserInfo Default { get; } = new();
}
