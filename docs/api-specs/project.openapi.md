---
description: Project & Service Management API — endpoints, scopes, headers, pagination, ETags, SLA constraints
---

# Project API

Reference: See shared API standards in ../api-specs/STANDARDS.md for authentication, headers, RFC7807 errors, cursor pagination, idempotency keys, rate limits, webhooks, and auditing.

## Security & Scopes
- OAuth2 scopes: project:read, project:write, timesheet:write

## Common Headers
- Authorization: Bearer <token>
- Idempotency-Key on POSTs (project/task creation, timesheet writes, approvals)
- X-Request-Id for tracing

## Resource Model (high-level)
- Project: id, name, client_id, dates, budget, status, manager_id
- Task: id, project_id, title, description, assignee_id, due_date, status, dependencies[]
- Milestone: id, project_id, name, due_date, status
- Timesheet: id, user_id, project_id, task_id, date, hours, status, approvals

## Endpoints
- POST /projects
- GET /projects/{id}
- GET /projects (cursor list; filters: client_id, status, due_before/after)
- POST /projects/{id}/tasks
- POST /projects/{id}/milestones
- POST /timesheets
- POST /timesheets/{id}/approve
- GET /projects/{id}/status
- GET /projects/audit?entity=Task&id=...

## Errors & SLA Conflicts
- RFC7807
- 409 for SLA constraint violations or invalid status transitions
- 412 ETag precondition failure when concurrent edits

## Pagination
- Cursor-based; sort by due_date when requested; consistent envelope
