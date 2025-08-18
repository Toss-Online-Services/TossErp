namespace TossErp.Sales.Domain.Enums;

/// <summary>
/// Payment methods supported by the POS system
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Cash payment
    /// </summary>
    Cash = 1,

    /// <summary>
    /// Card payment (credit/debit)
    /// </summary>
    Card = 2,

    /// <summary>
    /// Mobile payment (e.g., FNB eWallet, Capitec Pay)
    /// </summary>
    Mobile = 3,

    /// <summary>
    /// Bank transfer/EFT
    /// </summary>
    BankTransfer = 4,

    /// <summary>
    /// Store credit/voucher
    /// </summary>
    StoreCredit = 5,

    /// <summary>
    /// Buy now, pay later services
    /// </summary>
    BuyNowPayLater = 6,

    /// <summary>
    /// Cryptocurrency payment
    /// </summary>
    Cryptocurrency = 7,

    /// <summary>
    /// Account/credit sale
    /// </summary>
    OnAccount = 8
}
