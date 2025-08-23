description: Rules and best practices for Financial Management module in TOSS ERP III
applyTo: "**/*"
globs: "**/*"
alwaysApply: true
---

# Finance Module Instructions
- Use double-entry accounting principles for all financial transactions.
- Support multi-currency and multi-company setups.
- Automate journal entries from sub-ledgers (AP, AR, Payroll).
- Ensure compliance with local tax and audit requirements.
- Integrate with budgeting, reporting, and cooperative settlement features.
- Provide robust APIs for external integrations (bank feeds, payment gateways).
- Enforce role-based access for sensitive financial data.

## Security & Compliance
- OAuth2 scopes: finance:read, finance:write, payments:write; enforce least privilege.
- Mandatory audit trails (who, what, when, source doc) for all journal-affecting actions.
- Regional tax handling and period closing controls; approval workflows for postings.

## Acceptance Criteria
- API adheres to: RFC9457 errors, cursor pagination, idempotency keys on POST, rate limits.
- Cross-links available: [Model](mdc:docs/models/finance.model.md) · [API](mdc:docs/api-specs/finance.openapi.md) · [Workflow](mdc:docs/architecture/finance.workflow.md)
- Tests validate balanced entries, currency conversion, and lockout on closed periods.
