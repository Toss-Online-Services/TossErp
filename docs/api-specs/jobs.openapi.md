---
description: Async Jobs API outline — pollable long-running operations per STANDARDS
---

# Jobs API

Reference: See shared API standards in ./STANDARDS.md for async jobs contract and problem details.

## Security & Scopes
- OAuth2 bearer; scopes vary by originating module; reads typically allowed with the same module read scope.

## Resource
- Job: id, status (pending|running|succeeded|failed), percent_complete (0-100), result (object|null), error (problem+json|null), created_at, updated_at, metadata

## Endpoints
- GET /jobs/{job_id}
  - Scopes: module read scope
  - 200 with Job JSON
  - 404 if job unknown or not visible

- DELETE /jobs/{job_id}
  - Scopes: module write scope
  - 204 on success (best-effort cancel/cleanup when possible)
  - 409 if job already completed and cannot be cancelled

## Examples
```json
{
  "id": "job_01J6Q9R6W2",
  "status": "succeeded",
  "percent_complete": 100,
  "result": { "export_url": "https://files.toss.erp/exp_abc.csv" },
  "error": null,
  "created_at": "2025-08-23T12:00:00Z",
  "updated_at": "2025-08-23T12:05:13Z"
}
```

On failure:
```json
{
  "id": "job_01J6Q9R6W2",
  "status": "failed",
  "percent_complete": 80,
  "result": null,
  "error": {
    "type": "https://api.toss.erp/problems/export-failed",
    "title": "Export generation failed",
    "status": 500,
    "detail": "Stream aborted due to upstream timeout.",
    "traceId": "01J6Q9VV8E"
  },
  "created_at": "2025-08-23T12:00:00Z",
  "updated_at": "2025-08-23T12:03:59Z"
}
```
