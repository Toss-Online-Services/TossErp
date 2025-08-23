---
description: Rules and best practices for Pooled Credit & Mutual Financing in TOSS ERP III
applyTo: "**/*"
globs: "**/*"
alwaysApply: true
---

# Pooled Credit Instructions
- Manage member contributions, equity stakes, and credit pool size.
- Automate loan requests, approval workflows, and repayment schedules.
- Support collective guarantees, mutual credit scoring, and risk monitoring.
- Log all transactions and profit sharing transparently.
- Enforce secure data sharing, privacy, and cooperative governance.

## Security & Compliance
- OAuth2 scopes: pooledcredit:read, pooledcredit:write, loans:write; data minimization and consent for credit analysis.
- Audit logs on contributions, loan approvals, disbursements, repayments, and profit distributions.
- KYC/AML where applicable; segregation of duties for approvals.

## Acceptance Criteria
- API: RFC7807, cursor pagination, idempotency for contributions and disbursements; rate limits on sensitive operations.
- Conflict handling: 409 on over-allocation beyond pool size or delinquency constraints.
- Cross-links: [Model](mdc:docs/models/pooledcredit.model.md) · [API](mdc:docs/api-specs/pooledcredit.openapi.md) · [Workflow](mdc:docs/architecture/pooledcredit.workflow.md)
- Validate pool accounting integrity, repayment schedules, and member equity tracking.
