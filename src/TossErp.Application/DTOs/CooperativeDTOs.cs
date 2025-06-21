using System;
using TossErp.Domain.Enums;

namespace TossErp.Application.DTOs
{
    // Request DTOs
    public class CreateCooperativeRequest
    {
        public string CooperativeName { get; set; } = string.Empty;
        public string? TradingName { get; set; }
        public CooperativeType CooperativeType { get; set; }
        public string? Description { get; set; }
        public AddressDto Address { get; set; } = new();
        public ContactInfoDto ContactInfo { get; set; } = new();
        public BankAccountDetailsDto? BankAccountDetails { get; set; }
        public decimal InitialShareValue { get; set; }
        public int MinimumMembers { get; set; }
        public int MaximumMembers { get; set; }
        public DateTime? EstablishedDate { get; set; }
    }

    public class UpdateCooperativeRequest
    {
        public Guid Id { get; set; }
        public string? TradingName { get; set; }
        public string? Description { get; set; }
        public AddressDto? Address { get; set; }
        public ContactInfoDto? ContactInfo { get; set; }
        public BankAccountDetailsDto? BankAccountDetails { get; set; }
    }

    public class RegisterCooperativeRequest
    {
        public Guid Id { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public string? TaxNumber { get; set; }
    }

    public class AddCooperativeMemberRequest
    {
        public Guid CooperativeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal ShareValue { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    public class UpdateCooperativeMemberRequest
    {
        public Guid CooperativeId { get; set; }
        public Guid MemberId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    public class AddCooperativeDocumentRequest
    {
        public Guid CooperativeId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? DocumentUrl { get; set; }
    }

    public class ScheduleCooperativeMeetingRequest
    {
        public Guid CooperativeId { get; set; }
        public string MeetingType { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public string? Location { get; set; }
    }

    public class CooperativeFilterRequest
    {
        public string? CooperativeName { get; set; }
        public CooperativeType? CooperativeType { get; set; }
        public string? Township { get; set; }
        public string? Province { get; set; }
        public bool? IsRegistered { get; set; }
        public bool? IsActive { get; set; }
        public int? MinMemberCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }

    // Response DTOs
    public class CooperativeResponse
    {
        public Guid Id { get; set; }
        public string CooperativeName { get; set; } = string.Empty;
        public string? TradingName { get; set; }
        public CooperativeType CooperativeType { get; set; }
        public string? Description { get; set; }
        public AddressDto Address { get; set; } = new();
        public ContactInfoDto ContactInfo { get; set; } = new();
        public BankAccountDetailsDto? BankAccountDetails { get; set; }
        public decimal InitialShareValue { get; set; }
        public int MinimumMembers { get; set; }
        public int MaximumMembers { get; set; }
        public bool IsRegistered { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? TaxNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime EstablishedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public List<CooperativeMemberResponse> Members { get; set; } = new();
        public List<CooperativeDocumentResponse> Documents { get; set; } = new();
        public List<CooperativeMeetingResponse> Meetings { get; set; } = new();
        public int ActiveMemberCount { get; set; }
        public decimal TotalShareValue { get; set; }
        public bool HasMinimumMembers { get; set; }
    }

    public class CooperativeMemberResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal ShareValue { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public DateTime? ExitDate { get; set; }
        public string? ExitReason { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? Notes { get; set; }
    }

    public class CooperativeDocumentResponse
    {
        public Guid Id { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? DocumentUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public bool IsExpired { get; set; }
        public bool IsExpiringSoon { get; set; }
    }

    public class CooperativeMeetingResponse
    {
        public Guid Id { get; set; }
        public string MeetingType { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public string? Location { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? Notes { get; set; }
        public bool IsUpcoming { get; set; }
        public bool IsPast { get; set; }
        public bool IsToday { get; set; }
        public TimeSpan TimeUntilMeeting { get; set; }
    }

    public class CooperativeListResponse
    {
        public List<CooperativeResponse> Cooperatives { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class CooperativeStatisticsResponse
    {
        public int TotalCooperatives { get; set; }
        public int ActiveCooperatives { get; set; }
        public int TotalMembers { get; set; }
        public decimal TotalShareValue { get; set; }
        public int MeetingsThisMonth { get; set; }
        public List<string> TopTownships { get; set; } = new();
    }

    // Value Object DTOs
    public class AddressDto
    {
        public string StreetAddress { get; set; } = string.Empty;
        public string Township { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string? Landmark { get; set; }
    }

    public class ContactInfoDto
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string? EmailAddress { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? Website { get; set; }
    }

    public class BankAccountDetailsDto
    {
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string? BranchCode { get; set; }
        public string? AccountHolderName { get; set; }
        public string? SwiftCode { get; set; }
    }
} 
