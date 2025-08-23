---
description: HR & Payroll API — endpoints, scopes, headers, pagination, PII handling, approvals
---

# HR & Payroll API

Reference: See shared API standards in ../api-specs/STANDARDS.md for authentication, headers, RFC9457 errors, cursor pagination, idempotency keys, rate limits, webhooks, and auditing.

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

### POST /hr/employees
- Scopes: hr:write
- Headers: Idempotency-Key required
- Body (PII): { name, email, phone?, address?, role, status }
- Responses: 201 Created with Employee JSON (fields redacted by role); 422 validation

Example request:
```json
{ "name": "Ada Lovelace", "email": "ada@example.com", "role": "ENGINEER", "status": "ACTIVE" }
```

### GET /hr/employees/{id}
- Scopes: hr:read
- Responses: 200 OK with Employee JSON (PII redaction by role); 404 if not visible

### GET /hr/employees
- Scopes: hr:read
- Query: status, role, location, q, limit, cursor
- Response: list envelope with Employee[] (redacted by role)

### POST /hr/attendance
- Scopes: hr:write
- Headers: Idempotency-Key required
- Body: { employee_id, date, check_in, check_out? }
- Responses: 201 Created; 409 if overlapping attendance window; 422 validation

### POST /payroll/runs
- Scopes: payroll:write
- Headers: Idempotency-Key required
- Body: { period: { start, end }, auto_approve?: boolean }
- Responses: 202 Accepted with Location: /jobs/{job_id} (calculation); webhooks payroll.run.progress, payroll.run.completed

### POST /payroll/payslips/{id}/approve
- Scopes: payroll:write
- Headers: Idempotency-Key required
- Body: { approver_id, note? }
- Responses: 200 OK; 409 if already approved or invalid state; 403 if approver not allowed

### GET /hr/employees/{id}/payslips
- Scopes: hr:read or payroll:write
- Query: period_from, period_to, limit, cursor
- Response: list envelope with Payslip[]; redact bank details by role

### GET /hr/audit?entity=Employee&id={id}
- Scopes: hr:read
- Response: list envelope with audit entries (immutable)

## Errors & Compliance
- RFC9457
- 403 for PII access violations; 409 for approval conflicts; 422 validation

Example 403 problem:
```json
{
	"type": "https://api.toss.erp/problems/forbidden",
	"title": "Forbidden",
	"status": 403,
	"detail": "You do not have permission to view full PII for this employee.",
	"code": "PII_FORBIDDEN"
}
```

## Pagination & Filtering
- Cursor-based; ensure PII redaction in list views per role

PII redaction examples:
- Mask email as `a***@example.com` for roles lacking PII privileges
- Omit `address` and `phone` entirely for list endpoints unless `include_pii=true` and scope allows
