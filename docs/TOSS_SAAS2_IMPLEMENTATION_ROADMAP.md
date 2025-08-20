TOSS ERP III — Service-as-a-Software Implementation Roadmap

Purpose
- Provide a phased, actionable roadmap to implement Service-as-a-Software capabilities in TOSS ERP III. Each phase lists deliverables, acceptance criteria, and low-risk MVP tasks.

Assumptions
- Core platform is .NET-based and uses an EventBus (RabbitMQ) as shown in the repo.
- Team can add new microservices inside `src/` and follow existing build/deploy patterns.
- Access to an LLM provider (Azure OpenAI, OpenAI, or on-prem equivalent) will be provisioned by the team.

Checklist of requirements (from user brief)
- [ ] Deliver architecture & API/agent contract doc (`docs/SERVICE_AS_A_SOFTWARE.md`) — Done
- [ ] Provide phased implementation roadmap — Done (this file)
- [ ] Create initial scaffolds for Orchestrator and AgentManager services — TODO
- [ ] Define minimal OpenAPI contracts and Temporal workflows — TODO
- [ ] Provide tests and CI tasks for new services — TODO

Phase 0 — Preparation (1-2 weeks)
- Tasks:
  - [ ] Create `docs/SERVICE_AS_A_SOFTWARE.md` (done)
  - [ ] Choose orchestration engine (Temporal recommended) and provisioning plan
  - [ ] Choose LLM provider and setup API keys/secrets in `.env` or secret store
  - [ ] Add `docs/TOSS_SAAS2_IMPLEMENTATION_ROADMAP.md` (done)
- Acceptance:
  - Decision logged in repo `docs/` and team aligned.

Phase 1 — Core scaffolding & MVP (2-4 weeks)
- Goals: implement minimal runnable orchestrator, an AgentManager endpoint, and an Inventory autopilot that demonstrates end-to-end flow.
- Tasks:
  - [ ] Create microservice `src/Orchestrator` with OpenAPI and a simple `/orchestrator/execute` and `/orchestrator/workflow/{id}` backed by an in-memory workflow store initially.
  - [ ] Create microservice `src/AgentManager` with `/agents/intent` and `/agents/authorize-action` endpoints.
  - [ ] Implement `src/Connectors/WhatsAppConnector` as a simple webhook receiver that posts messages to `AgentManager` (use ngrok locally or Twilio simulator for dev).
  - [ ] Implement Inventory autopilot flow: nightly job reads stock, creates reorder suggestion, sends to owner via webhook/connector.
  - [ ] Add unit tests for endpoints and a simple integration test that simulates an intent -> orchestrator flow.
- Acceptance:
  - Postman/Swagger demo shows the autopilot creating a reorder suggestion and storing a workflow.

Phase 2 — Orchestration & Reliability (4-8 weeks)
- Goals: swap in Temporal (or chosen engine), implement durable workflows, retries, compensation, and visibility.
- Tasks:
  - [ ] Integrate Temporal .NET SDK in `src/Orchestrator` and define `ReorderWorkflow`, `InvoiceWorkflow`, `DailySummaryWorkflow`.
  - [ ] Replace in-memory store with Temporal workflows and ensure `/orchestrator/execute` starts a Temporal workflow.
  - [ ] Implement retries, timeouts, and compensation patterns for partial failures.
  - [ ] Add OpenTelemetry traces and Prometheus metrics for workflows and agents.
- Acceptance:
  - Workflows visible in Temporal UI; retries and compensations exercised in tests.

Phase 3 — Agent intelligence & grounding (4-8 weeks)
- Goals: connect LLMs, implement grounding (RAG), add model selection policies, and safety checks.
- Tasks:
  - [ ] Add `src/LLMAdapter` responsible for model calls, prompt templates, and response parsing.
  - [ ] Implement a `GroundingService` that retrieves context (inventory, suppliers, recent events) and builds RAG inputs.
  - [ ] Wire `AgentManager` to use `LLMAdapter` + `GroundingService` and produce suggested actions.
  - [ ] Implement human-in-the-loop approval UI (simple web UI or webhook flows) for high-risk actions.
- Acceptance:
  - Agents produce consistent suggested actions and can be approved/rejected by a user.

Phase 4 — Integrations & production readiness (6-12 weeks)
- Goals: add real connectors (WhatsApp via Twilio, Supplier APIs), security hardening, multi-tenant considerations, and outcome billing.
- Tasks:
  - [ ] Implement Twilio/WhatsApp connector for outbound messages and webhooks.
  - [ ] Implement connector scaffolds for common supplier APIs (mock providers first, then real integrations).
  - [ ] Implement tenant isolation, RBAC, and audit trail storage.
  - [ ] Implement metering service and billing events for outcomes (e.g., invoice_sent).
  - [ ] Add E2E tests, chaos tests (simulate supplier downtime), and run security scans.
- Acceptance:
  - Production-ready connectors, policy enforcement, and outcome-based billing events.

Phase 5 — Iterate, expand agents, and productize
- Goals: roll out domain agents (Marketing, Payroll), optimize models, and productize outcome pricing.
- Tasks:
  - Expand agent catalog, create templates for new agents, and support tenant-specific customization.
  - Add A/B testing harness to measure automation A/B vs manual.
  - Improve model evaluation with human feedback loops.

Developer Notes & Quick-start (local dev)
1) Create a new folder `src/Orchestrator` and `src/AgentManager`.
2) Start with minimal .NET WebAPI templates (use `dotnet new webapi`).
3) Add simple in-memory store for workflows initially; later swap to Temporal.
4) Add a `tests/` project for unit and integration tests.

CI / Testing
- Add GitHub Actions job to build new services, run unit tests, and run a small integration test using local mocks.

Open tasks for the team
- [ ] Provision LLM provider and add secrets to dev environment.
- [ ] Decide on Temporal vs Durable Functions and provision infrastructure.
- [ ] Assign owners for `Orchestrator`, `AgentManager`, and `Connectors` services.

Contact & governance
- Create a weekly engineering sync during the MVP phases and record decisions in `docs/decisions.md`.

Status: roadmap added to `docs/`. Next: scaffold minimal services if you want — I can create the WebAPI scaffolds and initial tests in `src/` now.
