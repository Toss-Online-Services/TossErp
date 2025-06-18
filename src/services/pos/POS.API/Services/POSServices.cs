using MediatR;
using POS.API.Application.Queries;

namespace POS.API.Services;

public class POSServices(
    IMediator mediator,
    IPOSQueries queries,
    ILogger<POSServices> logger)
{
    public IMediator Mediator { get; set; } = mediator;
    public ILogger<POSServices> Logger { get; } = logger;
    public IPOSQueries Queries { get; } = queries;
} 
