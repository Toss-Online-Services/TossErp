---
description: Rules and best practices for Credit Engine & Financing Hub in TOSS ERP III
applyTo: "**/*"
globs: "**/*"
alwaysApply: true
---

# Credit Engine Instructions
- Automate customer credit management, scoring, and dynamic decisions.
- Integrate with sales, receivables, and financing options.
- Provide risk forecasting, financial health metrics, and AI-driven insights.
- Support external credit bureau and fintech API integrations.
- Enforce data isolation, privacy, and consent for credit analysis.

## Security & Compliance
- OAuth2 scopes: credit:read, credit:write, finance:read; enforce data minimization and consent.
- Audit logs on credit score calculations, overrides, approvals, and financing decisions.
- Model explainability and bias checks where applicable; configurable retention policies.

## Acceptance Criteria
- API: RFC9457, cursor pagination; idempotency for decision requests; rate limits on scoring endpoints.
- Integrations: pluggable connectors for bureaus/fintechs; retries with backoff; circuit breaker patterns.
- Cross-links: [Model](mdc:docs/models/creditengine.model.md) · [API](mdc:docs/api-specs/creditengine.openapi.md) · [Workflow](mdc:docs/architecture/creditengine.workflow.md)
- Validate decisioning outcomes, overrides with audit, and downstream finance linkage.
