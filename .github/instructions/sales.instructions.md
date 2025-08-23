description: Rules and best practices for Sales & Customer Management module in TOSS ERP III
applyTo: "**/*"
globs: "**/*"
alwaysApply: true
---

# Sales Module Instructions
- Track leads, opportunities, quotations, and sales orders in a unified workflow.
- Integrate sales orders with inventory, fulfillment, and invoicing.
- Support B2B and B2C sales processes.
- Enable CRM features: customer database, communication logs, analytics.
- Automate conversion of quotes to orders and orders to invoices.
- Provide APIs for e-commerce and marketplace integration.
- Enforce data privacy and customer consent management.

## Security & Compliance
- OAuth2 scopes: sales:read, sales:write, crm:write; consent and Do-Not-Contact flags enforced.
- PII minimized and access-logged; approvals required on discount thresholds.

## Acceptance Criteria
- API standards: RFC9457 errors, cursor pagination, search/filter, idempotency on POST.
- Cross-links: [Model](mdc:docs/models/sales.model.md) · [API](mdc:docs/api-specs/sales.openapi.md) · [Workflow](mdc:docs/architecture/sales.workflow.md)
- E2E path: Lead→Opportunity→Quote→Order→Invoice; inventory reservation on order.
