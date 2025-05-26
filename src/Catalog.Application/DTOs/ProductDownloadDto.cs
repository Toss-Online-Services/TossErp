namespace Catalog.Application.DTOs;

public class ProductDownloadDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int DownloadId { get; set; }
    public string DownloadUrl { get; set; }
    public string DownloadFileName { get; set; }
    public int DownloadVersion { get; set; }
    public int DownloadCount { get; set; }
    public DateTime? DownloadExpirationDateUtc { get; set; }
    public int DownloadActivationTypeId { get; set; }
    public bool HasUserAgreement { get; set; }
    public string UserAgreementText { get; set; }
} 
