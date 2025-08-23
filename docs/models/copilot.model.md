---
description: Data model for AI Business Co-Pilot & Voice Assistant
---

# AI Copilot Data Model
- Entities: User, Interaction, Workflow, Insight, Recommendation, AuditLog
- Relationships: One-to-many (User <-> Interaction), One-to-many (Workflow <-> Insight), One-to-many (User <-> Recommendation)
- Key Fields: interaction_type, workflow_status, insight_score, recommendation_id
- Audit: All interactions and recommendations logged for security and improvement
