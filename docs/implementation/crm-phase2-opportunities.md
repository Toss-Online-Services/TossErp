# Phase 2: Opportunity & Deal Management

## 2.1 Opportunity Pipeline Management

### Enhanced Opportunity System
```csharp
public class Opportunity : AggregateRoot
{
    // Core opportunity data
    public string Name { get; private set; }
    public Money Value { get; private set; }
    public decimal Probability { get; private set; }
    public DateTime ExpectedCloseDate { get; private set; }
    
    // Pipeline management
    public OpportunityStage Stage { get; private set; }
    public StageHistory StageHistory { get; private set; }
    public List<OpportunityProduct> Products { get; private set; }
    public List<Competitor> Competitors { get; private set; }
    
    // Sales process
    public SalesProcess SalesProcess { get; private set; }
    public List<Milestone> Milestones { get; private set; }
    public RiskAssessment Risk { get; private set; }
}
```

### Opportunity Features
- **Stage Progression**: Automated stage advancement rules
- **Probability Calculation**: Dynamic probability based on stage/activities
- **Product Configuration**: Multiple products per opportunity
- **Competitor Tracking**: Competitive analysis and win/loss reasons
- **Revenue Forecasting**: Weighted pipeline calculations

## 2.2 Quotation & Proposal System

### Quote Management
```typescript
interface Quotation {
  id: string;
  opportunityId: string;
  customerId: string;
  version: number;
  status: 'draft' | 'sent' | 'viewed' | 'accepted' | 'rejected';
  validUntil: Date;
  items: QuotationItem[];
  terms: string;
  totalAmount: Money;
  discounts: Discount[];
}
```

### Quote Features
- **Dynamic Pricing**: Rule-based pricing engine
- **Approval Workflow**: Multi-level quote approval
- **Version Control**: Quote revision tracking
- **E-signature**: Digital signature integration
- **Quote Analytics**: View tracking, conversion rates

### UI Components
```vue
<!-- QuotationBuilder.vue -->
<template>
  <div class="quotation-builder">
    <QuoteHeader :quote="quote" />
    <ProductSelector @add-product="addProduct" />
    <QuoteLineItems 
      :items="quote.items"
      @update-item="updateItem"
    />
    <QuotePricing 
      :items="quote.items"
      :discounts="quote.discounts"
    />
    <QuoteActions 
      :quote="quote"
      @send="sendQuote"
      @save="saveQuote"
    />
  </div>
</template>
```

## 2.3 Deal Closing & Won/Lost Analysis

### Closing Process
```csharp
public class DealClosing : Entity
{
    public Guid OpportunityId { get; private set; }
    public ClosingOutcome Outcome { get; private set; }
    public DateTime ClosedDate { get; private set; }
    public Money ActualValue { get; private set; }
    public string WinLossReason { get; private set; }
    public List<LessonLearned> Lessons { get; private set; }
    public CompetitorAnalysis Competitors { get; private set; }
}
```

### Features
- **Win/Loss Tracking**: Detailed outcome analysis
- **Reason Categorization**: Structured win/loss reasons
- **Competitor Intelligence**: Win/loss by competitor
- **Sales Velocity**: Time to close analytics
- **Deal Post-mortem**: Team learning capture

## 2.4 Revenue Forecasting

### Forecasting Engine
```typescript
interface ForecastModel {
  period: 'monthly' | 'quarterly' | 'annual';
  methodology: 'probability' | 'historical' | 'stages';
  accuracy: number;
  predictions: ForecastPrediction[];
}

interface ForecastPrediction {
  period: string;
  predictedRevenue: Money;
  confidence: number;
  pipeline: OpportunityForecast[];
}
```

### Forecasting Features
- **Multiple Models**: Probability-based, historical, stage-based
- **Confidence Intervals**: Prediction accuracy tracking
- **Trend Analysis**: Revenue trend identification
- **Goal Tracking**: Target vs. actual comparison
- **Team Forecasts**: Individual and team predictions

### Implementation Structure
```
src/Services/crm/
├── Crm.Domain/
│   ├── Aggregates/
│   │   ├── Opportunity.cs (enhanced)
│   │   ├── Quotation.cs (new)
│   │   └── Forecast.cs (new)
│   ├── Services/
│   │   ├── OpportunityService.cs
│   │   ├── QuotationService.cs
│   │   ├── ForecastingService.cs
│   │   └── PricingService.cs
│   └── ValueObjects/
│       ├── Money.cs
│       ├── SalesStage.cs
│       └── CompetitorInfo.cs
```
