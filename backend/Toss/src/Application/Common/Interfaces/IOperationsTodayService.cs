using Toss.Application.Operations.Queries.GetTodayView;

namespace Toss.Application.Common.Interfaces;

public interface IOperationsTodayService
{
    Task<OperationsTodayDto> GetTodayAsync(int businessId, CancellationToken cancellationToken = default);
}

