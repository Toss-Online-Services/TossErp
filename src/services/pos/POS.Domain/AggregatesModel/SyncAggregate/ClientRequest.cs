using POS.Domain.SeedWork;
namespace POS.Domain.AggregatesModel.SyncAggregate;

public class ClientRequest : AggregateRoot
{
    public Guid StoreId { get; private set; }
    public string RequestType { get; private set; }
    public string RequestData { get; private set; }
    public string Status { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
  

    public ClientRequest(Guid storeId, string requestType, string requestData)
    {
        if (storeId == Guid.Empty)
            throw new ArgumentException("Store ID cannot be empty", nameof(storeId));

        if (string.IsNullOrWhiteSpace(requestType))
            throw new ArgumentException("Request type cannot be empty", nameof(requestType));

        if (string.IsNullOrWhiteSpace(requestData))
            throw new ArgumentException("Request data cannot be empty", nameof(requestData));

        StoreId = storeId;
        RequestType = requestType;
        RequestData = requestData;
        Status = "Pending";
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(string status, string? errorMessage = null)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status cannot be empty", nameof(status));

        Status = status;
        ErrorMessage = errorMessage;
        UpdatedAt = DateTime.UtcNow;
    }
} 
