using System;
using TossErp.Domain.Enums;

namespace TossErp.Application.DTOs
{
    public class CreateStokvelRequest
    {
        public string StokvelName { get; set; } = string.Empty;
        public StokvelType StokvelType { get; set; }
        public string? Description { get; set; }
        public decimal ContributionAmount { get; set; }
        public string ContributionFrequency { get; set; } = string.Empty;
        public int MemberLimit { get; set; }
        public DateTime? EstablishedDate { get; set; }
    }

    public class UpdateStokvelRequest
    {
        public Guid Id { get; set; }
        public string StokvelName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal ContributionAmount { get; set; }
        public string ContributionFrequency { get; set; } = string.Empty;
    }

    public class AddStokvelMemberRequest
    {
        public Guid StokvelId { get; set; }
        public Guid MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string MemberNumber { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public string? Notes { get; set; }
    }

    public class RecordContributionRequest
    {
        public Guid StokvelId { get; set; }
        public Guid MemberId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ContributionDate { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
    }

    public class ProcessPayoutRequest
    {
        public Guid StokvelId { get; set; }
        public Guid MemberId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PayoutDate { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
    }

    public class StokvelResponse
    {
        public Guid Id { get; set; }
        public string StokvelName { get; set; } = string.Empty;
        public StokvelType StokvelType { get; set; }
        public string? Description { get; set; }
        public decimal ContributionAmount { get; set; }
        public string ContributionFrequency { get; set; } = string.Empty;
        public int MemberLimit { get; set; }
        public bool IsActive { get; set; }
        public DateTime EstablishedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public List<StokvelMemberResponse> Members { get; set; } = new();
        public List<StokvelContributionResponse> Contributions { get; set; } = new();
        public List<StokvelPayoutResponse> Payouts { get; set; } = new();
        public List<StokvelMeetingResponse> Meetings { get; set; } = new();
        public decimal TotalContributions { get; set; }
        public decimal TotalPayouts { get; set; }
        public decimal CurrentBalance { get; set; }
        public int ActiveMemberCount { get; set; }
        public bool HasMinimumMembers { get; set; }
        public bool IsFull { get; set; }
    }

    public class StokvelMemberResponse
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string MemberNumber { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public DateTime? ExitDate { get; set; }
        public string? ExitReason { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? Notes { get; set; }
        public TimeSpan MembershipDuration { get; set; }
        public bool IsLongTermMember { get; set; }
        public bool IsNewMember { get; set; }
    }

    public class StokvelContributionResponse
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ContributionDate { get; set; }
        public string? ReferenceNumber { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string? Notes { get; set; }
        public bool IsLate { get; set; }
        public bool IsOnTime { get; set; }
    }

    public class StokvelPayoutResponse
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PayoutDate { get; set; }
        public string? ReferenceNumber { get; set; }
        public bool IsProcessed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? Notes { get; set; }
        public bool IsPending { get; set; }
        public bool IsOverdue { get; set; }
    }

    public class StokvelMeetingResponse
    {
        public Guid Id { get; set; }
        public string MeetingTitle { get; set; } = string.Empty;
        public DateTime MeetingDate { get; set; }
        public string? Location { get; set; }
        public string? Agenda { get; set; }
        public string? Minutes { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? Notes { get; set; }
        public bool IsUpcoming { get; set; }
        public bool IsPast { get; set; }
        public bool IsToday { get; set; }
        public TimeSpan TimeUntilMeeting { get; set; }
    }

    public class StokvelListResponse
    {
        public List<StokvelResponse> Stokvels { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class StokvelFilterRequest
    {
        public string? StokvelName { get; set; }
        public StokvelType? StokvelType { get; set; }
        public decimal? MinContributionAmount { get; set; }
        public decimal? MaxContributionAmount { get; set; }
        public string? ContributionFrequency { get; set; }
        public bool? IsActive { get; set; }
        public int? MinMemberCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }

    public class MemberBalanceResponse
    {
        public Guid MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public decimal ContributionTotal { get; set; }
        public decimal PayoutTotal { get; set; }
        public decimal Balance { get; set; }
        public int ContributionCount { get; set; }
        public int PayoutCount { get; set; }
    }

    public class StokvelResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string StokvelType { get; set; } = string.Empty;
        public decimal ContributionAmount { get; set; }
        public string ContributionFrequency { get; set; } = string.Empty;
        public int MinimumMembers { get; set; }
        public int MaxMembers { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<StokvelMemberResponseDTO> Members { get; set; } = new();
        public List<StokvelContributionResponseDTO> Contributions { get; set; } = new();
        public List<StokvelPayoutResponseDTO> Payouts { get; set; } = new();
        public List<StokvelMeetingResponseDTO> Meetings { get; set; } = new();
    }

    public class StokvelMemberResponseDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int RotationOrder { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
    }

    public class StokvelContributionResponseDTO
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class StokvelPayoutResponseDTO
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class StokvelMeetingResponseDTO
    {
        public Guid Id { get; set; }
        public string MeetingTitle { get; set; } = string.Empty;
        public DateTime MeetingDate { get; set; }
        public string? Location { get; set; }
        public string? Agenda { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public List<string> Attendees { get; set; } = new();
        public List<string> Minutes { get; set; } = new();
    }

    public class StokvelFinancialSummaryDTO
    {
        public decimal TotalContributions { get; set; }
        public decimal TotalPayouts { get; set; }
        public decimal CurrentBalance { get; set; }
        public int MemberCount { get; set; }
    }

    public class StokvelStatisticsDTO
    {
        public int TotalStokvels { get; set; }
        public int ActiveStokvels { get; set; }
        public int TotalMembers { get; set; }
        public decimal TotalContributions { get; set; }
        public decimal TotalPayouts { get; set; }
        public int MeetingsThisMonth { get; set; }
        public List<string> TopTownships { get; set; } = new();
    }

    public class CreateStokvelRequestDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string StokvelType { get; set; } = string.Empty;
        public decimal ContributionAmount { get; set; }
        public string ContributionFrequency { get; set; } = string.Empty;
        public int MinimumMembers { get; set; }
        public int MaxMembers { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class UpdateStokvelRequestDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string StokvelType { get; set; } = string.Empty;
        public decimal ContributionAmount { get; set; }
        public string ContributionFrequency { get; set; } = string.Empty;
        public int MinimumMembers { get; set; }
        public int MaxMembers { get; set; }
    }

    public class AddStokvelMemberRequestDTO
    {
        public Guid StokvelId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int RotationOrder { get; set; }
        public DateTime JoinDate { get; set; }
        public Guid AddedBy { get; set; }
    }

    public class UpdateStokvelMemberRequestDTO
    {
        public Guid StokvelId { get; set; }
        public Guid MemberId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int RotationOrder { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class RecordContributionRequestDTO
    {
        public Guid StokvelId { get; set; }
        public Guid MemberId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Reference { get; set; } = string.Empty;
    }

    public class ProcessPayoutRequestDTO
    {
        public Guid StokvelId { get; set; }
        public Guid MemberId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Reference { get; set; } = string.Empty;
    }

    public class ScheduleStokvelMeetingRequestDTO
    {
        public Guid StokvelId { get; set; }
        public string MeetingTitle { get; set; } = string.Empty;
        public DateTime MeetingDate { get; set; }
        public string? Location { get; set; }
        public string? Agenda { get; set; }
    }

    public class StokvelMemberFinancialDTO
    {
        public Guid MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public decimal TotalContributions { get; set; }
        public decimal TotalPayouts { get; set; }
        public decimal CurrentBalance { get; set; }
        public int ContributionCount { get; set; }
        public int PayoutCount { get; set; }
        public DateTime LastContributionDate { get; set; }
        public DateTime LastPayoutDate { get; set; }
        public bool IsInGoodStanding { get; set; }
        public decimal OutstandingAmount { get; set; }
    }
} 
