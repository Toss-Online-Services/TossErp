description: Rules and best practices for Reporting & Analytics module in TOSS ERP III
applyTo: "**/*"
globs: "**/*"
alwaysApply: true
---

# Reporting & Analytics Module Instructions
- Provide interactive dashboards and custom report builder.
- Support real-time KPIs, forecasting, and trend analysis.
- Enable data exports (Excel, PDF) and API access for BI tools.
- Integrate audit trails and change logs across all modules.
- Enforce data permissions and report scheduling.

## Security & Compliance
- OAuth2 scopes: reporting:read, reporting:write; enforce dataset-level permissions.
- Audit logs on report definition changes and scheduled runs.

## Acceptance Criteria
- API: RFC7807 errors, cursor pagination; async export job IDs; rate limits on heavy queries.
- Cross-links: [Model](mdc:docs/models/reporting.model.md) · [API](mdc:docs/api-specs/reporting.openapi.md) · [Workflow](mdc:docs/architecture/reporting.workflow.md)
- Validate permission filters, schedule triggers, and export integrity.
