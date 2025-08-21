namespace TossErp.CRM.Domain.Enums;

/// <summary>
/// Lead status in sales pipeline - inspired by ERPNext Lead management
/// </summary>
public enum LeadStatus
{
    New = 1,
    Open = 2,
    Replied = 3,
    Interested = 4,
    Qualified = 5,
    Unqualified = 6,
    Disqualified = 7,
    Lost = 8,
    Converted = 9,
    DoNotContact = 10
}

/// <summary>
/// Lead source tracking - for marketing ROI analysis
/// </summary>
public enum LeadSource
{
    Website = 1,
    SocialMedia = 2,
    Email = 3,
    Phone = 4,
    Referral = 5,
    Advertisement = 6,
    TradeShow = 7,
    ColdCall = 8,
    Partner = 9,
    DirectMail = 10,
    Other = 11
}

/// <summary>
/// Opportunity/Deal stage in sales pipeline
/// </summary>
public enum OpportunityStage
{
    Prospecting = 1,
    Qualification = 2,
    NeedsAnalysis = 3,
    ValueProposition = 4,
    Proposal = 5,
    Negotiation = 6,
    ClosedWon = 7,
    ClosedLost = 8,
    OnHold = 9
}

/// <summary>
/// Customer relationship status
/// </summary>
public enum CustomerStatus
{
    Prospect = 1,
    Active = 2,
    Inactive = 3,
    Churned = 4,
    Suspended = 5,
    VIP = 6
}

/// <summary>
/// Customer type classification for SaaS business
/// </summary>
public enum CustomerType
{
    Individual = 1,
    SmallBusiness = 2,
    Enterprise = 3,
    NonProfit = 4,
    Government = 5,
    Partner = 6,
    Reseller = 7
}

/// <summary>
/// Contact role in organization
/// </summary>
public enum ContactRole
{
    DecisionMaker = 1,
    Influencer = 2,
    User = 3,
    TechnicalContact = 4,
    FinancialContact = 5,
    ProjectManager = 6,
    Administrator = 7,
    Other = 8
}

/// <summary>
/// Activity type for CRM tracking
/// </summary>
public enum ActivityType
{
    Call = 1,
    Email = 2,
    Meeting = 3,
    Task = 4,
    Note = 5,
    Demo = 6,
    Proposal = 7,
    Contract = 8,
    Support = 9,
    Training = 10
}

/// <summary>
/// Activity priority level
/// </summary>
public enum ActivityPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}

/// <summary>
/// Activity status
/// </summary>
public enum ActivityStatus
{
    Scheduled = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
    Overdue = 5
}

/// <summary>
/// Communication direction
/// </summary>
public enum CommunicationDirection
{
    Inbound = 1,
    Outbound = 2
}

/// <summary>
/// Campaign type for marketing automation
/// </summary>
public enum CampaignType
{
    Email = 1,
    SocialMedia = 2,
    PaidAdvertising = 3,
    ContentMarketing = 4,
    Webinar = 5,
    TradeShow = 6,
    DirectMail = 7,
    Referral = 8,
    Partnership = 9,
    Other = 10
}

/// <summary>
/// Campaign status
/// </summary>
public enum CampaignStatus
{
    Planning = 1,
    Active = 2,
    Paused = 3,
    Completed = 4,
    Cancelled = 5
}

/// <summary>
/// Deal/Opportunity priority
/// </summary>
public enum OpportunityPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

/// <summary>
/// Sales territory classification
/// </summary>
public enum Territory
{
    NorthAmerica = 1,
    Europe = 2,
    AsiaPacific = 3,
    LatinAmerica = 4,
    MiddleEast = 5,
    Africa = 6,
    Global = 7
}

/// <summary>
/// Subscription/contract status for SaaS customers
/// </summary>
public enum SubscriptionStatus
{
    Trial = 1,
    Active = 2,
    PastDue = 3,
    Cancelled = 4,
    Expired = 5,
    Suspended = 6,
    Paused = 7
}

/// <summary>
/// Customer tier classification for SaaS business model
/// </summary>
public enum CustomerTier
{
    Basic = 1,
    Standard = 2,
    Premium = 3,
    Enterprise = 4
}

/// <summary>
/// Contact type classification
/// </summary>
public enum ContactType
{
    General = 1,
    Primary = 2,
    Secondary = 3,
    Technical = 4,
    Billing = 5,
    Support = 6
}

/// <summary>
/// Note type classification
/// </summary>
public enum NoteType
{
    General = 1,
    Meeting = 2,
    Call = 3,
    Follow = 4,
    Important = 5,
    Internal = 6
}

/// <summary>
/// Communication type
/// </summary>
public enum CommunicationType
{
    Email = 1,
    Phone = 2,
    SMS = 3,
    Meeting = 4,
    VideoCall = 5,
    Chat = 6,
    SocialMedia = 7,
    Letter = 8
}

/// <summary>
/// Communication status
/// </summary>
public enum CommunicationStatus
{
    Completed = 1,
    Failed = 2,
    Pending = 3,
    Cancelled = 4
}

/// <summary>
/// Document type classification
/// </summary>
public enum DocumentType
{
    General = 1,
    Contract = 2,
    Proposal = 3,
    Invoice = 4,
    Receipt = 5,
    Agreement = 6,
    Specification = 7,
    Presentation = 8,
    Brochure = 9,
    Other = 10
}

/// <summary>
/// Opportunity type classification
/// </summary>
public enum OpportunityType
{
    NewBusiness = 1,
    Existing = 2,
    Renewal = 3,
    Upgrade = 4,
    CrossSell = 5,
    Upsell = 6
}

/// <summary>
/// Campaign response type
/// </summary>
public enum CampaignResponseType
{
    Opened = 1,
    Clicked = 2,
    Responded = 3,
    Converted = 4,
    Unsubscribed = 5
}
