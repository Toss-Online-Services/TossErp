namespace POS.Domain.AggregatesModel.StoreAggregate;

public class StoreSettings
{
    public bool EnableTax { get; private set; }
    public decimal TaxRate { get; private set; }
    public bool EnableDiscounts { get; private set; }
    public int MaxDiscountPercentage { get; private set; }
    public bool RequireCustomerInfo { get; private set; }
    public bool EnableLoyaltyProgram { get; private set; }
    public string ReceiptHeader { get; private set; }
    public string ReceiptFooter { get; private set; }
    public bool EnableStockAlerts { get; private set; }
    public bool EnableEmailNotifications { get; private set; }
    public bool EnableSMSNotifications { get; private set; }
    public string? EmailTemplate { get; private set; }
    public string? SMSTemplate { get; private set; }

    public StoreSettings()
    {
        EnableTax = false;
        TaxRate = 0;
        EnableDiscounts = false;
        MaxDiscountPercentage = 0;
        RequireCustomerInfo = false;
        EnableLoyaltyProgram = false;
        ReceiptHeader = string.Empty;
        ReceiptFooter = string.Empty;
        EnableStockAlerts = false;
        EnableEmailNotifications = false;
        EnableSMSNotifications = false;
        EmailTemplate = null;
        SMSTemplate = null;
    }

    public StoreSettings(
        bool enableTax,
        decimal taxRate,
        bool enableDiscounts,
        int maxDiscountPercentage,
        bool requireCustomerInfo,
        bool enableLoyaltyProgram,
        string receiptHeader,
        string receiptFooter,
        bool enableStockAlerts = false,
        bool enableEmailNotifications = false,
        bool enableSMSNotifications = false,
        string? emailTemplate = null,
        string? smsTemplate = null)
    {
        EnableTax = enableTax;
        TaxRate = taxRate;
        EnableDiscounts = enableDiscounts;
        MaxDiscountPercentage = maxDiscountPercentage;
        RequireCustomerInfo = requireCustomerInfo;
        EnableLoyaltyProgram = enableLoyaltyProgram;
        ReceiptHeader = receiptHeader;
        ReceiptFooter = receiptFooter;
        EnableStockAlerts = enableStockAlerts;
        EnableEmailNotifications = enableEmailNotifications;
        EnableSMSNotifications = enableSMSNotifications;
        EmailTemplate = emailTemplate;
        SMSTemplate = smsTemplate;
    }
}
