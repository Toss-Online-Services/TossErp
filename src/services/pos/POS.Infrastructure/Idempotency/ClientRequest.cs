using System;

namespace TossErp.POS.Infrastructure.Idempotency;

public class ClientRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime RequestDate { get; set; }
} 
