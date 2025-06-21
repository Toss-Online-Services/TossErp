using System;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.CooperativeAggregate
{
    public class BankAccountDetails : ValueObject
    {
        public string BankName { get; private set; } = string.Empty;
        public string AccountNumber { get; private set; } = string.Empty;
        public string AccountType { get; private set; } = string.Empty;
        public string? BranchCode { get; private set; }
        public string? AccountHolderName { get; private set; }
        public string? SwiftCode { get; private set; }

        public BankAccountDetails(string bankName, string accountNumber, string accountType, string? branchCode = null, string? accountHolderName = null, string? swiftCode = null)
        {
            if (string.IsNullOrWhiteSpace(bankName))
                throw new ArgumentException("Bank name is required.", nameof(bankName));
            
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number is required.", nameof(accountNumber));
            
            if (string.IsNullOrWhiteSpace(accountType))
                throw new ArgumentException("Account type is required.", nameof(accountType));

            BankName = bankName.Trim();
            AccountNumber = accountNumber.Trim();
            AccountType = accountType.Trim();
            BranchCode = branchCode?.Trim();
            AccountHolderName = accountHolderName?.Trim();
            SwiftCode = swiftCode?.Trim();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return BankName;
            yield return AccountNumber;
            yield return AccountType;
            yield return BranchCode ?? string.Empty;
            yield return AccountHolderName ?? string.Empty;
            yield return SwiftCode ?? string.Empty;
        }

        public override string ToString()
        {
            var parts = new List<string> { BankName, AccountType, AccountNumber };
            if (!string.IsNullOrWhiteSpace(BranchCode))
                parts.Add($"Branch: {BranchCode}");
            if (!string.IsNullOrWhiteSpace(AccountHolderName))
                parts.Add(AccountHolderName);
            
            return string.Join(" - ", parts);
        }
    }
} 
