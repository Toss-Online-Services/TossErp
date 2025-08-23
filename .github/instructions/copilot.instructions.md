---
description: Rules and best practices for AI Business Co-Pilot & Voice Assistant in TOSS ERP III
applyTo: "**/*"
globs: "**/*"
alwaysApply: true
---

# AI Copilot Instructions
- Provide conversational interface (chat/voice) for all modules.
- Automate multi-step workflows and Q&A with context awareness.
- Surface AI-powered insights, trends, and recommendations.
- Learn from user interactions and adapt to roles/preferences.
- Enforce security, audit logs, and permission-based data access.

## Security & Compliance
- OAuth2 scopes aligned per module; context access must respect least privilege and data sensitivity.
- Audit all assistant actions (queries, mutations, automations) with actor, timestamp, and consent.
- Privacy: PII masking/redaction in prompts; configurable data retention and model usage policies.

## Acceptance Criteria
- API: RFC9457 errors; rate limits and abuse protection; streaming responses for voice where available.
- Cross-links: [Model](mdc:docs/models/copilot.model.md) · [API](mdc:docs/api-specs/copilot.openapi.md) · [Workflow](mdc:docs/architecture/copilot.workflow.md)
- Validate role-aware responses, safe automations, and proper audit/consent handling.
