---
description: Pooled Credit API outline — endpoints, auth scopes, headers, idempotency, conflicts, settlements
---

# Pooled Credit API

Reference: See shared API standards in ../api-specs/STANDARDS.md for authentication, headers, RFC7807 errors, cursor pagination, idempotency keys, rate limits, webhooks, and auditing.

## Security & Scopes
- OAuth2 scopes: pooledcredit:read, pooledcredit:write, loans:write
- Consent and eligibility checks enforced; role-based access

## Common Headers
- Authorization: Bearer <token>
- Idempotency-Key: required on POSTs that create/modify state (pool create, contribution, loan request, disbursement, repayment)
- X-Request-Id: optional caller-generated for tracing

## Resource Model (high-level)
- CreditPool: id, name, currency, participating_orgs[], size, utilization, policies, status
- Contribution: id, pool_id, member_id, amount, currency, date, status
- Loan: id, pool_id, borrower_id, amount, currency, terms, schedule, status, delinquency
- Distribution: id, pool_id, amount, ratio, date

## Endpoints
- POST /creditpools
  - Create credit pool; 201; 422 validation
- GET /creditpools/{id}
  - Pool details with balances and utilization
- GET /creditpools
  - Cursor list; filters: status, currency, size_gt/lt

- POST /creditpools/{id}/contributions
  - Add member contribution (idempotent); emits contribution.recorded

- POST /creditpools/{id}/loans
  - Request loan; requires consent and policy checks; may be async approval
  - 202 with Location for job when async; emits loan.requested

- POST /loans/{id}/disburse
  - Disburse approved loan; links to Finance; idempotent

- POST /loans/{id}/repay
  - Record repayment; idempotent; emits loan.repaid

- GET /creditpools/{id}/audit
  - Full audit trail; cursor pagination

## Errors & Conflicts
- RFC7807
- 409 on over-allocation beyond pool size or policy; 403 consent issues; 422 validation

## Webhooks
- contribution.recorded, loan.requested, loan.disbursed, loan.repaid
- HMAC-signed per standards
