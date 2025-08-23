---
description: HR & Payroll API — endpoints, scopes, headers, pagination, PII handling, approvals
---

# HR & Payroll API

Reference: See shared API standards in ../api-specs/STANDARDS.md for authentication, headers, RFC7807 errors, cursor pagination, idempotency keys, rate limits, webhooks, and auditing.

## Security & Scopes
- OAuth2 scopes: hr:read, hr:write, payroll:write (PII strongly guarded)
- PII masking/redaction by role; data minimization

## Common Headers
- Authorization: Bearer <token>
- Idempotency-Key required on POST payroll operations, employee creation, corrections
- X-Request-Id for tracing

## Resource Model (high-level)
- Employee: id, name, email, phone, address, role, status, documents[], compensation
- Attendance: id, employee_id, date, check_in, check_out, status
- PayrollRun: id, period, status, totals, approvals, posted_journal_id
- Payslip: id, employee_id, period, gross, net, taxes, deductions, status

## Endpoints
- POST /hr/employees
- GET /hr/employees/{id}
- GET /hr/employees (cursor; filters: status, role, location)
- POST /hr/attendance
- POST /payroll/runs
- POST /payroll/payslips/{id}/approve
- GET /hr/employees/{id}/payslips
- GET /hr/audit?entity=Employee&id=...

## Errors & Compliance
- RFC7807
- 403 for PII access violations; 409 for approval conflicts; 422 validation

## Pagination & Filtering
- Cursor-based; ensure PII redaction in list views per role
