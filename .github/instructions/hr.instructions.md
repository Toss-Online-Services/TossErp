description: Rules and best practices for Human Resources & Payroll module in TOSS ERP III
applyTo: "**/*"
globs: "**/*"
alwaysApply: true
---

# HR & Payroll Module Instructions
- Maintain secure employee records, contracts, and documents.
- Automate payroll processing, leave management, and attendance tracking.
- Support compliance reporting and benefits administration.
- Enable employee self-service portal for payslips, leave, and updates.
- Integrate with project costing and sales commission workflows.
- Enforce strict data privacy and access controls.

## Security & Compliance
- OAuth2 scopes: hr:read, hr:write, payroll:write; PII and payroll data masked by role.
- Mandatory audit on payroll runs, approvals, and corrections with reasons.

## Acceptance Criteria
- API: RFC9457 errors, cursor pagination; PII redaction on list endpoints; rate limits on payroll operations.
- Cross-links: [Model](mdc:docs/models/hr.model.md) · [API](mdc:docs/api-specs/hr.openapi.md) · [Workflow](mdc:docs/architecture/hr.workflow.md)
- Validate payroll calculations, leave accruals, and approval flows.
