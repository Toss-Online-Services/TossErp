description: Reporting & Analytics API
---

# Reporting API Outline
- Security: OAuth2, scopes: reporting:read, reporting:write
- Standards: see [API Standards](STANDARDS.md) for errors (RFC9457), pagination, idempotency, and rate limits
  - Long-running exports return async Job (see jobs.openapi.md): 202 + Location

## Resources
- Report: id, name, dataset, query(def), filters, visualization, owner_id, schedule?, status
- Dashboard: id, name, widgets[], owner_id
- Export: id, report_id, format(PDF|CSV|XLSX), status, url?

## Endpoints
- GET /reports (scope: reporting:read)
  - Filters: dataset, owner_id, name, created_from/to
- POST /reports (scope: reporting:write, Idempotency-Key)
  - 201; 422 invalid query; emits reporting.report.created
- GET /reports/{id} (scope: reporting:read)
- PUT /reports/{id} (scope: reporting:write, If-Match if ETag present)

- POST /dashboards (scope: reporting:write, Idempotency-Key)
- GET /dashboards/{id} (scope: reporting:read)

- POST /reports/{id}/export (scope: reporting:write, Idempotency-Key)
  - Body: { format, filters? }
  - Returns 202 with Location: /jobs/{jobId}
  - On completion, Job.result -> { url, expires_at }
  - Webhook: reporting.export.ready

- POST /reports/{id}/schedule (scope: reporting:write, Idempotency-Key)
  - Creates or updates a schedule; 201/200. Validates cron/timezone and recipients.

- GET /reporting/audit (scope: reporting:read) — entity, id

## Errors
- RFC9457: 413 for heavy ad-hoc exports (advise async); 429 with rate-limit headers; 422 invalid filters/query

## Webhooks
- reporting.report.created
- reporting.export.ready
  - Signed per Standards; thin payload with id/type

## Example
- POST /reports/{id}/export -> 202 with Location: /jobs/job_123
