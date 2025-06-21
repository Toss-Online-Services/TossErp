using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TossErp.Application.DTOs;

namespace TossErp.Application.Services
{
    public interface IStokvelService
    {
        // Core CRUD operations
        Task<StokvelResponseDTO> CreateStokvelAsync(CreateStokvelRequestDTO request);
        Task<StokvelResponseDTO> GetStokvelByIdAsync(Guid id);
        Task<IEnumerable<StokvelResponseDTO>> GetAllStokvelsAsync();
        Task<StokvelResponseDTO> UpdateStokvelAsync(UpdateStokvelRequestDTO request);
        Task DeleteStokvelAsync(Guid id);
        
        // Status management
        Task<StokvelResponseDTO> ActivateStokvelAsync(Guid id);
        Task<StokvelResponseDTO> DeactivateStokvelAsync(Guid id);
        
        // Filtering and search
        Task<IEnumerable<StokvelResponseDTO>> GetStokvelsByTypeAsync(string stokvelType);
        Task<IEnumerable<StokvelResponseDTO>> GetStokvelsByContributionAmountRangeAsync(decimal minAmount, decimal maxAmount);
        Task<IEnumerable<StokvelResponseDTO>> GetStokvelsByContributionFrequencyAsync(string frequency);
        Task<IEnumerable<StokvelResponseDTO>> GetActiveStokvelsAsync();
        Task<IEnumerable<StokvelResponseDTO>> SearchStokvelsAsync(string searchTerm);
        
        // Member management
        Task<StokvelMemberResponseDTO> AddMemberAsync(Guid stokvelId, AddStokvelMemberRequestDTO request);
        Task<StokvelMemberResponseDTO> UpdateMemberAsync(Guid stokvelId, Guid memberId, UpdateStokvelMemberRequestDTO request);
        Task RemoveMemberAsync(Guid stokvelId, Guid memberId);
        
        // Financial operations
        Task<StokvelContributionResponseDTO> RecordContributionAsync(Guid stokvelId, RecordContributionRequestDTO request);
        Task<StokvelPayoutResponseDTO> ProcessPayoutAsync(Guid stokvelId, ProcessPayoutRequestDTO request);
        
        // Meeting management
        Task<StokvelMeetingResponseDTO> ScheduleMeetingAsync(Guid stokvelId, ScheduleStokvelMeetingRequestDTO request);
        
        // Financial summaries
        Task<StokvelFinancialSummaryDTO> GetFinancialSummaryAsync(Guid stokvelId);
        Task<StokvelMemberFinancialDTO> GetMemberFinancialSummaryAsync(Guid stokvelId, Guid memberId);
        
        // Statistics
        Task<StokvelStatisticsDTO> GetStokvelStatisticsAsync();
        
        // Additional business logic methods
        Task<decimal> GetTotalContributionsAsync(Guid stokvelId);
        Task<decimal> GetTotalPayoutsAsync(Guid stokvelId);
        Task<decimal> GetCurrentBalanceAsync(Guid stokvelId);
        Task<decimal> GetMemberContributionTotalAsync(Guid stokvelId, Guid memberId);
        Task<decimal> GetMemberPayoutTotalAsync(Guid stokvelId, Guid memberId);
        Task<decimal> GetMemberBalanceAsync(Guid stokvelId, Guid memberId);
        Task<int> GetActiveMemberCountAsync(Guid stokvelId);
        Task<bool> HasMinimumMembersAsync(Guid stokvelId, int minimumMembers = 5);
        Task<bool> IsFullAsync(Guid stokvelId);
        Task<List<StokvelMemberResponseDTO>> GetMembersInRotationOrderAsync(Guid stokvelId);
        Task<MemberBalanceResponse> GetMemberBalanceDetailsAsync(Guid stokvelId, Guid memberId);
    }
} 
