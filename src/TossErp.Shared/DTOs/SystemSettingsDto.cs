using TossErp.Shared.Enums;

namespace TossErp.Shared.DTOs;

public class SystemSettingsDto
{
    // General
    public string SystemName { get; set; } = "";
    public string SiteName { get; set; } = "";
    public string SiteUrl { get; set; } = "";
    public LanguageCode Language { get; set; } = LanguageCode.en_US;
    public AppTimeZone TimeZone { get; set; } = AppTimeZone.UTC;
    public string DateFormat { get; set; } = "";
    public string TimeFormat { get; set; } = "";
    public string Version { get; set; } = "";
    public Theme Theme { get; set; } = Theme.Light;
    public int SessionTimeout { get; set; } = 30;
    // Security
    public int PasswordMinLength { get; set; } = 8;
    public int PasswordExpiryDays { get; set; } = 90;
    public int SessionTimeoutMinutes { get; set; } = 30;
    public int MaxLoginAttempts { get; set; } = 5;
    public bool RequireTwoFactor { get; set; } = false;
    public bool EnableAuditLog { get; set; } = true;
    public bool ForceHTTPS { get; set; } = true;
    public bool EnableCSRFProtection { get; set; } = true;
    public bool EnableSsl { get; set; } = true;
    public bool RequireUppercase { get; set; } = true;
    public bool RequireLowercase { get; set; } = true;
    public bool RequireNumbers { get; set; } = true;
    public bool RequireSpecialChars { get; set; } = true;
    // Email
    public string SmtpServer { get; set; } = "";
    public int SmtpPort { get; set; } = 587;
    public string SmtpUsername { get; set; } = "";
    public string SmtpPassword { get; set; } = "";
    public string FromEmail { get; set; } = "";
    public string FromName { get; set; } = "";
    public bool EnableSSL { get; set; } = true;
    // Backup
    public bool AutoBackup { get; set; } = true;
    public bool EnableAutoBackup { get; set; } = true;
    public BackupFrequency BackupFrequency { get; set; } = BackupFrequency.Daily;
    public string BackupPath { get; set; } = "";
    public int BackupRetentionDays { get; set; } = 30;
} 
