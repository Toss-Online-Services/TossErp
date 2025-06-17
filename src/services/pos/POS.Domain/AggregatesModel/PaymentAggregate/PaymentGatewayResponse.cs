using POS.Domain.Common;
using POS.Domain.Exceptions;

namespace POS.Domain.AggregatesModel.PaymentAggregate
{
    public class PaymentGatewayResponse : ValueObject
    {
        public string Status { get; private set; }
        public string? TransactionId { get; private set; }
        public string? AuthorizationCode { get; private set; }
        public string? ErrorCode { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string? RawResponse { get; private set; }
        public DateTime ResponseTime { get; private set; }

        private PaymentGatewayResponse() { }

        public PaymentGatewayResponse(string status, string? transactionId = null, 
            string? authorizationCode = null, string? errorCode = null, 
            string? errorMessage = null, string? rawResponse = null)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new DomainException("Status cannot be empty");

            Status = status;
            TransactionId = transactionId;
            AuthorizationCode = authorizationCode;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            RawResponse = rawResponse;
            ResponseTime = DateTime.UtcNow;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Status;
            yield return TransactionId ?? string.Empty;
            yield return AuthorizationCode ?? string.Empty;
            yield return ErrorCode ?? string.Empty;
            yield return ErrorMessage ?? string.Empty;
            yield return RawResponse ?? string.Empty;
            yield return ResponseTime;
        }
    }
} 
