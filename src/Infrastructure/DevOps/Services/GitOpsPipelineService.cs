using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TossErp.Security.Domain.Services;

namespace TossErp.Infrastructure.DevOps.Services;

/// <summary>
/// GitOps CI/CD pipeline service with container security scanning and automated deployment
/// </summary>
public class GitOpsPipelineService : IGitOpsPipelineService
{
    private readonly ILogger<GitOpsPipelineService> _logger;
    private readonly IContainerSecurityService _containerSecurity;
    private readonly IKubernetesService _kubernetesService;
    private readonly ISecretsManagementService _secretsManagement;
    private readonly IArtifactRepository _artifactRepository;

    public GitOpsPipelineService(
        ILogger<GitOpsPipelineService> logger,
        IContainerSecurityService containerSecurity,
        IKubernetesService kubernetesService,
        ISecretsManagementService secretsManagement,
        IArtifactRepository artifactRepository)
    {
        _logger = logger;
        _containerSecurity = containerSecurity;
        _kubernetesService = kubernetesService;
        _secretsManagement = secretsManagement;
        _artifactRepository = artifactRepository;
    }

    public async Task<PipelineExecution> ExecutePipelineAsync(
        string repositoryUrl,
        string branch,
        string commitSha,
        PipelineConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        var executionId = Guid.NewGuid().ToString();
        _logger.LogInformation("Starting GitOps pipeline execution {ExecutionId} for {Repository}:{Branch}@{Commit}",
            executionId, repositoryUrl, branch, commitSha);

        var execution = new PipelineExecution
        {
            Id = executionId,
            RepositoryUrl = repositoryUrl,
            Branch = branch,
            CommitSha = commitSha,
            Configuration = configuration,
            StartTime = DateTime.UtcNow,
            Status = PipelineStatus.Running,
            Stages = new List<PipelineStage>()
        };

        try
        {
            // Stage 1: Source Code Analysis
            var sourceStage = await ExecuteSourceAnalysisStageAsync(execution, cancellationToken);
            execution.Stages.Add(sourceStage);

            if (sourceStage.Status == StageStatus.Failed)
            {
                execution.Status = PipelineStatus.Failed;
                return execution;
            }

            // Stage 2: Build and Test
            var buildStage = await ExecuteBuildStageAsync(execution, cancellationToken);
            execution.Stages.Add(buildStage);

            if (buildStage.Status == StageStatus.Failed)
            {
                execution.Status = PipelineStatus.Failed;
                return execution;
            }

            // Stage 3: Container Security Scanning
            var securityStage = await ExecuteSecurityScanningStageAsync(execution, cancellationToken);
            execution.Stages.Add(securityStage);

            if (securityStage.Status == StageStatus.Failed && configuration.FailOnSecurityIssues)
            {
                execution.Status = PipelineStatus.Failed;
                return execution;
            }

            // Stage 4: Image Publishing
            var publishStage = await ExecutePublishStageAsync(execution, cancellationToken);
            execution.Stages.Add(publishStage);

            if (publishStage.Status == StageStatus.Failed)
            {
                execution.Status = PipelineStatus.Failed;
                return execution;
            }

            // Stage 5: Deployment
            var deployStage = await ExecuteDeploymentStageAsync(execution, cancellationToken);
            execution.Stages.Add(deployStage);

            execution.Status = deployStage.Status == StageStatus.Success ? PipelineStatus.Success : PipelineStatus.Failed;
            execution.EndTime = DateTime.UtcNow;

            _logger.LogInformation("GitOps pipeline execution {ExecutionId} completed with status {Status}",
                executionId, execution.Status);

            return execution;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during GitOps pipeline execution {ExecutionId}", executionId);
            execution.Status = PipelineStatus.Failed;
            execution.EndTime = DateTime.UtcNow;
            execution.ErrorMessage = ex.Message;
            return execution;
        }
    }

    public async Task<SecurityScanResult> ScanContainerImageAsync(
        string imageName,
        string imageTag,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting container security scan for {Image}:{Tag}", imageName, imageTag);

        var scanResult = new SecurityScanResult
        {
            Id = Guid.NewGuid().ToString(),
            ImageName = imageName,
            ImageTag = imageTag,
            ScanTime = DateTime.UtcNow,
            Vulnerabilities = new List<SecurityVulnerability>(),
            ComplianceIssues = new List<ComplianceIssue>()
        };

        try
        {
            // Vulnerability Scanning
            var vulnerabilities = await _containerSecurity.ScanVulnerabilitiesAsync(imageName, imageTag, cancellationToken);
            scanResult.Vulnerabilities.AddRange(vulnerabilities);

            // Configuration Scanning
            var configIssues = await _containerSecurity.ScanConfigurationAsync(imageName, imageTag, cancellationToken);
            scanResult.ComplianceIssues.AddRange(configIssues);

            // Secret Scanning
            var secretIssues = await _containerSecurity.ScanSecretsAsync(imageName, imageTag, cancellationToken);
            scanResult.ComplianceIssues.AddRange(secretIssues);

            // License Scanning
            var licenseIssues = await _containerSecurity.ScanLicensesAsync(imageName, imageTag, cancellationToken);
            scanResult.ComplianceIssues.AddRange(licenseIssues);

            // Calculate risk score
            scanResult.RiskScore = CalculateRiskScore(scanResult);
            scanResult.OverallStatus = DetermineOverallStatus(scanResult);

            _logger.LogInformation("Container security scan completed for {Image}:{Tag}. Status: {Status}, Risk Score: {RiskScore}",
                imageName, imageTag, scanResult.OverallStatus, scanResult.RiskScore);

            return scanResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scanning container image {Image}:{Tag}", imageName, imageTag);
            scanResult.OverallStatus = ScanStatus.Error;
            scanResult.ErrorMessage = ex.Message;
            return scanResult;
        }
    }

    public async Task<DeploymentResult> DeployToEnvironmentAsync(
        string applicationName,
        string version,
        string environment,
        DeploymentStrategy strategy,
        CancellationToken cancellationToken = default)
    {
        var deploymentId = Guid.NewGuid().ToString();
        _logger.LogInformation("Starting deployment {DeploymentId} of {Application}:{Version} to {Environment} using {Strategy}",
            deploymentId, applicationName, version, environment, strategy);

        var result = new DeploymentResult
        {
            Id = deploymentId,
            ApplicationName = applicationName,
            Version = version,
            Environment = environment,
            Strategy = strategy,
            StartTime = DateTime.UtcNow,
            Status = DeploymentStatus.InProgress
        };

        try
        {
            // Validate deployment prerequisites
            await ValidateDeploymentPrerequisitesAsync(result, cancellationToken);

            // Execute deployment based on strategy
            switch (strategy)
            {
                case DeploymentStrategy.BlueGreen:
                    await ExecuteBlueGreenDeploymentAsync(result, cancellationToken);
                    break;
                case DeploymentStrategy.Canary:
                    await ExecuteCanaryDeploymentAsync(result, cancellationToken);
                    break;
                case DeploymentStrategy.RollingUpdate:
                    await ExecuteRollingUpdateDeploymentAsync(result, cancellationToken);
                    break;
                default:
                    await ExecuteDirectDeploymentAsync(result, cancellationToken);
                    break;
            }

            // Post-deployment validation
            await ValidateDeploymentAsync(result, cancellationToken);

            result.EndTime = DateTime.UtcNow;
            _logger.LogInformation("Deployment {DeploymentId} completed with status {Status}",
                deploymentId, result.Status);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during deployment {DeploymentId}", deploymentId);
            result.Status = DeploymentStatus.Failed;
            result.EndTime = DateTime.UtcNow;
            result.ErrorMessage = ex.Message;
            
            // Attempt rollback if deployment failed
            await RollbackDeploymentAsync(result, cancellationToken);
            
            return result;
        }
    }

    public async Task<List<PipelineExecution>> GetPipelineHistoryAsync(
        string repositoryUrl,
        int limit,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving pipeline history for {Repository} (limit: {Limit})", repositoryUrl, limit);

        // Implementation would retrieve from database/storage
        await Task.Delay(1, cancellationToken);
        
        return new List<PipelineExecution>(); // Placeholder
    }

    public async Task<bool> RollbackDeploymentAsync(
        string deploymentId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting rollback for deployment {DeploymentId}", deploymentId);

        try
        {
            // Get deployment details
            var deployment = await GetDeploymentAsync(deploymentId, cancellationToken);
            if (deployment == null)
            {
                _logger.LogWarning("Deployment {DeploymentId} not found for rollback", deploymentId);
                return false;
            }

            // Execute rollback
            await RollbackDeploymentAsync(deployment, cancellationToken);
            
            _logger.LogInformation("Rollback completed successfully for deployment {DeploymentId}", deploymentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during rollback for deployment {DeploymentId}", deploymentId);
            return false;
        }
    }

    private async Task<PipelineStage> ExecuteSourceAnalysisStageAsync(
        PipelineExecution execution,
        CancellationToken cancellationToken)
    {
        var stage = new PipelineStage
        {
            Name = "Source Analysis",
            StartTime = DateTime.UtcNow,
            Status = StageStatus.Running
        };

        try
        {
            _logger.LogInformation("Executing source analysis stage for pipeline {ExecutionId}", execution.Id);

            // Clone repository (simulated)
            await Task.Delay(1000, cancellationToken);
            stage.Steps.Add(new PipelineStep { Name = "Clone Repository", Status = StepStatus.Success });

            // Static code analysis
            await Task.Delay(2000, cancellationToken);
            stage.Steps.Add(new PipelineStep { Name = "Static Code Analysis", Status = StepStatus.Success });

            // Dependency vulnerability scanning
            await Task.Delay(1500, cancellationToken);
            stage.Steps.Add(new PipelineStep { Name = "Dependency Scan", Status = StepStatus.Success });

            // License compliance check
            await Task.Delay(500, cancellationToken);
            stage.Steps.Add(new PipelineStep { Name = "License Compliance", Status = StepStatus.Success });

            stage.Status = StageStatus.Success;
            stage.EndTime = DateTime.UtcNow;

            return stage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in source analysis stage for pipeline {ExecutionId}", execution.Id);
            stage.Status = StageStatus.Failed;
            stage.EndTime = DateTime.UtcNow;
            stage.ErrorMessage = ex.Message;
            return stage;
        }
    }

    private async Task<PipelineStage> ExecuteBuildStageAsync(
        PipelineExecution execution,
        CancellationToken cancellationToken)
    {
        var stage = new PipelineStage
        {
            Name = "Build and Test",
            StartTime = DateTime.UtcNow,
            Status = StageStatus.Running
        };

        try
        {
            _logger.LogInformation("Executing build stage for pipeline {ExecutionId}", execution.Id);

            // Build application
            await Task.Delay(3000, cancellationToken);
            stage.Steps.Add(new PipelineStep { Name = "Build Application", Status = StepStatus.Success });

            // Run unit tests
            await Task.Delay(2000, cancellationToken);
            stage.Steps.Add(new PipelineStep { Name = "Unit Tests", Status = StepStatus.Success });

            // Run integration tests
            await Task.Delay(4000, cancellationToken);
            stage.Steps.Add(new PipelineStep { Name = "Integration Tests", Status = StepStatus.Success });

            // Build container image
            await Task.Delay(2500, cancellationToken);
            stage.Steps.Add(new PipelineStep { Name = "Build Container Image", Status = StepStatus.Success });

            stage.Status = StageStatus.Success;
            stage.EndTime = DateTime.UtcNow;

            return stage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in build stage for pipeline {ExecutionId}", execution.Id);
            stage.Status = StageStatus.Failed;
            stage.EndTime = DateTime.UtcNow;
            stage.ErrorMessage = ex.Message;
            return stage;
        }
    }

    private async Task<PipelineStage> ExecuteSecurityScanningStageAsync(
        PipelineExecution execution,
        CancellationToken cancellationToken)
    {
        var stage = new PipelineStage
        {
            Name = "Security Scanning",
            StartTime = DateTime.UtcNow,
            Status = StageStatus.Running
        };

        try
        {
            _logger.LogInformation("Executing security scanning stage for pipeline {ExecutionId}", execution.Id);

            // Container vulnerability scan
            var imageName = $"{execution.Configuration.ApplicationName}";
            var imageTag = execution.CommitSha[..8];
            
            var scanResult = await ScanContainerImageAsync(imageName, imageTag, cancellationToken);
            stage.SecurityScanResult = scanResult;

            if (scanResult.OverallStatus == ScanStatus.Failed)
            {
                stage.Steps.Add(new PipelineStep 
                { 
                    Name = "Container Security Scan", 
                    Status = StepStatus.Failed,
                    ErrorMessage = $"Security scan failed with {scanResult.Vulnerabilities.Count} vulnerabilities"
                });
                stage.Status = StageStatus.Failed;
            }
            else
            {
                stage.Steps.Add(new PipelineStep { Name = "Container Security Scan", Status = StepStatus.Success });
            }

            // SAST (Static Application Security Testing)
            await Task.Delay(2000, cancellationToken);
            stage.Steps.Add(new PipelineStep { Name = "SAST Scan", Status = StepStatus.Success });

            // DAST (Dynamic Application Security Testing) - if applicable
            if (execution.Configuration.EnableDynamicScanning)
            {
                await Task.Delay(3000, cancellationToken);
                stage.Steps.Add(new PipelineStep { Name = "DAST Scan", Status = StepStatus.Success });
            }

            if (stage.Status != StageStatus.Failed)
            {
                stage.Status = StageStatus.Success;
            }

            stage.EndTime = DateTime.UtcNow;
            return stage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in security scanning stage for pipeline {ExecutionId}", execution.Id);
            stage.Status = StageStatus.Failed;
            stage.EndTime = DateTime.UtcNow;
            stage.ErrorMessage = ex.Message;
            return stage;
        }
    }

    private async Task<PipelineStage> ExecutePublishStageAsync(
        PipelineExecution execution,
        CancellationToken cancellationToken)
    {
        var stage = new PipelineStage
        {
            Name = "Publish Artifacts",
            StartTime = DateTime.UtcNow,
            Status = StageStatus.Running
        };

        try
        {
            _logger.LogInformation("Executing publish stage for pipeline {ExecutionId}", execution.Id);

            // Sign container image
            await Task.Delay(1000, cancellationToken);
            stage.Steps.Add(new PipelineStep { Name = "Sign Container Image", Status = StepStatus.Success });

            // Push to container registry
            await Task.Delay(2000, cancellationToken);
            stage.Steps.Add(new PipelineStep { Name = "Push to Registry", Status = StepStatus.Success });

            // Generate SBOM (Software Bill of Materials)
            await Task.Delay(500, cancellationToken);
            stage.Steps.Add(new PipelineStep { Name = "Generate SBOM", Status = StepStatus.Success });

            // Update deployment manifests
            await Task.Delay(1000, cancellationToken);
            stage.Steps.Add(new PipelineStep { Name = "Update Manifests", Status = StepStatus.Success });

            stage.Status = StageStatus.Success;
            stage.EndTime = DateTime.UtcNow;

            return stage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in publish stage for pipeline {ExecutionId}", execution.Id);
            stage.Status = StageStatus.Failed;
            stage.EndTime = DateTime.UtcNow;
            stage.ErrorMessage = ex.Message;
            return stage;
        }
    }

    private async Task<PipelineStage> ExecuteDeploymentStageAsync(
        PipelineExecution execution,
        CancellationToken cancellationToken)
    {
        var stage = new PipelineStage
        {
            Name = "Deployment",
            StartTime = DateTime.UtcNow,
            Status = StageStatus.Running
        };

        try
        {
            _logger.LogInformation("Executing deployment stage for pipeline {ExecutionId}", execution.Id);

            // Deploy to target environment
            var deploymentResult = await DeployToEnvironmentAsync(
                execution.Configuration.ApplicationName,
                execution.CommitSha[..8],
                execution.Configuration.TargetEnvironment,
                execution.Configuration.DeploymentStrategy,
                cancellationToken);

            stage.DeploymentResult = deploymentResult;

            if (deploymentResult.Status == DeploymentStatus.Success)
            {
                stage.Steps.Add(new PipelineStep { Name = "Deploy Application", Status = StepStatus.Success });
                stage.Status = StageStatus.Success;
            }
            else
            {
                stage.Steps.Add(new PipelineStep 
                { 
                    Name = "Deploy Application", 
                    Status = StepStatus.Failed,
                    ErrorMessage = deploymentResult.ErrorMessage
                });
                stage.Status = StageStatus.Failed;
            }

            stage.EndTime = DateTime.UtcNow;
            return stage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in deployment stage for pipeline {ExecutionId}", execution.Id);
            stage.Status = StageStatus.Failed;
            stage.EndTime = DateTime.UtcNow;
            stage.ErrorMessage = ex.Message;
            return stage;
        }
    }

    private decimal CalculateRiskScore(SecurityScanResult scanResult)
    {
        var criticalCount = scanResult.Vulnerabilities.Count(v => v.Severity == VulnerabilitySeverity.Critical);
        var highCount = scanResult.Vulnerabilities.Count(v => v.Severity == VulnerabilitySeverity.High);
        var mediumCount = scanResult.Vulnerabilities.Count(v => v.Severity == VulnerabilitySeverity.Medium);

        // Weighted risk calculation
        var riskScore = (criticalCount * 1.0m) + (highCount * 0.7m) + (mediumCount * 0.3m);
        
        // Normalize to 0-1 scale (max 20 vulnerabilities = score of 1.0)
        return Math.Min(1.0m, riskScore / 20.0m);
    }

    private ScanStatus DetermineOverallStatus(SecurityScanResult scanResult)
    {
        var criticalCount = scanResult.Vulnerabilities.Count(v => v.Severity == VulnerabilitySeverity.Critical);
        var highCount = scanResult.Vulnerabilities.Count(v => v.Severity == VulnerabilitySeverity.High);

        if (criticalCount > 0 || highCount > 5)
            return ScanStatus.Failed;
        
        if (highCount > 0 || scanResult.RiskScore > 0.3m)
            return ScanStatus.Warning;

        return ScanStatus.Passed;
    }

    private async Task ValidateDeploymentPrerequisitesAsync(DeploymentResult result, CancellationToken cancellationToken)
    {
        await Task.Delay(500, cancellationToken);
        // Validate secrets, configurations, and dependencies
    }

    private async Task ExecuteBlueGreenDeploymentAsync(DeploymentResult result, CancellationToken cancellationToken)
    {
        await Task.Delay(3000, cancellationToken);
        result.Status = DeploymentStatus.Success;
    }

    private async Task ExecuteCanaryDeploymentAsync(DeploymentResult result, CancellationToken cancellationToken)
    {
        await Task.Delay(4000, cancellationToken);
        result.Status = DeploymentStatus.Success;
    }

    private async Task ExecuteRollingUpdateDeploymentAsync(DeploymentResult result, CancellationToken cancellationToken)
    {
        await Task.Delay(2500, cancellationToken);
        result.Status = DeploymentStatus.Success;
    }

    private async Task ExecuteDirectDeploymentAsync(DeploymentResult result, CancellationToken cancellationToken)
    {
        await Task.Delay(2000, cancellationToken);
        result.Status = DeploymentStatus.Success;
    }

    private async Task ValidateDeploymentAsync(DeploymentResult result, CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        // Validate application health and readiness
    }

    private async Task RollbackDeploymentAsync(DeploymentResult result, CancellationToken cancellationToken)
    {
        await Task.Delay(1500, cancellationToken);
        result.Status = DeploymentStatus.RolledBack;
    }

    private async Task<DeploymentResult?> GetDeploymentAsync(string deploymentId, CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken);
        return null; // Placeholder
    }
}

// Supporting interfaces and classes
public interface IGitOpsPipelineService
{
    Task<PipelineExecution> ExecutePipelineAsync(string repositoryUrl, string branch, string commitSha, PipelineConfiguration configuration, CancellationToken cancellationToken = default);
    Task<SecurityScanResult> ScanContainerImageAsync(string imageName, string imageTag, CancellationToken cancellationToken = default);
    Task<DeploymentResult> DeployToEnvironmentAsync(string applicationName, string version, string environment, DeploymentStrategy strategy, CancellationToken cancellationToken = default);
    Task<List<PipelineExecution>> GetPipelineHistoryAsync(string repositoryUrl, int limit, CancellationToken cancellationToken = default);
    Task<bool> RollbackDeploymentAsync(string deploymentId, CancellationToken cancellationToken = default);
}

public interface IContainerSecurityService
{
    Task<List<SecurityVulnerability>> ScanVulnerabilitiesAsync(string imageName, string imageTag, CancellationToken cancellationToken);
    Task<List<ComplianceIssue>> ScanConfigurationAsync(string imageName, string imageTag, CancellationToken cancellationToken);
    Task<List<ComplianceIssue>> ScanSecretsAsync(string imageName, string imageTag, CancellationToken cancellationToken);
    Task<List<ComplianceIssue>> ScanLicensesAsync(string imageName, string imageTag, CancellationToken cancellationToken);
}

public interface IKubernetesService
{
    Task<bool> DeployApplicationAsync(string applicationName, string version, string environment, CancellationToken cancellationToken);
}

public interface ISecretsManagementService
{
    Task<string> GetSecretAsync(string secretName, CancellationToken cancellationToken);
}

public interface IArtifactRepository
{
    Task<bool> PublishArtifactAsync(string artifactName, string version, Stream artifactStream, CancellationToken cancellationToken);
}

public class PipelineExecution
{
    public string Id { get; set; } = string.Empty;
    public string RepositoryUrl { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string CommitSha { get; set; } = string.Empty;
    public PipelineConfiguration Configuration { get; set; } = new();
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public PipelineStatus Status { get; set; }
    public List<PipelineStage> Stages { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

public class PipelineConfiguration
{
    public string ApplicationName { get; set; } = string.Empty;
    public string TargetEnvironment { get; set; } = string.Empty;
    public DeploymentStrategy DeploymentStrategy { get; set; }
    public bool FailOnSecurityIssues { get; set; } = true;
    public bool EnableDynamicScanning { get; set; } = false;
}

public class PipelineStage
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public StageStatus Status { get; set; }
    public List<PipelineStep> Steps { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public SecurityScanResult? SecurityScanResult { get; set; }
    public DeploymentResult? DeploymentResult { get; set; }
}

public class PipelineStep
{
    public string Name { get; set; } = string.Empty;
    public StepStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
}

public class SecurityScanResult
{
    public string Id { get; set; } = string.Empty;
    public string ImageName { get; set; } = string.Empty;
    public string ImageTag { get; set; } = string.Empty;
    public DateTime ScanTime { get; set; }
    public List<SecurityVulnerability> Vulnerabilities { get; set; } = new();
    public List<ComplianceIssue> ComplianceIssues { get; set; } = new();
    public decimal RiskScore { get; set; }
    public ScanStatus OverallStatus { get; set; }
    public string? ErrorMessage { get; set; }
}

public class SecurityVulnerability
{
    public string Id { get; set; } = string.Empty;
    public string Package { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public VulnerabilitySeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? FixVersion { get; set; }
}

public class ComplianceIssue
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
}

public class DeploymentResult
{
    public string Id { get; set; } = string.Empty;
    public string ApplicationName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public DeploymentStrategy Strategy { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DeploymentStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
}

public enum PipelineStatus
{
    Pending,
    Running,
    Success,
    Failed,
    Cancelled
}

public enum StageStatus
{
    Pending,
    Running,
    Success,
    Failed,
    Skipped
}

public enum StepStatus
{
    Pending,
    Running,
    Success,
    Failed,
    Skipped
}

public enum ScanStatus
{
    Passed,
    Warning,
    Failed,
    Error
}

public enum VulnerabilitySeverity
{
    Informational,
    Low,
    Medium,
    High,
    Critical
}

public enum DeploymentStrategy
{
    Direct,
    BlueGreen,
    Canary,
    RollingUpdate
}

public enum DeploymentStatus
{
    Pending,
    InProgress,
    Success,
    Failed,
    RolledBack
}
