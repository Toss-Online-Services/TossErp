---
description: Data model for Credit Engine & Financing Hub
---

# Credit Engine Data Model
- Entities: Customer, CreditProfile, CreditScore, Decision, FinancingOption, RiskForecast, BureauIntegration
- Relationships: One-to-many (Customer <-> CreditProfile), One-to-many (CreditProfile <-> Decision), One-to-many (Customer <-> FinancingOption)
- Key Fields: credit_score, decision_status, risk_metrics, consent_status
- Audit: Credit analysis logs, consent records
