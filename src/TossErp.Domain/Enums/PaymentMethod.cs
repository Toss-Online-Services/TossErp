namespace TossErp.Domain.Enums
{
    public enum PaymentMethod
    {
        // Traditional Payment Methods
        Cash = 1,
        BankTransfer = 2,
        CreditCard = 3,
        DebitCard = 4,
        Cheque = 5,
        
        // Digital Payment Methods
        MobileMoney = 6,         // M-Pesa, VodaPay, etc.
        EWallet = 7,             // Digital wallets
        QRCode = 8,              // QR code payments
        USSD = 9,                // USSD-based payments
        
        // Informal Payment Methods
        Barter = 10,             // Goods/services exchange
        Credit = 11,             // Buy now, pay later
        Layby = 12,              // Lay-by system
        Installment = 13,        // Installment payments
        
        // Community-Based Payment Methods
        StokvelCredit = 14,      // Credit from stokvel
        CooperativeCredit = 15,  // Credit from cooperative
        CommunityCredit = 16,    // Community-based credit
        
        // Government and NGO
        Voucher = 17,            // Government/NGO vouchers
        Grant = 18,              // Government grants
        Subsidy = 19,            // Subsidized payments
        
        // Other
        Other = 20
    }
} 
