using System;
using System.ComponentModel.DataAnnotations;

namespace eShop.POS.Infrastructure.Idempotency;

public class ClientRequest
{
    public string Id { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    public DateTime Time { get; set; }
}
