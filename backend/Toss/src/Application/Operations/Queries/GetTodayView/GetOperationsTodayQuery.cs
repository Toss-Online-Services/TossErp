using Toss.Application.Common.Interfaces;

namespace Toss.Application.Operations.Queries.GetTodayView;

public record GetOperationsTodayQuery(int BusinessId) : IRequest<OperationsTodayDto>;

public class GetOperationsTodayQueryHandler : IRequestHandler<GetOperationsTodayQuery, OperationsTodayDto>
{
    private readonly IOperationsTodayService _operationsTodayService;

    public GetOperationsTodayQueryHandler(IOperationsTodayService operationsTodayService)
    {
        _operationsTodayService = operationsTodayService;
    }

    public Task<OperationsTodayDto> Handle(GetOperationsTodayQuery request, CancellationToken cancellationToken)
    {
        return _operationsTodayService.GetTodayAsync(request.BusinessId, cancellationToken);
    }
}

