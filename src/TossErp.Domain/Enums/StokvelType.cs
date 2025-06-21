namespace TossErp.Domain.Enums
{
    public enum StokvelType
    {
        // Traditional Stokvels
        Contribution = 1,        // Members contribute and receive lump sum in rotation
        Grocery = 2,             // Members contribute for bulk grocery purchases
        Purchasing = 3,          // Members contribute for large purchases (furniture, appliances)
        Family = 4,              // Family-based savings group
        Investment = 5,          // Investment-focused stokvel
        Party = 6,               // Social event funding
        Borrowing = 7,           // Lending and borrowing among members
        
        // Specialized Stokvels
        Funeral = 8,             // Funeral expense coverage
        Education = 9,           // School fees and education expenses
        Business = 10,           // Business startup and expansion funding
        Property = 11,           // Property investment and development
        Vehicle = 12,            // Vehicle purchase and maintenance
        
        // Community Development
        CommunityProject = 13,   // Community infrastructure projects
        SkillsDevelopment = 14,  // Training and skills development
        YouthDevelopment = 15,   // Youth-focused initiatives
        
        // Other
        Other = 16
    }
} 
