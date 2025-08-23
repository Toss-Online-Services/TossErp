---
description: OpenAPI spec outline for AI Copilot API endpoints
---

# AI Copilot API Spec
- Security: OAuth2; scopes per target module; least privilege enforced
- Standards: see [API Standards](STANDARDS.md); RFC9457 errors; streaming where available
		- All actions are auditable; responses include `X-Request-Id`

## Endpoints
- POST /copilot/interact (scopes: copilot:write + module-specific read; Idempotency-Key)
	- Start conversation or action; may stream responses.
- GET /copilot/{id}/workflow (scope: copilot:read)
	- Retrieve current workflow state; 200.
- POST /copilot/{id}/recommend (scope: copilot:write; Idempotency-Key)
	- Submit a recommendation/action plan; emits copilot.recommendation.created.
- GET /copilot/{id}/audit (scope: copilot:read)
	- Retrieve audit log; cursor pagination.

## Webhooks
- copilot.recommendation.created
- copilot.workflow.completed

## Example
- POST /copilot/interact { prompt: "Generate AR aging report" } -> 202 Location: /jobs/job_abc
