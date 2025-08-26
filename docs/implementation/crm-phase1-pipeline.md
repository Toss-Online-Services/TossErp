# Phase 1: Sales Pipeline & Lead Management Implementation

## 1.1 Visual Sales Pipeline

### Frontend Components
```
toss-web/
├── components/crm/
│   ├── SalesPipeline.vue - Main pipeline board
│   ├── PipelineStage.vue - Individual stage column
│   ├── OpportunityCard.vue - Draggable deal cards
│   ├── PipelineFilters.vue - Filtering controls
│   └── StageMetrics.vue - Stage performance stats
```

### Key Features
- **Drag & Drop**: Vue.Draggable for opportunity movement
- **Real-time Updates**: WebSocket for collaborative editing
- **Customizable Stages**: Admin configurable pipeline stages
- **Probability Tracking**: Weighted pipeline value calculations
- **Activity Indicators**: Recent activity, overdue tasks

### Backend Requirements
```csharp
// New API Endpoints
GET /api/crm/pipeline/stages
GET /api/crm/pipeline/opportunities
PUT /api/crm/opportunities/{id}/stage
POST /api/crm/pipeline/stages
```

### Implementation Steps
1. Create pipeline domain models and stage configuration
2. Build Vue.js pipeline board with drag-drop functionality
3. Implement WebSocket real-time updates
4. Add pipeline analytics and metrics calculation
5. Create mobile-responsive pipeline view

## 1.2 Enhanced Lead Management

### Lead Qualification System
```typescript
interface LeadQualificationCriteria {
  budget: number;
  authority: boolean;
  need: string;
  timeline: string;
  score: number;
}
```

### Features
- **BANT Qualification**: Budget, Authority, Need, Timeline scoring
- **Lead Scoring Rules**: Configurable scoring algorithms
- **Auto-Assignment**: Round-robin or territory-based assignment
- **Lead Nurturing**: Automated follow-up sequences
- **Conversion Tracking**: Lead-to-customer journey analytics

### UI Enhancements
- Lead scoring visualization
- Qualification checklist interface
- Bulk lead operations
- Lead import/export functionality
- Territory management interface
