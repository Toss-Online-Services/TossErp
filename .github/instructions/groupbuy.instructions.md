---
description: Rules and best practices for Group Buying & Collective Procurement in TOSS ERP III
applyTo: "**/*"
globs: "**/*"
alwaysApply: true
---

# Group Buying Instructions
- Enable joint purchase requests and aggregation of member commitments.
- Automate vendor negotiation, RFQs, and contract management.
- Support split shipping, allocation, and settlement workflows.
- Log savings and ensure transparent accounting for all participants.
- Enforce governance, permissions, and member criteria for group buys.

## Security & Compliance
- OAuth2 scopes: groupbuy:read, groupbuy:write, procurement:write; enforce member eligibility and approval workflows.
- Immutable audit log for commitments, RFQs, vendor selections, and allocation decisions.
- Multi-company and role-based visibility; consent for cross-organization data sharing.

## Acceptance Criteria
- API: RFC9457, cursor pagination, idempotency for commitments and RFQ responses; rate-limiting on negotiation endpoints.
- Conflict handling: 409 when commitments exceed capacity or deadlines; transparent allocation rules.
- Cross-links: [Model](mdc:docs/models/groupbuy.model.md) · [API](mdc:docs/api-specs/groupbuy.openapi.md) · [Workflow](mdc:docs/architecture/groupbuy.workflow.md)
- Validate end-to-end flow: Request → Commitments → RFQ/Vendor → Allocation → Settlement.
