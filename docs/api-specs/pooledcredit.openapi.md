---
description: Pooled Credit API outline — endpoints, auth scopes, headers, idempotency, conflicts, settlements
---

# Pooled Credit API

Reference: See shared API standards in ../api-specs/STANDARDS.md for authentication, headers, RFC9457 errors, cursor pagination, idempotency keys, rate limits, webhooks, and auditing.

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
- Loan: id, pool_id, borrower_id, amount, currency, terms, schedule, status, delinquency, etag
- Distribution: id, pool_id, amount, ratio, date

## Endpoints (scopes and semantics)
- POST /creditpools (scope: pooledcredit:write, Idempotency-Key)
  - Create credit pool; 201; 422 validation
- GET /creditpools/{id} (scope: pooledcredit:read)
  - Pool details with balances and utilization
- GET /creditpools (scope: pooledcredit:read)
  - Cursor list; filters: status, currency, size_gt/lt

- POST /creditpools/{id}/contributions (scope: pooledcredit:write, Idempotency-Key)
  - Add member contribution; 201; emits pooledcredit.contribution.recorded

- POST /creditpools/{id}/loans (scope: loans:write, Idempotency-Key)
  - Request loan; requires consent and policy checks; may be async approval
  - 202 with Location for job when async; emits pooledcredit.loan.requested

- GET /loans/{id} (scope: pooledcredit:read)
- PUT /loans/{id} (scope: loans:write; If-Match when ETag present)
  - 412 on ETag mismatch; updates allowed pre-disbursement

- POST /loans/{id}/disburse (scope: loans:write, Idempotency-Key)
  - Disburse approved loan; links to Finance; 201; emits pooledcredit.loan.disbursed

- POST /loans/{id}/repay (scope: loans:write, Idempotency-Key)
  - Record repayment; 201; emits pooledcredit.loan.repaid

- GET /creditpools/{id}/audit (scope: pooledcredit:read)
  - Full audit trail; cursor pagination

## Errors & Conflicts
- RFC9457 problem+json
- 409 on over-allocation beyond pool size or delinquency constraints; 403 consent issues; 422 validation; 412 ETag mismatch

## Webhooks
- pooledcredit.contribution.recorded
- pooledcredit.loan.requested
- pooledcredit.loan.disbursed
- pooledcredit.loan.repaid
- Signed per Standards; include minimal payload (resource id, type, pool_id)

## Examples
- POST /creditpools/{id}/loans -> 202 + Location: /jobs/job_456
- POST /loans/{id}/repay { amount, currency, date } -> 201 { id, status:"APPLIED" }

## Schemas (JSON shape)

CreditPool
```
{
  "id": "pool_01J6Q6TCC0",
  "name": "Main Cooperative Pool",
  "currency": "USD",
  "size": 25000000,
  "utilization": 0.42,
  "policies": {
    "max_loan_ratio": 0.6,
    "delinquency_threshold_days": 30,
    "approval_required": true
  },
  "participating_orgs": ["org_abc", "org_xyz"],
  "status": "ACTIVE",
  "created_at": "2025-08-23T12:34:56Z",
  "updated_at": "2025-08-23T12:34:56Z"
}
```

Contribution (response)
```
{
  "id": "ctrb_01J6Q6W0S8",
  "pool_id": "pool_01J6Q6TCC0",
  "member_id": "mem_123",
  "amount": 500000,
  "currency": "USD",
  "status": "POSTED",
  "posted_at": "2025-08-23T14:00:00Z"
}
```

Loan (response)
```
{
  "id": "loan_01J6Q6Z5G7",
  "pool_id": "pool_01J6Q6TCC0",
  "borrower_id": "mem_123",
  "amount": 1000000,
  "currency": "USD",
  "terms": { "rate_bps": 900, "tenor_months": 12, "amortization": "EQUAL_INSTALLMENTS" },
  "schedule": {
    "frequency": "MONTHLY",
    "first_due_date": "2025-09-30",
    "installments": 12
  },
  "status": "APPROVAL_PENDING",
  "delinquency": null,
  "etag": "\"W/9c6e2b8a\"",
  "created_at": "2025-08-23T15:00:00Z",
  "updated_at": "2025-08-23T15:00:00Z"
}
```

Repayment (request)
```
{
  "amount": 85000,
  "currency": "USD",
  "date": "2025-10-30",
  "reference": "INV-2025-0012"
}
```

## Webhook payload examples

pooledcredit.contribution.recorded
```
{
  "id": "evt_01J6Q72F1S",
  "type": "pooledcredit.contribution.recorded",
  "created_at": "2025-08-23T14:00:02Z",
  "request_id": "01J6Q6PVE6W0",
  "data": {
    "id": "ctrb_01J6Q6W0S8",
    "pool_id": "pool_01J6Q6TCC0",
    "member_id": "mem_123",
    "amount": 500000,
    "currency": "USD",
    "status": "POSTED"
  }
}
```

pooledcredit.loan.disbursed
```
{
  "id": "evt_01J6Q75GKM",
  "type": "pooledcredit.loan.disbursed",
  "created_at": "2025-08-24T10:15:00Z",
  "request_id": "01J6Q75F7PQ1",
  "data": {
    "id": "loan_01J6Q6Z5G7",
    "pool_id": "pool_01J6Q6TCC0",
    "amount": 1000000,
    "currency": "USD",
    "txn_id": "fin_txn_01J6Q75J1M"
  }
}
```

Sign with X-TOSS-Signature per Standards; tolerate clock skew and de-duplicate via event id.

## Notes
- Rate limits: see global headers; on 429 include Retry-After and backoff.
- Finance integration: disbursement/repayment create balanced journal entries; failures MUST roll back pooledcredit state.
