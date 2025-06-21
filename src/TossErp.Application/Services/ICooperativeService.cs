using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TossErp.Application.DTOs;
using TossErp.Domain.Enums;

namespace TossErp.Application.Services
{
    public interface ICooperativeService
    {
        Task<CooperativeResponse> CreateCooperativeAsync(CreateCooperativeRequest request);
        Task<CooperativeResponse> GetCooperativeByIdAsync(Guid id);
        Task<IEnumerable<CooperativeResponse>> GetAllCooperativesAsync();
        Task<IEnumerable<CooperativeResponse>> GetCooperativesByTypeAsync(CooperativeType cooperativeType);
        Task<IEnumerable<CooperativeResponse>> GetCooperativesByTypeAsync(string cooperativeType);
        Task<IEnumerable<CooperativeResponse>> GetCooperativesByTownshipAsync(string township);
        Task<IEnumerable<CooperativeResponse>> GetCooperativesByProvinceAsync(string province);
        Task<IEnumerable<CooperativeResponse>> GetRegisteredCooperativesAsync();
        Task<IEnumerable<CooperativeResponse>> GetActiveCooperativesAsync();
        Task<IEnumerable<CooperativeResponse>> SearchCooperativesAsync(string searchTerm);
        Task<CooperativeResponse> UpdateCooperativeAsync(Guid id, UpdateCooperativeRequest request);
        Task<CooperativeResponse> RegisterCooperativeAsync(Guid id);
        Task<CooperativeResponse> ActivateCooperativeAsync(Guid id);
        Task<CooperativeResponse> DeactivateCooperativeAsync(Guid id);
        Task<CooperativeMemberResponse> AddMemberAsync(Guid cooperativeId, AddCooperativeMemberRequest request);
        Task<CooperativeMemberResponse> UpdateMemberAsync(Guid cooperativeId, Guid memberId, UpdateCooperativeMemberRequest request);
        Task RemoveMemberAsync(Guid cooperativeId, Guid memberId);
        Task<CooperativeDocumentResponse> AddDocumentAsync(Guid cooperativeId, AddCooperativeDocumentRequest request);
        Task<CooperativeMeetingResponse> ScheduleMeetingAsync(Guid cooperativeId, ScheduleCooperativeMeetingRequest request);
        Task DeleteCooperativeAsync(Guid id);
        Task<CooperativeStatisticsResponse> GetCooperativeStatisticsAsync();
    }
} 
