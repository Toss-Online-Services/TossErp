description: Rules and best practices for Project & Service Management module in TOSS ERP III
applyTo: "**/*"
globs: "**/*"
alwaysApply: true
---

# Project Module Instructions
- Support project planning, milestones, task dependencies, and resource allocation.
- Integrate time tracking, timesheets, and approvals.
- Link project costs to finance and payroll modules.
- Enable service delivery management and field service scheduling.
- Provide APIs for mobile app and client portal integration.
- Enforce role-based access and project audit logs.

## Security & Compliance
- OAuth2 scopes: project:read, project:write, timesheet:write; role-based access to projects.
- Audit logs on task transitions, approvals, and SLA breaches.

## Acceptance Criteria
- API: RFC7807 errors, cursor pagination, sort by due_date; rate limits on timesheet writes.
- Cross-links: [Model](mdc:docs/models/project.model.md) · [API](mdc:docs/api-specs/project.openapi.md) · [Workflow](mdc:docs/architecture/project.workflow.md)
- Validate timesheet approval, SLA checks, and finance linkage for billing.
