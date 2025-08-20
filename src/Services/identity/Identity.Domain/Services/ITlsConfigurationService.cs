namespace Identity.Domain.Services;

public interface ITlsConfigurationService
{
    bool IsTlsEnabled { get; }
    string MinimumTlsVersion { get; }
    bool RequireClientCertificate { get; }
    bool ValidateCertificateRevocation { get; }
    Task<bool> ValidateCertificateAsync(string certificateThumbprint, CancellationToken cancellationToken = default);
    Task<bool> IsCertificateValidAsync(string certificateThumbprint, CancellationToken cancellationToken = default);
}
