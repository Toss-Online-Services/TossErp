description: TOSS ERP III API Standards (errors, pagination, idempotency, rate limits, headers)
---

# API Standards

These conventions apply to all TOSS ERP III APIs.

## Authentication & Authorization
- OAuth2 bearer tokens
- Scopes are least-privilege; each module documents required scopes per endpoint

## Request/Response Metadata
- Request ID: responses include `X-Request-Id` header (string). Clients may send `X-Request-Id` to correlate.
- Content types: JSON for requests/responses; errors use `application/problem+json`
- Idempotency header: `Idempotency-Key` on POST/PUT/PATCH that change state (see Idempotency)
- Common headers: `Accept`, `Content-Type`, `Authorization`, `X-Request-Id`, `Idempotency-Key`

## Errors (Problem Details RFC 9457)
- Media type: `application/problem+json` (RFC 9457, which obsoletes RFC 7807)
- Members: `type` (absolute URI recommended), `title`, `status`, `detail`, `instance`
- Extensions: `code` (machine code), `traceId`, `errors` (array of `{ name|pointer, reason }` for validation), and domain-specific fields
- Common statuses: 400, 401, 403, 404, 409, 412 (precondition), 422, 429, 500
- Use `422 Unprocessable Content` for validation failures with field-level `errors`
- Problem type URIs SHOULD resolve to human-readable docs; use absolute URIs or full-path relative URIs
- Language negotiation: `Content-Language` and `Accept-Language` MAY be used for localized `title`/`detail`

Example (validation error):

```json
{
  "type": "https://api.toss.erp/problems/validation-error",
  "title": "Your request is not valid.",
  "status": 422,
  "detail": "See errors for field-level details.",
  "instance": "https://api.toss.erp/instances/01J6Q3H2ZP6QH9X2Y3GJ4B7N8K",
  "code": "VALIDATION_FAILED",
  "traceId": "01J6Q3H2ZP6QH9X2Y3GJ4B7N8K",
  "errors": [
    { "pointer": "#/start", "reason": "must be before end" },
    { "name": "quantity", "reason": "must be a positive integer" }
  ]
}
```

## Idempotency
- Provide `Idempotency-Key` header for POST/PUT operations that create/update resources
- Keys: client-generated UUID v4 (up to 255 chars), safe to retry for 24 hours
- Same key + same parameters MUST return the same result
 - Safe to retry on network timeouts and 5xx; return 201 on first, 200 on subsequent identical retries

## Pagination (Cursor)
- Requests: `limit` (1..100, default 20), `cursor` (opaque)
- Responses: envelope with `items` (array), `next_cursor` (nullable), `prev_cursor` (nullable), `has_more` (bool), `total` (optional, expensive)
- Sorting: stable descending by `created_at` unless specified

## Filtering & Search
- `q` for free-text search (module-specific fields)
- Structured filtering via query fields (e.g., `status`, `date_from`, `date_to`)
- Avoid RQL; keep simple, additive filters

## Rate Limiting
- Headers: `X-RateLimit-Limit`, `X-RateLimit-Remaining`, `X-RateLimit-Reset`
- On 429: include `Retry-After` (seconds); clients should use exponential backoff with jitter
 - Bulk/async endpoints may have separate lower limits; document per-module exceptions

Example headers:

```
X-RateLimit-Limit: 120
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 1724412345
Retry-After: 30
```

## Concurrency & Versioning
- Optimistic concurrency via `If-Match`/`ETag` where applicable; 412 on mismatch
- Semantic versioning via `Accept` header (e.g., custom vendor versions) when needed
 - For GETs, support `If-None-Match` returning `304 Not Modified` where feasible

Example conditional update:

```
PUT /assets/ast_123
If-Match: "W/\"e3b0c44298fc1c14\""
Idempotency-Key: 6b1c6068-08fb-4c0d-9b39-0a7a7a845a6d
Content-Type: application/json

{ "status": "INACTIVE" }

HTTP/1.1 412 Precondition Failed
Content-Type: application/problem+json

{ "type": "https://api.toss.erp/problems/precondition-failed", "title": "ETag mismatch", "status": 412, "detail": "The resource has changed since you retrieved it." }
```

## Webhooks & Events
- Event payloads include `id`, `type`, `created_at`, `data` object, and `request_id`
- Sign webhooks and document signature verification
 - Include `X-TOSS-Signature` header (HMAC-SHA256 over raw body with shared secret); provide timestamp tolerance to avoid replay
 - Retries: at-least-once delivery with exponential backoff; de-dupe via event `id`
 - Respond with 2xx to ack; non-2xx triggers retries

Signature verification example (Node.js):

```js
import crypto from 'crypto';

export function verifySignature(rawBody, signatureHeader, secret) {
  const expected = crypto
    .createHmac('sha256', secret)
    .update(rawBody)
    .digest('hex');
  return crypto.timingSafeEqual(Buffer.from(signatureHeader, 'hex'), Buffer.from(expected, 'hex'));
}
```

## Auditing
- Write operations produce audit entries with actor, timestamp, before/after, and source document references
 - Store `request_id` and `idempotency_key` (if provided) with audit records

## Localization & Time
- All timestamps in ISO 8601 UTC (e.g., `2025-08-23T12:34:56Z`)
- Currency amounts in minor units (integers) with `currency` code

## Standard Response Envelope (Lists)
```json
{
  "items": [ /* entities */ ],
  "next_cursor": "opaque",
  "prev_cursor": null,
  "has_more": true,
  "total": 123
}
```

## Async Jobs
- Long-running operations SHOULD return `202 Accepted` with `Location: /jobs/{job_id}`
- Job resource includes: `id`, `status` (pending|running|succeeded|failed), `percent_complete`, `result` (on success), `error` (problem+json on failure), and `created_at/updated_at`
- Clients poll `GET /jobs/{job_id}` or subscribe to webhooks if available

Example job resource:

```json
{
  "id": "job_01J6Q5W8C3M7D2K9",
  "status": "running",
  "percent_complete": 65,
  "result": null,
  "error": null,
  "created_at": "2025-08-23T12:34:56Z",
  "updated_at": "2025-08-23T12:36:10Z"
}
```

## Retries & Backoff (Client Guidance)
- Retry on `429` (respect `Retry-After`), `503`, connection resets/timeouts, and idempotent POSTs with `Idempotency-Key`
- Use exponential backoff with jitter; cap retries; log `X-Request-Id`
