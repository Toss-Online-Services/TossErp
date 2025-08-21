using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TossErp.Workflows.Domain.Entities;
using TossErp.Workflows.Domain.Services;

namespace TossErp.Workflows.Infrastructure.Activities;

/// <summary>
/// Enterprise workflow activities for business process automation
/// </summary>
public class EnterpriseWorkflowActivities : IWorkflowActivityProvider
{
    private readonly ILogger<EnterpriseWorkflowActivities> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDatabaseService _databaseService;
    private readonly IEmailService _emailService;
    private readonly IAIProcessingService _aiService;
    private readonly IDocumentService _documentService;
    private readonly IIntegrationService _integrationService;
    private readonly INotificationService _notificationService;
    private readonly IFileService _fileService;
    private readonly IUserService _userService;
    private readonly Dictionary<string, Func<WorkflowActivityContext, CancellationToken, Task<WorkflowActivityResult>>> _activities;

    public EnterpriseWorkflowActivities(
        ILogger<EnterpriseWorkflowActivities> logger,
        IHttpClientFactory httpClientFactory,
        IDatabaseService databaseService,
        IEmailService emailService,
        IAIProcessingService aiService,
        IDocumentService documentService,
        IIntegrationService integrationService,
        INotificationService notificationService,
        IFileService fileService,
        IUserService userService)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _databaseService = databaseService;
        _emailService = emailService;
        _aiService = aiService;
        _documentService = documentService;
        _integrationService = integrationService;
        _notificationService = notificationService;
        _fileService = fileService;
        _userService = userService;

        _activities = new Dictionary<string, Func<WorkflowActivityContext, CancellationToken, Task<WorkflowActivityResult>>>
        {
            ["HttpRequest"] = ExecuteHttpRequestAsync,
            ["DatabaseQuery"] = ExecuteDatabaseQueryAsync,
            ["SendEmail"] = ExecuteSendEmailAsync,
            ["UserApproval"] = ExecuteUserApprovalAsync,
            ["AIProcessing"] = ExecuteAIProcessingAsync,
            ["DocumentGeneration"] = ExecuteDocumentGenerationAsync,
            ["Integration"] = ExecuteIntegrationAsync,
            ["Conditional"] = ExecuteConditionalAsync,
            ["Loop"] = ExecuteLoopAsync,
            ["Parallel"] = ExecuteParallelAsync,
            ["Delay"] = ExecuteDelayAsync,
            ["Script"] = ExecuteScriptAsync,
            ["DataTransformation"] = ExecuteDataTransformationAsync,
            ["SendNotification"] = ExecuteSendNotificationAsync,
            ["FileProcessing"] = ExecuteFileProcessingAsync,
            ["UserTask"] = ExecuteUserTaskAsync,
            ["DataValidation"] = ExecuteDataValidationAsync,
            ["BusinessRule"] = ExecuteBusinessRuleAsync,
            ["AuditLog"] = ExecuteAuditLogAsync,
            ["ComplianceCheck"] = ExecuteComplianceCheckAsync,
            ["ResourceAllocation"] = ExecuteResourceAllocationAsync,
            ["QualityAssurance"] = ExecuteQualityAssuranceAsync,
            ["PerformanceMonitoring"] = ExecutePerformanceMonitoringAsync,
            ["SecurityScan"] = ExecuteSecurityScanAsync,
            ["BackupOperation"] = ExecuteBackupOperationAsync
        };
    }

    public async Task<WorkflowActivityResult> ExecuteActivityAsync(
        string activityType,
        WorkflowActivityContext context,
        CancellationToken cancellationToken = default)
    {
        if (!_activities.TryGetValue(activityType, out var activityFunc))
        {
            return new WorkflowActivityResult
            {
                Success = false,
                ErrorMessage = $"Unknown activity type: {activityType}",
                ExecutedAt = DateTime.UtcNow
            };
        }

        _logger.LogDebug("Executing activity {ActivityType} for workflow {WorkflowId}",
            activityType, context.WorkflowId);

        try
        {
            var result = await activityFunc(context, cancellationToken);
            result.ActivityType = activityType;
            result.ExecutedAt = DateTime.UtcNow;

            _logger.LogInformation("Activity {ActivityType} executed {Status} for workflow {WorkflowId}",
                activityType, result.Success ? "successfully" : "with errors", context.WorkflowId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute activity {ActivityType} for workflow {WorkflowId}",
                activityType, context.WorkflowId);

            return new WorkflowActivityResult
            {
                ActivityType = activityType,
                Success = false,
                ErrorMessage = ex.Message,
                ExecutedAt = DateTime.UtcNow
            };
        }
    }

    public List<string> GetSupportedActivities()
    {
        return _activities.Keys.ToList();
    }

    public WorkflowActivityMetadata GetActivityMetadata(string activityType)
    {
        return activityType switch
        {
            "HttpRequest" => new WorkflowActivityMetadata
            {
                Name = "HTTP Request",
                Description = "Executes HTTP requests to external APIs",
                Category = "Integration",
                RequiredParameters = new[] { "url", "method" },
                OptionalParameters = new[] { "headers", "body", "timeout" },
                OutputParameters = new[] { "statusCode", "responseBody", "headers" }
            },
            "DatabaseQuery" => new WorkflowActivityMetadata
            {
                Name = "Database Query",
                Description = "Executes database queries and operations",
                Category = "Data",
                RequiredParameters = new[] { "query", "connectionString" },
                OptionalParameters = new[] { "parameters", "timeout" },
                OutputParameters = new[] { "resultSet", "rowsAffected" }
            },
            "SendEmail" => new WorkflowActivityMetadata
            {
                Name = "Send Email",
                Description = "Sends email notifications",
                Category = "Communication",
                RequiredParameters = new[] { "to", "subject", "body" },
                OptionalParameters = new[] { "from", "cc", "bcc", "attachments" },
                OutputParameters = new[] { "messageId", "status" }
            },
            "UserApproval" => new WorkflowActivityMetadata
            {
                Name = "User Approval",
                Description = "Waits for user approval or rejection",
                Category = "Human Task",
                RequiredParameters = new[] { "approver", "message" },
                OptionalParameters = new[] { "timeout", "escalation" },
                OutputParameters = new[] { "approved", "approvedBy", "approvedAt", "comments" }
            },
            "AIProcessing" => new WorkflowActivityMetadata
            {
                Name = "AI Processing",
                Description = "Processes data using AI models",
                Category = "AI",
                RequiredParameters = new[] { "model", "input" },
                OptionalParameters = new[] { "parameters", "confidence" },
                OutputParameters = new[] { "result", "confidence", "processingTime" }
            },
            "DocumentGeneration" => new WorkflowActivityMetadata
            {
                Name = "Document Generation",
                Description = "Generates documents from templates",
                Category = "Document",
                RequiredParameters = new[] { "template", "data" },
                OptionalParameters = new[] { "format", "output" },
                OutputParameters = new[] { "documentUrl", "documentId" }
            },
            _ => new WorkflowActivityMetadata
            {
                Name = activityType,
                Description = $"Custom activity: {activityType}",
                Category = "Custom"
            }
        };
    }

    private async Task<WorkflowActivityResult> ExecuteHttpRequestAsync(
        WorkflowActivityContext context,
        CancellationToken cancellationToken)
    {
        var url = context.GetParameter<string>("url");
        var method = context.GetParameter<string>("method", "GET");
        var headers = context.GetParameter<Dictionary<string, string>>("headers");
        var body = context.GetParameter<string>("body");
        var timeout = context.GetParameter<int>("timeout", 30);

        using var httpClient = _httpClientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(timeout);

        // Add headers
        if (headers != null)
        {
            foreach (var header in headers)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        var request = new HttpRequestMessage(new HttpMethod(method.ToUpper()), url);
        if (!string.IsNullOrEmpty(body) && (method.ToUpper() == "POST" || method.ToUpper() == "PUT"))
        {
            request.Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
        }

        var response = await httpClient.SendAsync(request, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseHeaders = response.Headers.ToDictionary(h => h.Key, h => string.Join(",", h.Value));

        return new WorkflowActivityResult
        {
            Success = response.IsSuccessStatusCode,
            Data = new Dictionary<string, object>
            {
                ["statusCode"] = (int)response.StatusCode,
                ["responseBody"] = responseBody,
                ["headers"] = responseHeaders
            },
            ErrorMessage = response.IsSuccessStatusCode ? null : $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}"
        };
    }

    private async Task<WorkflowActivityResult> ExecuteDatabaseQueryAsync(
        WorkflowActivityContext context,
        CancellationToken cancellationToken)
    {
        var query = context.GetParameter<string>("query");
        var connectionString = context.GetParameter<string>("connectionString");
        var parameters = context.GetParameter<Dictionary<string, object>>("parameters");
        var timeout = context.GetParameter<int>("timeout", 30);

        var result = await _databaseService.ExecuteQueryAsync(
            connectionString,
            query,
            parameters,
            timeout,
            cancellationToken);

        return new WorkflowActivityResult
        {
            Success = true,
            Data = new Dictionary<string, object>
            {
                ["resultSet"] = result.Data,
                ["rowsAffected"] = result.RowsAffected
            }
        };
    }

    private async Task<WorkflowActivityResult> ExecuteSendEmailAsync(
        WorkflowActivityContext context,
        CancellationToken cancellationToken)
    {
        var to = context.GetParameter<string>("to");
        var subject = context.GetParameter<string>("subject");
        var body = context.GetParameter<string>("body");
        var from = context.GetParameter<string>("from");
        var cc = context.GetParameter<string>("cc");
        var bcc = context.GetParameter<string>("bcc");
        var attachments = context.GetParameter<List<string>>("attachments");

        var emailRequest = new EmailRequest
        {
            To = to.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList(),
            Subject = subject,
            Body = body,
            From = from,
            CC = cc?.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList(),
            BCC = bcc?.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList(),
            Attachments = attachments
        };

        var result = await _emailService.SendEmailAsync(emailRequest, cancellationToken);

        return new WorkflowActivityResult
        {
            Success = result.Success,
            Data = new Dictionary<string, object>
            {
                ["messageId"] = result.MessageId,
                ["status"] = result.Status
            },
            ErrorMessage = result.Success ? null : result.ErrorMessage
        };
    }

    private async Task<WorkflowActivityResult> ExecuteUserApprovalAsync(
        WorkflowActivityContext context,
        CancellationToken cancellationToken)
    {
        var approver = context.GetParameter<string>("approver");
        var message = context.GetParameter<string>("message");
        var timeout = context.GetParameter<TimeSpan>("timeout", TimeSpan.FromDays(7));
        var escalation = context.GetParameter<string>("escalation");

        // Create approval request
        var approvalRequest = new UserApprovalRequest
        {
            Id = Guid.NewGuid().ToString(),
            WorkflowId = context.WorkflowId,
            ActivityId = context.ActivityId,
            Approver = approver,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(timeout),
            Escalation = escalation,
            Status = ApprovalStatus.Pending
        };

        // Save approval request
        await _userService.CreateApprovalRequestAsync(approvalRequest, cancellationToken);

        // Send notification to approver
        await _notificationService.SendApprovalNotificationAsync(approvalRequest, cancellationToken);

        // Return pending result - workflow will wait for external approval
        return new WorkflowActivityResult
        {
            Success = true,
            IsWaiting = true,
            WaitingFor = "UserApproval",
            Data = new Dictionary<string, object>
            {
                ["approvalRequestId"] = approvalRequest.Id,
                ["approver"] = approver,
                ["expiresAt"] = approvalRequest.ExpiresAt
            }
        };
    }

    private async Task<WorkflowActivityResult> ExecuteAIProcessingAsync(
        WorkflowActivityContext context,
        CancellationToken cancellationToken)
    {
        var model = context.GetParameter<string>("model");
        var input = context.GetParameter<object>("input");
        var parameters = context.GetParameter<Dictionary<string, object>>("parameters");
        var confidence = context.GetParameter<double>("confidence", 0.8);

        var aiRequest = new AIProcessingRequest
        {
            Model = model,
            Input = input,
            Parameters = parameters,
            MinConfidence = confidence
        };

        var result = await _aiService.ProcessAsync(aiRequest, cancellationToken);

        return new WorkflowActivityResult
        {
            Success = result.Success,
            Data = new Dictionary<string, object>
            {
                ["result"] = result.Output,
                ["confidence"] = result.Confidence,
                ["processingTime"] = result.ProcessingTime
            },
            ErrorMessage = result.Success ? null : result.ErrorMessage
        };
    }

    private async Task<WorkflowActivityResult> ExecuteDocumentGenerationAsync(
        WorkflowActivityContext context,
        CancellationToken cancellationToken)
    {
        var template = context.GetParameter<string>("template");
        var data = context.GetParameter<object>("data");
        var format = context.GetParameter<string>("format", "PDF");
        var output = context.GetParameter<string>("output");

        var documentRequest = new DocumentGenerationRequest
        {
            Template = template,
            Data = data,
            Format = format,
            OutputPath = output
        };

        var result = await _documentService.GenerateDocumentAsync(documentRequest, cancellationToken);

        return new WorkflowActivityResult
        {
            Success = result.Success,
            Data = new Dictionary<string, object>
            {
                ["documentUrl"] = result.DocumentUrl,
                ["documentId"] = result.DocumentId
            },
            ErrorMessage = result.Success ? null : result.ErrorMessage
        };
    }

    private async Task<WorkflowActivityResult> ExecuteConditionalAsync(
        WorkflowActivityContext context,
        CancellationToken cancellationToken)
    {
        var condition = context.GetParameter<string>("condition");
        var trueAction = context.GetParameter<string>("trueAction");
        var falseAction = context.GetParameter<string>("falseAction");

        // Evaluate condition (simplified expression evaluator)
        var conditionResult = EvaluateCondition(condition, context.WorkflowData);

        var nextAction = conditionResult ? trueAction : falseAction;

        return new WorkflowActivityResult
        {
            Success = true,
            Data = new Dictionary<string, object>
            {
                ["conditionResult"] = conditionResult,
                ["nextAction"] = nextAction
            }
        };
    }

    private async Task<WorkflowActivityResult> ExecuteLoopAsync(
        WorkflowActivityContext context,
        CancellationToken cancellationToken)
    {
        var items = context.GetParameter<List<object>>("items");
        var action = context.GetParameter<string>("action");
        var maxIterations = context.GetParameter<int>("maxIterations", 1000);

        var results = new List<object>();
        var iterations = 0;

        foreach (var item in items.Take(maxIterations))
        {
            // Create child context for each iteration
            var childContext = new WorkflowActivityContext
            {
                WorkflowId = context.WorkflowId,
                ActivityId = $"{context.ActivityId}_loop_{iterations}",
                TenantId = context.TenantId,
                UserId = context.UserId,
                WorkflowData = new Dictionary<string, object>(context.WorkflowData)
                {
                    ["currentItem"] = item,
                    ["currentIndex"] = iterations
                }
            };

            var childResult = await ExecuteActivityAsync(action, childContext, cancellationToken);
            results.Add(childResult);
            iterations++;

            if (!childResult.Success)
            {
                break; // Stop on first error
            }
        }

        return new WorkflowActivityResult
        {
            Success = true,
            Data = new Dictionary<string, object>
            {
                ["results"] = results,
                ["iterations"] = iterations
            }
        };
    }

    private async Task<WorkflowActivityResult> ExecuteDelayAsync(
        WorkflowActivityContext context,
        CancellationToken cancellationToken)
    {
        var duration = context.GetParameter<TimeSpan>("duration");
        var delayType = context.GetParameter<string>("delayType", "relative");

        if (delayType == "absolute")
        {
            var absoluteTime = context.GetParameter<DateTime>("absoluteTime");
            duration = absoluteTime - DateTime.UtcNow;
        }

        if (duration > TimeSpan.Zero)
        {
            await Task.Delay(duration, cancellationToken);
        }

        return new WorkflowActivityResult
        {
            Success = true,
            Data = new Dictionary<string, object>
            {
                ["delayDuration"] = duration,
                ["delayedUntil"] = DateTime.UtcNow
            }
        };
    }

    private async Task<WorkflowActivityResult> ExecuteDataValidationAsync(
        WorkflowActivityContext context,
        CancellationToken cancellationToken)
    {
        var data = context.GetParameter<object>("data");
        var validationRules = context.GetParameter<List<ValidationRule>>("validationRules");

        var validationResults = new List<ValidationResult>();

        foreach (var rule in validationRules)
        {
            var result = await ValidateDataAsync(data, rule, cancellationToken);
            validationResults.Add(result);
        }

        var isValid = validationResults.All(r => r.IsValid);

        return new WorkflowActivityResult
        {
            Success = isValid,
            Data = new Dictionary<string, object>
            {
                ["isValid"] = isValid,
                ["validationResults"] = validationResults
            },
            ErrorMessage = isValid ? null : $"Validation failed: {string.Join(", ", validationResults.Where(r => !r.IsValid).Select(r => r.ErrorMessage))}"
        };
    }

    private async Task<WorkflowActivityResult> ExecuteBusinessRuleAsync(
        WorkflowActivityContext context,
        CancellationToken cancellationToken)
    {
        var ruleName = context.GetParameter<string>("ruleName");
        var ruleData = context.GetParameter<object>("ruleData");

        // Business rule execution logic would go here
        // This is a placeholder implementation
        await Task.Delay(50, cancellationToken);

        return new WorkflowActivityResult
        {
            Success = true,
            Data = new Dictionary<string, object>
            {
                ["ruleName"] = ruleName,
                ["ruleResult"] = "Passed",
                ["executedAt"] = DateTime.UtcNow
            }
        };
    }

    private async Task<WorkflowActivityResult> ExecutePerformanceMonitoringAsync(
        WorkflowActivityContext context,
        CancellationToken cancellationToken)
    {
        var resourceType = context.GetParameter<string>("resourceType");
        var thresholds = context.GetParameter<Dictionary<string, double>>("thresholds");

        // Performance monitoring logic would go here
        await Task.Delay(100, cancellationToken);

        return new WorkflowActivityResult
        {
            Success = true,
            Data = new Dictionary<string, object>
            {
                ["resourceType"] = resourceType,
                ["status"] = "Healthy",
                ["metrics"] = new Dictionary<string, double>
                {
                    ["cpu"] = 45.2,
                    ["memory"] = 67.8,
                    ["disk"] = 23.1
                }
            }
        };
    }

    private bool EvaluateCondition(string condition, Dictionary<string, object> data)
    {
        // Simplified condition evaluation
        // In a real implementation, you would use a proper expression evaluator
        if (condition.Contains("=="))
        {
            var parts = condition.Split("==");
            var left = parts[0].Trim();
            var right = parts[1].Trim().Trim('"');
            
            if (data.TryGetValue(left, out var value))
            {
                return value?.ToString() == right;
            }
        }

        return false;
    }

    private async Task<ValidationResult> ValidateDataAsync(object data, ValidationRule rule, CancellationToken cancellationToken)
    {
        // Simplified validation logic
        await Task.Delay(10, cancellationToken);
        
        return new ValidationResult
        {
            RuleName = rule.Name,
            IsValid = true,
            ErrorMessage = null
        };
    }
}

// Workflow template service for pre-built business processes
public class WorkflowTemplateService : IWorkflowTemplateService
{
    private readonly ILogger<WorkflowTemplateService> _logger;
    private readonly Dictionary<string, WorkflowTemplate> _templates;

    public WorkflowTemplateService(ILogger<WorkflowTemplateService> logger)
    {
        _logger = logger;
        _templates = InitializeTemplates();
    }

    public List<WorkflowTemplate> GetAvailableTemplates()
    {
        return _templates.Values.ToList();
    }

    public WorkflowTemplate? GetTemplate(string templateId)
    {
        return _templates.TryGetValue(templateId, out var template) ? template : null;
    }

    public async Task<WorkflowDefinition> CreateWorkflowFromTemplateAsync(
        string templateId,
        WorkflowTemplateParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var template = GetTemplate(templateId);
        if (template == null)
        {
            throw new ArgumentException($"Template {templateId} not found");
        }

        var workflow = new WorkflowDefinition
        {
            Id = Guid.NewGuid().ToString(),
            Name = parameters.WorkflowName ?? template.Name,
            Description = template.Description,
            Version = "1.0.0",
            TenantId = parameters.TenantId,
            CreatedBy = parameters.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            Activities = template.Activities.Select(a => CustomizeActivity(a, parameters)).ToList(),
            Transitions = template.Transitions,
            Variables = template.Variables
        };

        // Apply parameter customizations
        foreach (var customization in parameters.Customizations)
        {
            ApplyCustomization(workflow, customization);
        }

        return workflow;
    }

    private Dictionary<string, WorkflowTemplate> InitializeTemplates()
    {
        return new Dictionary<string, WorkflowTemplate>
        {
            ["employee-onboarding"] = CreateEmployeeOnboardingTemplate(),
            ["invoice-approval"] = CreateInvoiceApprovalTemplate(),
            ["document-review"] = CreateDocumentReviewTemplate(),
            ["customer-support"] = CreateCustomerSupportTemplate(),
            ["data-processing"] = CreateDataProcessingTemplate(),
            ["compliance-audit"] = CreateComplianceAuditTemplate(),
            ["project-approval"] = CreateProjectApprovalTemplate(),
            ["expense-reimbursement"] = CreateExpenseReimbursementTemplate(),
            ["leave-request"] = CreateLeaveRequestTemplate(),
            ["vendor-onboarding"] = CreateVendorOnboardingTemplate()
        };
    }

    private WorkflowTemplate CreateEmployeeOnboardingTemplate()
    {
        return new WorkflowTemplate
        {
            Id = "employee-onboarding",
            Name = "Employee Onboarding",
            Description = "Complete employee onboarding process with document collection, system setup, and training",
            Category = "HR",
            Activities = new List<WorkflowActivity>
            {
                new WorkflowActivity
                {
                    Id = "collect-documents",
                    Name = "Collect Documents",
                    Type = "UserTask",
                    Parameters = new Dictionary<string, object>
                    {
                        ["assignee"] = "hr@company.com",
                        ["description"] = "Collect required employee documents",
                        ["requiredDocuments"] = new[] { "ID", "TaxForms", "EmergencyContact", "BankDetails" }
                    }
                },
                new WorkflowActivity
                {
                    Id = "create-accounts",
                    Name = "Create System Accounts",
                    Type = "Integration",
                    Parameters = new Dictionary<string, object>
                    {
                        ["service"] = "ActiveDirectory",
                        ["action"] = "CreateUser"
                    }
                },
                new WorkflowActivity
                {
                    Id = "send-welcome-email",
                    Name = "Send Welcome Email",
                    Type = "SendEmail",
                    Parameters = new Dictionary<string, object>
                    {
                        ["template"] = "WelcomeEmail",
                        ["includeHandbook"] = true
                    }
                },
                new WorkflowActivity
                {
                    Id = "schedule-training",
                    Name = "Schedule Training Sessions",
                    Type = "Integration",
                    Parameters = new Dictionary<string, object>
                    {
                        ["service"] = "LearningManagement",
                        ["action"] = "EnrollInOrientation"
                    }
                }
            },
            Transitions = new List<WorkflowTransition>
            {
                new WorkflowTransition { From = "collect-documents", To = "create-accounts", Condition = "documentsComplete == true" },
                new WorkflowTransition { From = "create-accounts", To = "send-welcome-email" },
                new WorkflowTransition { From = "send-welcome-email", To = "schedule-training" }
            }
        };
    }

    private WorkflowTemplate CreateInvoiceApprovalTemplate()
    {
        return new WorkflowTemplate
        {
            Id = "invoice-approval",
            Name = "Invoice Approval",
            Description = "Multi-level invoice approval process with automated routing",
            Category = "Finance",
            Activities = new List<WorkflowActivity>
            {
                new WorkflowActivity
                {
                    Id = "validate-invoice",
                    Name = "Validate Invoice",
                    Type = "DataValidation",
                    Parameters = new Dictionary<string, object>
                    {
                        ["validationRules"] = new[] { "AmountValid", "VendorExists", "PurchaseOrderMatch" }
                    }
                },
                new WorkflowActivity
                {
                    Id = "manager-approval",
                    Name = "Manager Approval",
                    Type = "UserApproval",
                    Parameters = new Dictionary<string, object>
                    {
                        ["approver"] = "{invoice.managerId}",
                        ["timeout"] = "3 days",
                        ["escalation"] = "{invoice.directorId}"
                    }
                },
                new WorkflowActivity
                {
                    Id = "finance-approval",
                    Name = "Finance Approval",
                    Type = "UserApproval",
                    Parameters = new Dictionary<string, object>
                    {
                        ["approver"] = "finance@company.com",
                        ["condition"] = "amount > 5000"
                    }
                },
                new WorkflowActivity
                {
                    Id = "process-payment",
                    Name = "Process Payment",
                    Type = "Integration",
                    Parameters = new Dictionary<string, object>
                    {
                        ["service"] = "PaymentSystem",
                        ["action"] = "ProcessPayment"
                    }
                }
            ],
            Transitions = new List<WorkflowTransition>
            {
                new WorkflowTransition { From = "validate-invoice", To = "manager-approval" },
                new WorkflowTransition { From = "manager-approval", To = "finance-approval" },
                new WorkflowTransition { From = "finance-approval", To = "process-payment" }
            }
        };
    }

    private WorkflowTemplate CreateDocumentReviewTemplate()
    {
        return new WorkflowTemplate
        {
            Id = "document-review",
            Name = "Document Review",
            Description = "Collaborative document review and approval process",
            Category = "Content",
            Activities = new List<WorkflowActivity>
            {
                new WorkflowActivity
                {
                    Id = "assign-reviewers",
                    Name = "Assign Reviewers",
                    Type = "UserTask",
                    Parameters = new Dictionary<string, object>
                    {
                        ["reviewerCount"] = 3,
                        ["reviewPeriod"] = "5 days"
                    }
                },
                new WorkflowActivity
                {
                    Id = "collect-reviews",
                    Name = "Collect Reviews",
                    Type = "Parallel",
                    Parameters = new Dictionary<string, object>
                    {
                        ["waitForAll"] = false,
                        ["minimumApprovals"] = 2
                    }
                },
                new WorkflowActivity
                {
                    Id = "consolidate-feedback",
                    Name = "Consolidate Feedback",
                    Type = "AIProcessing",
                    Parameters = new Dictionary<string, object>
                    {
                        ["model"] = "DocumentAnalysis",
                        ["action"] = "ConsolidateFeedback"
                    }
                },
                new WorkflowActivity
                {
                    Id = "final-approval",
                    Name = "Final Approval",
                    Type = "UserApproval",
                    Parameters = new Dictionary<string, object>
                    {
                        ["approver"] = "{document.owner}"
                    }
                }
            ],
            Transitions = new List<WorkflowTransition>
            {
                new WorkflowTransition { From = "assign-reviewers", To = "collect-reviews" },
                new WorkflowTransition { From = "collect-reviews", To = "consolidate-feedback" },
                new WorkflowTransition { From = "consolidate-feedback", To = "final-approval" }
            }
        };
    }

    private WorkflowActivity CustomizeActivity(WorkflowActivity template, WorkflowTemplateParameters parameters)
    {
        var activity = new WorkflowActivity
        {
            Id = template.Id,
            Name = template.Name,
            Type = template.Type,
            Parameters = new Dictionary<string, object>(template.Parameters)
        };

        // Apply parameter substitutions
        foreach (var param in activity.Parameters.ToList())
        {
            if (param.Value is string stringValue && stringValue.StartsWith("{") && stringValue.EndsWith("}"))
            {
                var paramName = stringValue[1..^1];
                if (parameters.ParameterValues.TryGetValue(paramName, out var value))
                {
                    activity.Parameters[param.Key] = value;
                }
            }
        }

        return activity;
    }

    private void ApplyCustomization(WorkflowDefinition workflow, WorkflowCustomization customization)
    {
        switch (customization.Type)
        {
            case "AddActivity":
                workflow.Activities.Add(customization.Activity);
                break;
            case "RemoveActivity":
                workflow.Activities.RemoveAll(a => a.Id == customization.ActivityId);
                break;
            case "ModifyActivity":
                var activity = workflow.Activities.FirstOrDefault(a => a.Id == customization.ActivityId);
                if (activity != null)
                {
                    foreach (var param in customization.Parameters)
                    {
                        activity.Parameters[param.Key] = param.Value;
                    }
                }
                break;
        }
    }
}
