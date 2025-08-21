using MediatR;
using TossErp.HumanResources.Domain.Enums;
using TossErp.HumanResources.Domain.Aggregates.EmployeeAggregate;
using Microsoft.Extensions.Logging;

namespace TossErp.HumanResources.Application.Employees.Commands.ChangeEmployeeStatus;

public record ChangeEmployeeStatusCommand(
    Guid EmployeeId,
    EmployeeStatus NewStatus,
    string? Reason = null) : IRequest<bool>;

public class ChangeEmployeeStatusCommandHandler : IRequestHandler<ChangeEmployeeStatusCommand, bool>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<ChangeEmployeeStatusCommandHandler> _logger;

    public ChangeEmployeeStatusCommandHandler(
        IEmployeeRepository employeeRepository,
        ILogger<ChangeEmployeeStatusCommandHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(ChangeEmployeeStatusCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee == null)
        {
            _logger.LogWarning("Employee not found with ID: {EmployeeId}", request.EmployeeId);
            return false;
        }

        var previousStatus = employee.Status;

        switch (request.NewStatus)
        {
            case EmployeeStatus.Active:
                employee.Activate();
                break;
            case EmployeeStatus.Inactive:
                employee.Deactivate(request.Reason);
                break;
            case EmployeeStatus.Terminated:
                employee.Terminate(request.Reason);
                break;
            case EmployeeStatus.OnLeave:
                employee.SetOnLeave();
                break;
            case EmployeeStatus.Suspended:
                employee.Suspend(request.Reason);
                break;
            default:
                throw new ArgumentException($"Invalid employee status: {request.NewStatus}");
        }

        _employeeRepository.Update(employee);
        await _employeeRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        _logger.LogInformation("Employee status changed from {PreviousStatus} to {NewStatus} for ID: {EmployeeId}", 
            previousStatus, request.NewStatus, employee.Id);

        return true;
    }
}
