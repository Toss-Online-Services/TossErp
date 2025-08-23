---
description: OpenAPI spec outline for AI Copilot API endpoints
---

# AI Copilot API Spec
- Security: OAuth2; scopes per target module; least privilege enforced
- Standards: see [API Standards](STANDARDS.md); streaming where available
	- All actions are auditable; responses include `X-Request-Id`
- POST /copilot/interact: Start copilot interaction
- GET /copilot/{id}/workflow: Retrieve workflow status
- POST /copilot/{id}/recommend: Submit recommendation
- GET /copilot/{id}/audit: Retrieve audit log
