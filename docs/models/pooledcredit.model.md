---
description: Data model for Pooled Credit & Mutual Financing
---

# Pooled Credit Data Model
- Entities: CreditPool, Member, Contribution, Loan, Guarantee, Repayment, CreditScore, RiskMonitor
- Relationships: Many-to-many (CreditPool <-> Member), One-to-many (CreditPool <-> Loan), One-to-many (Loan <-> Repayment)
- Key Fields: pool_size, member_equity, loan_status, repayment_schedule, risk_score
- Audit: Transaction logs, profit sharing, governance actions
