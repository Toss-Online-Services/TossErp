using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using TossErp.Workflows.Domain.Entities;
using TossErp.Workflows.Domain.Services;

namespace TossErp.Workflows.Infrastructure.Engine;

/// <summary>
/// Advanced workflow automation engine with intelligent orchestration and multi-tenant support
/// </summary>
public class WorkflowEngine : IWorkflowEngine
{
    private readonly ILogger<WorkflowEngine> _logger;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IWorkflowExecutionService _executionService;
    private readonly IWorkflowTriggerService _triggerService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IWorkflowSecurityService _securityService;
    private readonly IWorkflowAnalyticsService _analyticsService;
    private readonly IWorkflowAuditService _auditService;
    private readonly Dictionary<string, IWorkflowActivity> _activities;
    private readonly Dictionary<string, WorkflowExecution> _runningWorkflows;
    private readonly Timer _maintenanceTimer;

    public WorkflowEngine(
        ILogger<WorkflowEngine> logger,
        IWorkflowRepository workflowRepository,
        IWorkflowExecutionService executionService,
        IWorkflowTriggerService triggerService,
        IServiceProvider serviceProvider,
        IWorkflowSecurityService securityService,
        IWorkflowAnalyticsService analyticsService,
        IWorkflowAuditService auditService)
    {
        _logger = logger;
        _workflowRepository = workflowRepository;
        _executionService = executionService;
        _triggerService = triggerService;
        _serviceProvider = serviceProvider;
        _securityService = securityService;
        _analyticsService = analyticsService;
        _auditService = auditService;
        _activities = new Dictionary<string, IWorkflowActivity>();
        _runningWorkflows = new Dictionary<string, WorkflowExecution>();
        
        // Initialize maintenance timer for cleanup and monitoring
        _maintenanceTimer = new Timer(PerformMaintenance, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        
        RegisterBuiltInActivities();
    }

    public async Task<WorkflowExecution> StartWorkflowAsync(
        string tenantId,
        string workflowId,
        Dictionary<string, object> initialData,
        WorkflowTrigger trigger,
        CancellationToken cancellationToken = default)
    {
        var executionId = Guid.NewGuid().ToString();
        _logger.LogInformation("Starting workflow {WorkflowId} for tenant {TenantId} with execution {ExecutionId}",
            workflowId, tenantId, executionId);

        try
        {
            // Security validation
            var hasPermission = await _securityService.CanExecuteWorkflowAsync(tenantId, workflowId, cancellationToken);
            if (!hasPermission)
            {
                throw new UnauthorizedAccessException($"Tenant {tenantId} does not have permission to execute workflow {workflowId}");
            }

            // Get workflow definition
            var workflow = await _workflowRepository.GetWorkflowAsync(tenantId, workflowId, cancellationToken);
            if (workflow == null)
            {
                throw new InvalidOperationException($"Workflow {workflowId} not found for tenant {tenantId}");
            }

            if (workflow.Status != WorkflowStatus.Active)
            {
                throw new InvalidOperationException($"Workflow {workflowId} is not active (status: {workflow.Status})");
            }

            // Create execution context
            var execution = new WorkflowExecution
            {
                Id = executionId,
                TenantId = tenantId,
                WorkflowId = workflowId,
                WorkflowVersion = workflow.Version,
                Status = WorkflowExecutionStatus.Running,
                StartedAt = DateTime.UtcNow,
                Trigger = trigger,
                Data = new Dictionary<string, object>(initialData),
                ActivityExecutions = new List<ActivityExecution>(),
                ExecutionContext = new WorkflowExecutionContext
                {
                    TenantId = tenantId,
                    UserId = trigger.UserId,
                    CorrelationId = trigger.CorrelationId ?? Guid.NewGuid().ToString(),
                    TimeoutAt = DateTime.UtcNow.Add(workflow.ExecutionTimeout ?? TimeSpan.FromHours(24))
                }
            };

            // Save execution state
            await _workflowRepository.SaveExecutionAsync(execution, cancellationToken);
            _runningWorkflows[executionId] = execution;

            // Audit log
            await _auditService.LogWorkflowStartedAsync(execution, cancellationToken);

            // Start workflow execution
            _ = Task.Run(async () => await ExecuteWorkflowAsync(execution, workflow, cancellationToken), cancellationToken);

            _logger.LogInformation("Workflow execution {ExecutionId} started successfully", executionId);
            return execution;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start workflow {WorkflowId} for tenant {TenantId}", workflowId, tenantId);
            throw;
        }
    }

    public async Task<WorkflowExecution> ResumeWorkflowAsync(
        string executionId,
        string activityId,
        Dictionary<string, object> activityResult,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Resuming workflow execution {ExecutionId} from activity {ActivityId}", executionId, activityId);

        try
        {
            var execution = await GetExecutionAsync(executionId, cancellationToken);
            if (execution == null)
            {
                throw new InvalidOperationException($"Workflow execution {executionId} not found");
            }

            if (execution.Status != WorkflowExecutionStatus.Waiting)
            {
                throw new InvalidOperationException($"Workflow execution {executionId} is not in waiting state (status: {execution.Status})");
            }

            // Find the waiting activity
            var activityExecution = execution.ActivityExecutions.FirstOrDefault(a => a.ActivityId == activityId && a.Status == ActivityExecutionStatus.Waiting);
            if (activityExecution == null)
            {
                throw new InvalidOperationException($"Activity {activityId} is not waiting for input in execution {executionId}");
            }

            // Update activity with result
            activityExecution.Status = ActivityExecutionStatus.Completed;
            activityExecution.CompletedAt = DateTime.UtcNow;
            activityExecution.Result = activityResult;

            // Merge activity result into workflow data
            foreach (var kvp in activityResult)
            {
                execution.Data[kvp.Key] = kvp.Value;
            }

            // Update execution status
            execution.Status = WorkflowExecutionStatus.Running;
            execution.LastActivityCompletedAt = DateTime.UtcNow;

            // Save state
            await _workflowRepository.SaveExecutionAsync(execution, cancellationToken);

            // Resume execution
            var workflow = await _workflowRepository.GetWorkflowAsync(execution.TenantId, execution.WorkflowId, cancellationToken);
            _ = Task.Run(async () => await ContinueWorkflowExecutionAsync(execution, workflow, activityId, cancellationToken), cancellationToken);

            _logger.LogInformation("Workflow execution {ExecutionId} resumed successfully", executionId);
            return execution;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resume workflow execution {ExecutionId}", executionId);
            throw;
        }
    }

    public async Task<bool> CancelWorkflowAsync(string executionId, string reason, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cancelling workflow execution {ExecutionId}. Reason: {Reason}", executionId, reason);

        try
        {
            var execution = await GetExecutionAsync(executionId, cancellationToken);
            if (execution == null)
            {
                _logger.LogWarning("Workflow execution {ExecutionId} not found for cancellation", executionId);
                return false;
            }

            if (execution.Status == WorkflowExecutionStatus.Completed || 
                execution.Status == WorkflowExecutionStatus.Failed || 
                execution.Status == WorkflowExecutionStatus.Cancelled)
            {
                _logger.LogWarning("Workflow execution {ExecutionId} is already in terminal state: {Status}", executionId, execution.Status);
                return false;
            }

            // Update execution status
            execution.Status = WorkflowExecutionStatus.Cancelled;
            execution.CompletedAt = DateTime.UtcNow;
            execution.ErrorMessage = $"Cancelled: {reason}";

            // Cancel running activities
            foreach (var activity in execution.ActivityExecutions.Where(a => a.Status == ActivityExecutionStatus.Running))
            {
                activity.Status = ActivityExecutionStatus.Cancelled;
                activity.CompletedAt = DateTime.UtcNow;
                activity.ErrorMessage = "Workflow cancelled";
            }

            // Save state
            await _workflowRepository.SaveExecutionAsync(execution, cancellationToken);

            // Remove from running workflows
            _runningWorkflows.TryRemove(executionId, out _);

            // Audit log
            await _auditService.LogWorkflowCancelledAsync(execution, reason, cancellationToken);

            // Analytics
            await _analyticsService.RecordWorkflowCancelledAsync(execution, cancellationToken);

            _logger.LogInformation("Workflow execution {ExecutionId} cancelled successfully", executionId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel workflow execution {ExecutionId}", executionId);
            throw;
        }
    }

    public async Task<List<WorkflowExecution>> GetTenantExecutionsAsync(
        string tenantId,
        WorkflowExecutionFilter filter,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting workflow executions for tenant {TenantId}", tenantId);

        try
        {
            var executions = await _workflowRepository.GetExecutionsByTenantAsync(tenantId, filter, cancellationToken);
            
            // Apply additional filtering
            if (!string.IsNullOrEmpty(filter.WorkflowId))
            {
                executions = executions.Where(e => e.WorkflowId == filter.WorkflowId).ToList();
            }

            if (filter.Status.HasValue)
            {
                executions = executions.Where(e => e.Status == filter.Status.Value).ToList();
            }

            if (filter.StartedAfter.HasValue)
            {
                executions = executions.Where(e => e.StartedAt >= filter.StartedAfter.Value).ToList();
            }

            if (filter.StartedBefore.HasValue)
            {
                executions = executions.Where(e => e.StartedAt <= filter.StartedBefore.Value).ToList();
            }

            // Apply pagination
            if (filter.Skip.HasValue)
            {
                executions = executions.Skip(filter.Skip.Value).ToList();
            }

            if (filter.Take.HasValue)
            {
                executions = executions.Take(filter.Take.Value).ToList();
            }

            return executions.OrderByDescending(e => e.StartedAt).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get executions for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<WorkflowAnalytics> GetWorkflowAnalyticsAsync(
        string tenantId,
        string workflowId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting workflow analytics for tenant {TenantId}, workflow {WorkflowId}", tenantId, workflowId);

        try
        {
            return await _analyticsService.GetWorkflowAnalyticsAsync(tenantId, workflowId, startDate, endDate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get analytics for tenant {TenantId}, workflow {WorkflowId}", tenantId, workflowId);
            throw;
        }
    }

    private async Task ExecuteWorkflowAsync(WorkflowExecution execution, WorkflowDefinition workflow, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Executing workflow {WorkflowId} for execution {ExecutionId}", workflow.Id, execution.Id);

            // Start from the first activity or resume from current position
            var currentActivityId = GetNextActivityId(execution, workflow);
            
            while (currentActivityId != null && !cancellationToken.IsCancellationRequested)
            {
                // Check execution timeout
                if (DateTime.UtcNow > execution.ExecutionContext.TimeoutAt)
                {
                    execution.Status = WorkflowExecutionStatus.Failed;
                    execution.ErrorMessage = "Workflow execution timed out";
                    execution.CompletedAt = DateTime.UtcNow;
                    break;
                }

                var activityResult = await ExecuteActivityAsync(execution, workflow, currentActivityId, cancellationToken);
                
                if (activityResult.Status == ActivityExecutionStatus.Waiting)
                {
                    execution.Status = WorkflowExecutionStatus.Waiting;
                    break;
                }
                
                if (activityResult.Status == ActivityExecutionStatus.Failed)
                {
                    execution.Status = WorkflowExecutionStatus.Failed;
                    execution.ErrorMessage = activityResult.ErrorMessage;
                    execution.CompletedAt = DateTime.UtcNow;
                    break;
                }

                // Merge activity result into workflow data
                if (activityResult.Result != null)
                {
                    foreach (var kvp in activityResult.Result)
                    {
                        execution.Data[kvp.Key] = kvp.Value;
                    }
                }

                // Get next activity
                currentActivityId = GetNextActivityId(execution, workflow, currentActivityId);
            }

            // Check if workflow is completed
            if (currentActivityId == null && execution.Status == WorkflowExecutionStatus.Running)
            {
                execution.Status = WorkflowExecutionStatus.Completed;
                execution.CompletedAt = DateTime.UtcNow;
            }

            // Save final state
            await _workflowRepository.SaveExecutionAsync(execution, cancellationToken);

            // Remove from running workflows if completed
            if (execution.Status != WorkflowExecutionStatus.Running && execution.Status != WorkflowExecutionStatus.Waiting)
            {
                _runningWorkflows.TryRemove(execution.Id, out _);
            }

            // Audit and analytics
            await _auditService.LogWorkflowCompletedAsync(execution, cancellationToken);
            await _analyticsService.RecordWorkflowExecutionAsync(execution, cancellationToken);

            _logger.LogInformation("Workflow execution {ExecutionId} completed with status {Status}", 
                execution.Id, execution.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing workflow {ExecutionId}", execution.Id);
            
            execution.Status = WorkflowExecutionStatus.Failed;
            execution.ErrorMessage = ex.Message;
            execution.CompletedAt = DateTime.UtcNow;
            
            await _workflowRepository.SaveExecutionAsync(execution, cancellationToken);
            _runningWorkflows.TryRemove(execution.Id, out _);
        }
    }

    private async Task<ActivityExecution> ExecuteActivityAsync(
        WorkflowExecution execution,
        WorkflowDefinition workflow,
        string activityId,
        CancellationToken cancellationToken)
    {
        var activity = workflow.Activities.FirstOrDefault(a => a.Id == activityId);
        if (activity == null)
        {
            throw new InvalidOperationException($"Activity {activityId} not found in workflow {workflow.Id}");
        }

        var activityExecution = new ActivityExecution
        {
            Id = Guid.NewGuid().ToString(),
            ActivityId = activityId,
            ActivityType = activity.Type,
            Status = ActivityExecutionStatus.Running,
            StartedAt = DateTime.UtcNow,
            Input = PrepareActivityInput(execution, activity)
        };

        execution.ActivityExecutions.Add(activityExecution);
        execution.LastActivityStartedAt = DateTime.UtcNow;

        try
        {
            _logger.LogDebug("Executing activity {ActivityId} of type {ActivityType} in execution {ExecutionId}",
                activityId, activity.Type, execution.Id);

            // Get activity implementation
            if (!_activities.TryGetValue(activity.Type, out var activityImplementation))
            {
                throw new InvalidOperationException($"Activity type {activity.Type} is not registered");
            }

            // Execute activity
            var context = new ActivityExecutionContext
            {
                Execution = execution,
                Activity = activity,
                Input = activityExecution.Input,
                TenantId = execution.TenantId,
                UserId = execution.ExecutionContext.UserId,
                CorrelationId = execution.ExecutionContext.CorrelationId
            };

            var result = await activityImplementation.ExecuteAsync(context, cancellationToken);

            // Update activity execution
            activityExecution.Status = result.Status;
            activityExecution.Result = result.Data;
            activityExecution.ErrorMessage = result.ErrorMessage;
            
            if (result.Status != ActivityExecutionStatus.Waiting)
            {
                activityExecution.CompletedAt = DateTime.UtcNow;
            }

            _logger.LogDebug("Activity {ActivityId} completed with status {Status} in execution {ExecutionId}",
                activityId, result.Status, execution.Id);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing activity {ActivityId} in execution {ExecutionId}", activityId, execution.Id);
            
            activityExecution.Status = ActivityExecutionStatus.Failed;
            activityExecution.ErrorMessage = ex.Message;
            activityExecution.CompletedAt = DateTime.UtcNow;
        }

        return activityExecution;
    }

    private async Task ContinueWorkflowExecutionAsync(
        WorkflowExecution execution,
        WorkflowDefinition workflow,
        string completedActivityId,
        CancellationToken cancellationToken)
    {
        // Resume workflow execution from the next activity
        var nextActivityId = GetNextActivityId(execution, workflow, completedActivityId);
        
        if (nextActivityId != null)
        {
            await ExecuteWorkflowAsync(execution, workflow, cancellationToken);
        }
        else
        {
            // Workflow completed
            execution.Status = WorkflowExecutionStatus.Completed;
            execution.CompletedAt = DateTime.UtcNow;
            await _workflowRepository.SaveExecutionAsync(execution, cancellationToken);
            _runningWorkflows.TryRemove(execution.Id, out _);
        }
    }

    private string? GetNextActivityId(WorkflowExecution execution, WorkflowDefinition workflow, string? currentActivityId = null)
    {
        if (currentActivityId == null)
        {
            // Return the first activity
            return workflow.Activities.FirstOrDefault()?.Id;
        }

        var currentActivity = workflow.Activities.FirstOrDefault(a => a.Id == currentActivityId);
        if (currentActivity?.NextActivityId != null)
        {
            return currentActivity.NextActivityId;
        }

        // Check for conditional transitions
        if (currentActivity?.ConditionalTransitions != null)
        {
            foreach (var transition in currentActivity.ConditionalTransitions)
            {
                if (EvaluateCondition(execution.Data, transition.Condition))
                {
                    return transition.NextActivityId;
                }
            }
        }

        return null; // End of workflow
    }

    private bool EvaluateCondition(Dictionary<string, object> data, string condition)
    {
        // Simple condition evaluation - in production, use a proper expression evaluator
        // For now, support basic equality checks like "status == 'approved'"
        var parts = condition.Split(new[] { "==", "!=", ">", "<", ">=", "<=" }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2) return false;

        var key = parts[0].Trim();
        var expectedValue = parts[1].Trim().Trim('\'', '"');

        if (!data.TryGetValue(key, out var actualValue)) return false;

        return actualValue?.ToString() == expectedValue;
    }

    private Dictionary<string, object> PrepareActivityInput(WorkflowExecution execution, WorkflowActivity activity)
    {
        var input = new Dictionary<string, object>(execution.Data);
        
        // Add activity-specific configuration
        if (activity.Configuration != null)
        {
            foreach (var kvp in activity.Configuration)
            {
                input[$"config_{kvp.Key}"] = kvp.Value;
            }
        }

        return input;
    }

    private async Task<WorkflowExecution?> GetExecutionAsync(string executionId, CancellationToken cancellationToken)
    {
        // Try running workflows first
        if (_runningWorkflows.TryGetValue(executionId, out var runningExecution))
        {
            return runningExecution;
        }

        // Fallback to repository
        return await _workflowRepository.GetExecutionAsync(executionId, cancellationToken);
    }

    private void RegisterBuiltInActivities()
    {
        // Register built-in activity types
        _activities["HttpRequest"] = _serviceProvider.GetRequiredService<HttpRequestActivity>();
        _activities["DatabaseQuery"] = _serviceProvider.GetRequiredService<DatabaseQueryActivity>();
        _activities["SendEmail"] = _serviceProvider.GetRequiredService<SendEmailActivity>();
        _activities["SendNotification"] = _serviceProvider.GetRequiredService<SendNotificationActivity>();
        _activities["UserApproval"] = _serviceProvider.GetRequiredService<UserApprovalActivity>();
        _activities["DataTransformation"] = _serviceProvider.GetRequiredService<DataTransformationActivity>();
        _activities["AIProcessing"] = _serviceProvider.GetRequiredService<AIProcessingActivity>();
        _activities["DocumentGeneration"] = _serviceProvider.GetRequiredService<DocumentGenerationActivity>();
        _activities["Integration"] = _serviceProvider.GetRequiredService<IntegrationActivity>();
        _activities["Conditional"] = _serviceProvider.GetRequiredService<ConditionalActivity>();
        _activities["Loop"] = _serviceProvider.GetRequiredService<LoopActivity>();
        _activities["Parallel"] = _serviceProvider.GetRequiredService<ParallelActivity>();
        _activities["Delay"] = _serviceProvider.GetRequiredService<DelayActivity>();
        _activities["Script"] = _serviceProvider.GetRequiredService<ScriptActivity>();
    }

    private void PerformMaintenance(object? state)
    {
        try
        {
            // Clean up completed executions from memory
            var completedExecutions = _runningWorkflows.Values
                .Where(e => e.Status == WorkflowExecutionStatus.Completed ||
                           e.Status == WorkflowExecutionStatus.Failed ||
                           e.Status == WorkflowExecutionStatus.Cancelled)
                .ToList();

            foreach (var execution in completedExecutions)
            {
                _runningWorkflows.TryRemove(execution.Id, out _);
            }

            // Check for timed out executions
            var timedOutExecutions = _runningWorkflows.Values
                .Where(e => DateTime.UtcNow > e.ExecutionContext.TimeoutAt)
                .ToList();

            foreach (var execution in timedOutExecutions)
            {
                _ = Task.Run(async () => await CancelWorkflowAsync(execution.Id, "Execution timeout", CancellationToken.None));
            }

            _logger.LogDebug("Maintenance completed. Cleaned up {CompletedCount} completed executions, found {TimeoutCount} timed out executions",
                completedExecutions.Count, timedOutExecutions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during workflow engine maintenance");
        }
    }

    public void Dispose()
    {
        _maintenanceTimer?.Dispose();
    }
}

// Supporting interfaces and enums
public interface IWorkflowEngine : IDisposable
{
    Task<WorkflowExecution> StartWorkflowAsync(string tenantId, string workflowId, Dictionary<string, object> initialData, WorkflowTrigger trigger, CancellationToken cancellationToken = default);
    Task<WorkflowExecution> ResumeWorkflowAsync(string executionId, string activityId, Dictionary<string, object> activityResult, CancellationToken cancellationToken = default);
    Task<bool> CancelWorkflowAsync(string executionId, string reason, CancellationToken cancellationToken = default);
    Task<List<WorkflowExecution>> GetTenantExecutionsAsync(string tenantId, WorkflowExecutionFilter filter, CancellationToken cancellationToken = default);
    Task<WorkflowAnalytics> GetWorkflowAnalyticsAsync(string tenantId, string workflowId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}

public enum WorkflowExecutionStatus
{
    Running,
    Waiting,
    Completed,
    Failed,
    Cancelled
}

public enum ActivityExecutionStatus
{
    Running,
    Waiting,
    Completed,
    Failed,
    Cancelled
}

public enum WorkflowStatus
{
    Draft,
    Active,
    Inactive,
    Deprecated
}
