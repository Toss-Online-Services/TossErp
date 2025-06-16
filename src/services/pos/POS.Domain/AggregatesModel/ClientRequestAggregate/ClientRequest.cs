using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.ClientRequestAggregate;

public class ClientRequest : Entity
{
    public string? RequestId { get; private set; }
    public string? StoreId { get; private set; }
    public string? Status { get; private set; }
    public string? Type { get; private set; }
    public string? Data { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private ClientRequest() { }

    public ClientRequest(string requestId, string storeId, string type, string data)
    {
        RequestId = requestId;
        StoreId = storeId;
        Type = type;
        Data = data;
        Status = "Pending";
        CreatedAt = DateTime.UtcNow;
    }

    public void Process()
    {
        Status = "Processed";
        ProcessedAt = DateTime.UtcNow;
    }

    public void Fail()
    {
        Status = "Failed";
        ProcessedAt = DateTime.UtcNow;
    }
} 
